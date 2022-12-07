using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LittleGame_TouchCat : LittleGame
{
    public Image pointCircle;
    public float fillSpeed;

    public override void StartGame(Cat cat)
    {
        base.StartGame(cat);
        pointCircle.fillAmount = 0;
    }

    public void Drag()
    {
        pointCircle.fillAmount += fillSpeed;

        if (pointCircle.fillAmount >= 1)
        {
            Close();

            //ExitAnim
            anim.SetBool(CatAnimTable.IsCanExit.ToString(), true);

            App.system.confirm.OnlyConfirm().Active(endId, () =>
            {
                Success();
                cat.catHeartEffect.Play();
                OpenLobby();
            });
        }
    }
}
