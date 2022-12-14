using System.Collections;
using System.Collections.Generic;
using Firebase.Firestore;
using UnityEngine;

[CreateAssetMenu(fileName = "ACR0006", menuName = "Factory/Quests/ACR/Create ACR0006")]
public class ACR0006 : AchieveQuest
{
    public override void Init()
    {
        App.system.myTime.OnFirstLogin += AddProgress;
    }

    private void AddProgress()
    {
        Progress++;
    }

    // public override int Progress
    // {
    //     get
    //     {
    //         var day = (Timestamp.GetCurrentTimestamp().ToDateTime() - App.system.myTime.AccountCreateDateTime).Days;
    //         App.system.quest.QuestProgressData[commonId] = day;
    //         return App.system.quest.QuestProgressData[commonId];
    //     }
    //     set
    //     {
    //         App.system.quest.QuestProgressData[commonId] = value;
    //     }
    // }
}
