﻿using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Lobby : ControllerBehavior
{
    public void Open()
    {
        App.system.bgm.FadeIn().Play("Lobby");
        App.view.lobby.Open();
        App.system.catNotify.PopUp();
    }
    
    [Button]
    public void Test()
    {
        App.system.player.AddExp(20);
    }

    public void Close()
    {
        App.view.lobby.Close();
    }

    public void OpenBuildMode()
    {
        Close();
        App.controller.build.Open();
    }

    public void OpenBag()
    {
        App.controller.bag.Open();
        Close();
    }

    public void OpenFeed()
    {
        App.controller.feed.Open();
        Close();
    }

    public void OpenMap()
    {
        App.system.cat.ToggleCatsGameTimer(true);

        App.system.bgm.FadeOut();
        App.system.transition.Active(0, () =>
        {
            Close();
            App.controller.map.Open();
        });
    }

    public void OpenScreenshot()
    {
        App.system.screenshot.OnScreenshotComplete += CloseScreenshot;
        App.system.screenshot.OnClose += CloseScreenshot;
        Close();
        App.system.screenshot.Open();
    }

    private void CloseScreenshot()
    {
        Open();
        App.system.screenshot.OnScreenshotComplete -= CloseScreenshot;
        App.system.screenshot.OnClose -= CloseScreenshot;
    }

    public void OpenInformation()
    {
        Close();
        App.controller.information.Open();
    }

    public void OpenFriend()
    {
        Close();
        App.controller.friend.Open();
    }

    public void OpenMall()
    {
        App.controller.mall.Open();
        DOVirtual.DelayedCall(0.25f, () => App.controller.mall.SelectPage(0));
    }

    public void OpenTopUp()
    {
        App.controller.mall.Open();
        DOVirtual.DelayedCall(0.25f, () => App.controller.mall.SelectPage(6));
    }

    public void OpenCatGuide()
    {
        App.controller.catGuide.Open();
    }

    public void OpenDailyQuest()
    {
        App.controller.dailyQuest.Open();
    }
}