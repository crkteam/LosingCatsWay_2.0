using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class View_EntranceDiary : ViewBehaviour
{
    [Title("Book")] [SerializeField] private Card_EntranceDiary leftBook;
    [SerializeField] private Card_EntranceDiary rightBook;
    [SerializeField] private Card_EntranceDiary centerBook;

    [Title("UI")] [SerializeField] private TextMeshProUGUI catNameText;

    public override void Init()
    {
        base.Init();
        App.model.entrance.OnSortedLosingCatDatasChange += OnSortedLosingCatDatasChange;
    }

    private void OnSortedLosingCatDatasChange(object value)
    {
        var losingCatDatas = (List<CloudLosingCatData>)value;

        Vector2 originCenter = new Vector2(0.6f, 0.6f);
        Vector2 originLeft = new Vector2(0.38f, 0.38f);
        Vector2 originRight = new Vector2(0.38f, 0.38f);

        centerBook.transform.DOScale(originCenter, 0.3f).From(Vector2.zero).SetEase(Ease.OutExpo).SetDelay(0.05f);
        leftBook.transform.DOScale(originLeft, 0.3f).From(Vector2.zero).SetEase(Ease.OutExpo).SetDelay(0.05f);
        rightBook.transform.DOScale(originRight, 0.3f).From(Vector2.zero).SetEase(Ease.OutExpo).SetDelay(0.05f)
            .OnStart(() =>
            {
                catNameText.text = losingCatDatas[0].CatData.CatName;
                
                //0中間 1左邊 2右邊
                for (int i = 0; i < 3; i++)
                {
                    //center
                    if (i == 0)
                    {
                        centerBook.SetData(losingCatDatas[0]);
                        continue;
                    }

                    //left
                    if (i == 1)
                    {
                        if (i > losingCatDatas.Count - 1)
                        {
                            leftBook.SetData(null);
                            continue;
                        }

                        leftBook.SetData(losingCatDatas[i]);
                        continue;
                    }
            
                    //right
                    if (i > losingCatDatas.Count - 1)
                        rightBook.SetData(null);
                    else
                        rightBook.SetData(losingCatDatas[i]);
                }
            });
    }
}
