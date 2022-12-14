using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card_CatNotify : MvcBehaviour
{
    [SerializeField] private CatSkin catSkin;
    [SerializeField] private RectTransform bubbleRect;
    [SerializeField] private RectTransform chatRect;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject redDot;
    [SerializeField] private Button button;

    [Title("Dotween")]
    [SerializeField] private float popDuration;
    [SerializeField] private Ease openEase;
    [SerializeField] private Ease closeEase;

    public Callback OnClick;

    private Sequence popSeq;

    public void Init()
    {
        bubbleRect.localScale = Vector3.zero;
        chatRect.localScale = Vector3.zero;
        catSkin.SetActive(false);
        gameObject.SetActive(false);
        button.interactable = true;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => 
        {
            Click();
            button.interactable = false;
        });
    }

    [Button(30)]
    public void Open(Cat cat)
    {
        gameObject.SetActive(true);

        CloudCatData cloudCatData = cat.cloudCatData;
        catSkin.ChangeSkin(cloudCatData);
        catSkin.SetActive(true);

        text.text = App.factory.stringFactory.GetCatNotify(cat.catNotifyId);

        popSeq.Kill();
        popSeq = DOTween.Sequence();

        popSeq
            .OnStart(() =>
            {
                //button.interactable = false;
                redDot.SetActive(false);
                bubbleRect.localScale = Vector3.zero;
                chatRect.localScale = Vector3.zero;
            })
            .Append(bubbleRect.DOScale(Vector3.one, popDuration).From(Vector3.zero).SetEase(openEase))
            .Append(chatRect.DOScale(Vector3.one, popDuration).From(Vector3.zero).SetEase(openEase))
            .AppendInterval(1f)
            .Append(chatRect.DOScale(Vector3.zero, popDuration).From(Vector3.one).SetEase(closeEase))
            .OnComplete(() =>
            {
                //button.interactable = true;
                redDot.SetActive(true);
            });
    }

    [Button(30)]
    public void Close()
    {
        catSkin.SetActive(false);

        popSeq.Kill();
        popSeq = DOTween.Sequence();

        popSeq
            .OnStart(() =>
            {
                //button.interactable = false;
                redDot.SetActive(false);
                bubbleRect.localScale = Vector3.one;
                chatRect.localScale = Vector3.one;
            })
            .Append(bubbleRect.DOScale(Vector3.zero, popDuration).From(Vector3.one).SetEase(closeEase))
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                App.system.catNotify.PopUp();
            });
    }

    public void Click()
    {
        OnClick?.Invoke();
        OnClick = null;
        Close();
        App.system.soundEffect.Play("Button");
        VibrateExtension.Vibrate(VibrateType.Nope);
    }
}
