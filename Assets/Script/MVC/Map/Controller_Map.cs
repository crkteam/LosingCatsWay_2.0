using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Doozy.Runtime.UIManager.Components;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Controller_Map : ControllerBehavior
{
    [Title("G8")] [SerializeField] private bool isG8;
    [SerializeField] private UIButton lobbyMapButton;
    [SerializeField] private GameObject lobbyMapMask;
    
    public Callback OnMapOpen;
    
    public void Open()
    {
        App.system.bgm.FadeIn().Play("Map");
        App.view.map.Open();
        OnMapOpen?.Invoke();
    }

    public void OpenShelter()
    {
        App.system.bgm.FadeOut();
        App.system.transition.Active(0.5f, () =>
        {
            App.view.map.Close();
            App.controller.shelter.Open();
        });
    }

    public void OpenFindCat(int index)
    {
        App.system.bgm.FadeOut();
        App.system.transition.OnlyOpen(() =>
        {
            App.view.map.Close();
            App.system.findCat.ActiveMap(index);
        });
    }

    public void OpenShop()
    {
        App.system.bgm.FadeOut();
        App.system.transition.Active(0.5f, () =>
        {
            App.view.map.Close();
            App.controller.shop.Open();
        });
    }

    public void OpenHospital()
    {
        App.system.bgm.FadeOut();
        App.system.transition.Active(0.5f, () =>
        {
            App.view.map.Close();
            App.controller.clinic.Open();
        });
    }

    public void OpenGreenHouse()
    {
        App.system.bgm.FadeOut();
        App.system.transition.Active(0.5f, () =>
        {
            App.view.map.Close();
            App.controller.greenHouse.Open();
        });
    }

    public void OpenPark()
    {
        App.system.bgm.FadeOut();
        App.system.transition.Active(0.5f, () =>
        {
            App.view.map.Close();
            App.controller.park.Open();
        });
    }

    public void Close()
    {
        if (isG8)
        {
            lobbyMapButton.interactable = false;
            lobbyMapMask.SetActive(true);
        }
        
        App.system.cat.ToggleCatsGameTimer(false);
        App.system.bgm.FadeOut();
        App.system.transition.Active(0, () =>
        {
            App.view.map.Close();
            DOVirtual.DelayedCall(0.175f, App.controller.lobby.Open);
        });
    }
}