using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Firebase.Firestore;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Controller_Shelter : ControllerBehavior
{
    [SerializeField] private Card_ChipInfo info;
    [SerializeField] private Scrollbar scrollbar;

    [Title("Quest")] [SerializeField] private SHR001 freeRefresh;
    [SerializeField] private SHR002 adsRefresh;

    public CallbackValue OnAdoptCat;
    public Callback OnFreeRefresh;
    public Callback OnAdsRefresh;
    
    #region Basic

    public void Init()
    {
        App.system.myTime.OnFirstLogin += ResetRefreshPerDay;

        UpdateRefresh();
        
        freeRefresh.Init();
        adsRefresh.Init();
    }
    
    public void Open()
    {
        App.system.bgm.FadeIn().Play("Shelter");
        App.view.shelter.Open();
        GetCloudCatDatas();
        scrollbar.value = 0;
    }

    public void Close()
    {
        App.system.bgm.FadeOut();
        App.system.transition.Active(0, () =>
        {
            App.view.shelter.Close();
            App.controller.map.Open();
        });
    }

    public void OpenAbandon()
    {
        App.system.abandon.Active("Shelter");
    }

    public void OpenSubShelter()
    {
        App.view.shelter.subShelter.Open();
    }

    public void CloseSubShelter()
    {
        App.view.shelter.subShelter.Close();
        info.CloseInfo();
    }

    #endregion

    #region Cats

    public async void GetCloudCatDatas()
    {
        List<CloudCatData> cloudCatDatas = await App.system.cloudSave.LoadCloudCatDatasByOwner("Shelter", 12);

        App.model.shelter.CloudCatDatas = cloudCatDatas;
        
        for (int i = 0; i < App.view.shelter.cages.Length; i++)
        {
            App.view.shelter.cages[i].button.interactable = true;
            App.view.shelter.cages[i].catSkin.SetActive(true);
        }
    }

    #endregion
    
    #region Refresh
    
    public void RefreshShelterCats()
    {
        if (!freeRefresh.IsReach)
        {
            App.system.confirm.Active(ConfirmTable.RefreshConfirm, () => 
            {
                GetCloudCatDatas();
                scrollbar.value = 0;
                
                OnFreeRefresh?.Invoke();
                UpdateRefresh();
            });
            return;
        }

        if (!adsRefresh.IsReach)
        {
            //TODO Ads Refresh Confirm
            App.system.confirm.Active(ConfirmTable.Fix, () => 
            {
                //TODO Ads
                GetCloudCatDatas();
                scrollbar.value = 0;
                
                OnAdsRefresh?.Invoke();
                UpdateRefresh();

                if (App.model.shelter.AdsRefresh <= 0)
                    return;

                App.model.shelter.Cooldown = App.system.myTime.MyTimeNow.AddMinutes(1);
                InvokeRepeating(nameof(CooldownCounter), 1f, 1f);
            });
        }
    }

    private void CooldownCounter()
    {
        if (App.model.shelter.Cooldown <= App.system.myTime.MyTimeNow)
            CancelInvoke(nameof(CooldownCounter));
        App.model.shelter.Cooldown = App.model.shelter.Cooldown;
    }

    private void UpdateRefresh()
    {
        App.model.shelter.FreeRefresh = freeRefresh.TargetCount - freeRefresh.Progress;

        if (App.model.shelter.FreeRefresh > 0)
            return;
        
        App.model.shelter.AdsRefresh = adsRefresh.TargetCount - adsRefresh.Progress;
    }
    
    private void ResetRefreshPerDay()
    {
        freeRefresh.Progress = 0;
        adsRefresh.Progress = 0;
    }

    #endregion

    #region Search

    public async void Search()
    {
        string searchCatId = App.view.shelter.inputField.text;

        App.view.shelter.inputField.text = string.Empty;

        var cloudCatData = await App.system.cloudSave.LoadCloudCatDataById(searchCatId);

        if (cloudCatData != null)
        {
            if (cloudCatData.CatData.Owner != "Shelter")
            {
                App.system.confirm.OnlyConfirm().Active(ConfirmTable.Fix);
                return;
            }

            App.model.shelter.SelectedAdoptCloudCatData = cloudCatData;
            OpenSubShelter();
        }
        else
        {
            App.system.confirm.OnlyConfirm().Active(ConfirmTable.Fix);
        }
    }

    #endregion

    #region Adopt

    public void SelectAdopt(int index)
    {
        App.model.shelter.SelectedAdoptCloudCatData = App.model.shelter.CloudCatDatas[index];
        App.model.shelter.SelectedCageIndex = index;
        OpenSubShelter();
    }

    public async void Adopt()
    {
        CloseSubShelter();

        if (App.system.player.CanAdoptCatCount <= 0)
        {
            int count = App.system.room.FeatureRoomsCount;

            if (App.system.player.CatSlot >= count)
                App.system.confirm.OnlyConfirm().Active(ConfirmTable.NeedMoreFeedRoom);
            else
                App.system.confirm.OnlyConfirm().Active(ConfirmTable.NeedMoreCatSlot);
            
            return;
        }

        if (await CheckIsAdopted())
        {
            App.system.confirm.OnlyConfirm().Active(ConfirmTable.Fix);
            CloseSubShelter();
            return;
        }

        App.system.confirm.Active(ConfirmTable.AdoptConfirm, okEvent:() =>
        {
            var cloudCatData = App.model.shelter.SelectedAdoptCloudCatData;

            Cat cat = App.system.cat.CreateCatObject(cloudCatData);
            cat.GetLikeSnack();
            cat.GetLikeSoup();

            cloudCatData.CatData.Owner = App.system.player.PlayerId;
            App.system.cloudSave.UpdateCloudCatData(cloudCatData);

            cloudCatData.CatDiaryData.AdoptLocation = "Shelter";
            cloudCatData.CatDiaryData.AdoptTimestamp = Timestamp.GetCurrentTimestamp();
            App.system.cloudSave.UpdateCloudCatDiaryData(cloudCatData);

            CloseCage(cloudCatData.CatData.CatId);
            
            OnAdoptCat?.Invoke(cloudCatData);
            
            App.system.catRename.CantCancel().Active(cloudCatData, "Shelter", () =>
            {
                DOVirtual.DelayedCall(0.1f, () => App.system.confirm.OnlyConfirm().Active(ConfirmTable.HasNewCat));
            });
        });
    }

    private void CloseCage(string CatId)
    {
        App.view.shelter.cages[App.model.shelter.SelectedCageIndex].SetActive(false);
        
        //ValueChange
        List<CloudCatData> cats = App.model.shelter.CloudCatDatas;
        for (int i = 0; i < cats.Count; i++)
        {
            if (cats[i].CatData.CatId != CatId) continue;
            cats.RemoveAt(i);
        }

        App.model.shelter.CloudCatDatas = cats;
    }

    private async Task<bool> CheckIsAdopted()
    {
        string id = App.model.shelter.SelectedAdoptCloudCatData.CatData.CatId;

        var cloudCatData = await App.system.cloudSave.LoadCloudCatDataById(id);

        if (cloudCatData == null)
        {
            return true;
        }

        return false;
    }

    #endregion

    #region Chip

    public void ToggleInfo()
    {
        info.ToggleInfo();
    }

    #endregion

    #region SubShelter

    public void CopySubShelterCatId()
    {
        string CatId = App.model.shelter.SelectedAdoptCloudCatData.CatData.CatId;
        CatId.CopyToClipboard();
        App.system.confirm.OnlyConfirm().Active(ConfirmTable.Copied);
    }

    #endregion
}