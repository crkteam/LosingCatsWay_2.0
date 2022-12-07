using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class View_ClinicResult : ViewBehaviour
{
    [Title("Top")]
    [SerializeField] private CatSkin catSkin;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image moodImage;
    [SerializeField] private GameObject[] genderObjects; //��r�M�ϰ��@��
    [SerializeField] private TextMeshProUGUI ageText;
    [SerializeField] private TextMeshProUGUI sizeText;
    [SerializeField] private TextMeshProUGUI idText;
    
    [Title("Down")]
    //[SerializeField] private GameObject[] healthObjects; //���d��r
    [SerializeField] private TextMeshProUGUI sickNameText;
    [SerializeField] private GameObject[] sickLevels;
    [SerializeField] private TextMeshProUGUI sickInfoText;
    [SerializeField] private TextMeshProUGUI metCountText;
    [SerializeField] private CanvasGroup contentGroup;
    //TODO Icon

    [Title("MetCount")] [SerializeField] private GameObject metCountObject;
    [SerializeField] private GameObject cantMetObject;

    Queue<string> resultIds; //���h�֯f����ݭn½��

    private Cat cat;

    public override void Init()
    {
        base.Init();
        App.model.clinic.OnSelectedCatChange += OnSelectedCatChange;
        App.model.clinic.OnPaymentChange += OnPaymentChange;
    }

    public override void Open()
    {
        base.Open();
        catSkin.SetActive(true);
        ReadResult();
    }

    public override void Close()
    {
        base.Close();
        catSkin.SetActive(false);
    }

    private void OnSelectedCatChange(object value)
    {
        cat = (Cat)value;

        catSkin.ChangeSkin(cat.cloudCatData);

        nameText.text = cat.cloudCatData.CatData.CatName;

        int mood = CatExtension.GetCatMood(cat.cloudCatData);
        moodImage.sprite = App.factory.catFactory.GetMoodSprite(mood);

        for (int i = 0; i < genderObjects.Length; i++)
        {
            if (i == cat.cloudCatData.CatData.Sex)
                genderObjects[i].SetActive(true);
            else
                genderObjects[i].SetActive(false);
        }

        ageText.text = cat.cloudCatData.CatData.CatAge.ToString();
        sizeText.text = $"{CatExtension.GetCatRealSize(cat.cloudCatData.CatData.BodyScale):0.00}cm";
        idText.text = $"ID:{cat.cloudCatData.CatData.CatId}";

        //TODO HealthObjects
    }

    private void OnPaymentChange(object value)
    {
        var payment = (Dictionary<string, int>)value;
        resultIds = new Queue<string>();
        for (int i = 0; i < payment.Count; i++)
        {
            resultIds.Enqueue(payment.ElementAt(i).Key);
        }
    }

    public void ReadResult()
    {
        if (resultIds.Count > 0)
        {
            contentGroup.DOFade(0, 0.25f).From(1);

            DOVirtual.DelayedCall(0.25f, () =>
            {
                ChangeContent();
            });

            contentGroup.DOFade(1, 0.25f).From(0).SetDelay(.5f);
        }
        else
        {
            //TODO �ѽ��X
            App.controller.clinic.CloseCheckResult();
            App.controller.clinic.OpenChooseFunction();
        }
    }

    private void ChangeContent()
    {
        if (resultIds.Count <= 0) return;
        string id = resultIds.Dequeue();

        //�^�E
        if (id == "CP001")
        {
            sickNameText.text = "CP001";
            sickInfoText.text = "CP001";
            
            for (int i = 0; i < sickLevels.Length; i++)
            {
                int sickLevel = App.factory.sickFactory.GetSickLevel(cat.cloudCatData.CatHealthData.SickId);
                if (i == sickLevel)
                    sickLevels[i].SetActive(true);
                else
                    sickLevels[i].SetActive(false);
            }

            int count = App.factory.sickFactory.GetMetCount(cat.cloudCatData.CatHealthData.SickId);
            if (count < 0)
            {
                metCountObject.SetActive(false);
                cantMetObject.SetActive(true);
            }
            else
            {
                metCountText.text = count.ToString();
                metCountObject.SetActive(true);
                cantMetObject.SetActive(false);
            }
            return;
        }

        //�̭]
        if (id == "CP002")
        {
            sickNameText.text = "CP002";
            sickInfoText.text = "CP002";
            metCountText.text = "-";

            for (int i = 0; i < sickLevels.Length; i++)
                sickLevels[i].SetActive(false);

            sickLevels[3].SetActive(true);
            return;
        }

        //�w�����D
        if (id == "CP003")
        {
            sickNameText.text = "CP003";
            sickInfoText.text = "CP003";
            metCountText.text = "-";

            for (int i = 0; i < sickLevels.Length; i++)
                sickLevels[i].SetActive(false);

            sickLevels[3].SetActive(true);
            return;
        }

        //����
        if (id == "CP004")
        {
            sickNameText.text = "CP004";
            sickInfoText.text = "CP004";
            metCountText.text = "-";

            for (int i = 0; i < sickLevels.Length; i++)
                sickLevels[i].SetActive(false);

            sickLevels[3].SetActive(true);
            return;
        }

        //����
        if (id == "CP005" || id == "CP006")
        {
            sickNameText.text = "CP005+6";
            sickInfoText.text = "CP005+6";
            metCountText.text = "-";

            for (int i = 0; i < sickLevels.Length; i++)
                sickLevels[i].SetActive(false);

            sickLevels[3].SetActive(true);
            return;
        }

        //����
        if (id == "CP007")
        {
            sickNameText.text = "CP007";
            sickInfoText.text = "CP007";
            metCountText.text = "-";

            for (int i = 0; i < sickLevels.Length; i++)
                sickLevels[i].SetActive(false);

            sickLevels[3].SetActive(true);
            return;
        }
    }
}
