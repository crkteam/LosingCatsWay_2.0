using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.Common.Extensions;
using Firebase.Firestore;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiaryFactory : SerializedMonoBehaviour
{
    #region Get

    private int GetAdoptDays(CloudCatData cloudCatData)
    {
        int result = 0;

        var adoptTime = cloudCatData.CatDiaryData.AdoptTimestamp.ToDateTime();
        var deathTime = cloudCatData.CatData.DeathTime.ToDateTime();
        result = (deathTime - adoptTime).Days + 1;

        return result;
    }

    private DateTime GetRandomDate(DateTime adoptDate, int days, float min, float max)
    {
        return adoptDate.AddDays(days * Random.Range(min, max));
    }

    private string GetDiaryTone(CloudCatData cloudCatData, string diaryId)
    {
        //沒有容器個性(2)
        if (!cloudCatData.CatData.PersonalityTypes.Contains(2))
        {
            return diaryId + "_E"; //普通口吻
        }
        
        //有容器個性(2)
        int level = 0;
            
        for (int i = 0; i < cloudCatData.CatData.PersonalityTypes.Count; i++)
        {
            if (cloudCatData.CatData.PersonalityTypes[i] != 2)continue;
            level = cloudCatData.CatData.PersonalityLevels[i];
        }

        switch (level)
        {
            case 0:
                return diaryId + "_D"; //孤僻口吻
            case 1:
                return diaryId + "_C"; //怕生口吻
            case 2:
                return diaryId + "_B"; //隨和口吻
            case 3:
                return diaryId + "_A"; //溫馴口吻
            default:
                return diaryId + "_E"; //普通口吻
        }
    }

    #endregion

    public List<CloudSave_DiaryData> GetDiaryDatas(CloudCatData cloudCatData)
    {
        //初見
        CloudSave_DiaryData hello = GetDiaryHello(cloudCatData);

        //相處
        CloudSave_DiaryData together = GetDiaryTogether(cloudCatData);

        //個性
        CloudSave_DiaryData personal = GetDiaryPersonal(cloudCatData);

        //生病
        CloudSave_DiaryData sick = GetDiarySick(cloudCatData);

        //告別
        CloudSave_DiaryData bye = GetDiaryBye(cloudCatData);

        List<CloudSave_DiaryData> result = new List<CloudSave_DiaryData>();
        result.Add(hello);
        result.Add(together);
        if (personal != null) result.Add(personal);

        int ageLevel = CatExtension.GetCatAgeLevel(cloudCatData.CatData.SurviveDays);
        if (ageLevel != 0)
        {
            CloudSave_DiaryData interact = GetDiaryInteract(cloudCatData);
            result.Add(interact);
            CloudSave_DiaryData record = GetDiaryRecord(cloudCatData);
            if (record != null) result.Add(record);
        }

        if (sick != null)
            result.Add(sick);

        result.Add(bye);
        
        return result;
    }

    //初見
    private CloudSave_DiaryData GetDiaryHello(CloudCatData cloudCatData)
    {
        CloudSave_DiaryData result = new CloudSave_DiaryData();
        int ageLevel = CatExtension.GetCatAgeLevel(cloudCatData.CatData.SurviveDays);

        string adoptLocation = cloudCatData.CatDiaryData.AdoptLocation;
        result.DiaryDate = Timestamp.FromDateTime(cloudCatData.CatDiaryData.AdoptTimestamp.ToDateTime());

        if (ageLevel == 0)
        {
            switch (adoptLocation)
            {
                case "Shelter":
                    result.DiaryId = "101";
                    break;
                case "OutSide":
                    result.DiaryId = "102";
                    break;
                case "Home":
                    result.DiaryId = "103";
                    break;
            }
        }
        else
        {
            switch (adoptLocation)
            {
                case "Shelter":
                    result.DiaryId = "105";
                    break;
                case "OutSide":
                    result.DiaryId = "106";
                    break;
            }
        }

        result.DiaryId = GetDiaryTone(cloudCatData, result.DiaryId);

        return result;
    }

    //相處
    private CloudSave_DiaryData GetDiaryTogether(CloudCatData cloudCatData)
    {
        CloudSave_DiaryData result = new CloudSave_DiaryData();
        int ageLevel = CatExtension.GetCatAgeLevel(cloudCatData.CatData.SurviveDays);
        DateTime adoptDate = cloudCatData.CatDiaryData.AdoptTimestamp.ToDateTime();

        result.DiaryDate = Timestamp.FromDateTime(GetRandomDate(adoptDate, GetAdoptDays(cloudCatData), 0.15f, 0.37f));

        if (ageLevel == 0)
            result.DiaryId = (200 + Random.Range(1, 6)).ToString();
        else
            result.DiaryId = (205 + Random.Range(1, 6)).ToString();

        result.DiaryId = GetDiaryTone(cloudCatData, result.DiaryId);

        return result;
    }

    //個性
    private CloudSave_DiaryData GetDiaryPersonal(CloudCatData cloudCatData)
    {
        CloudSave_DiaryData result = new CloudSave_DiaryData();
        DateTime adoptDate = cloudCatData.CatDiaryData.AdoptTimestamp.ToDateTime();

        var types = cloudCatData.CatData.PersonalityTypes;
        var levels = cloudCatData.CatData.PersonalityLevels;

        if (!types.Contains(0) && !types.Contains(1) && !types.Contains(3)) return null;

        List<string> tmp = new List<string>();

        for (int i = 0; i < types.Count; i++) //刷出所有可以用的個性日記Ids
        {
            int type = types[i];
            int level = levels[i];
            string value = String.Empty;

            switch (type)
            {
                case 0:
                    value = (304 - level).ToString();
                    break;
                case 1:
                    value = (308 - level).ToString();
                    break;
                case 3:
                    value = (312 - level).ToString();
                    break;
            }

            if (!value.IsNullOrEmpty())
                tmp.Add(value);
        }

        result.DiaryDate = Timestamp.FromDateTime(GetRandomDate(adoptDate, GetAdoptDays(cloudCatData), 0.38f, 0.62f));
        result.DiaryId = tmp[Random.Range(0, tmp.Count)];

        result.DiaryId = GetDiaryTone(cloudCatData, result.DiaryId);

        return result;
    }

    //互動
    private CloudSave_DiaryData GetDiaryInteract(CloudCatData cloudCatData)
    {
        CloudSave_DiaryData result = new CloudSave_DiaryData();
        DateTime adoptDate = cloudCatData.CatDiaryData.AdoptTimestamp.ToDateTime();

        result.DiaryDate = Timestamp.FromDateTime(GetRandomDate(adoptDate, GetAdoptDays(cloudCatData), 0.63f, 0.72f));
        result.DiaryId = (400 + Random.Range(1, 6)).ToString();

        result.DiaryId = GetDiaryTone(cloudCatData, result.DiaryId);

        return result;
    }

    //紀錄
    private CloudSave_DiaryData GetDiaryRecord(CloudCatData cloudCatData)
    {
        CloudSave_DiaryData result = new CloudSave_DiaryData();
        DateTime adoptDate = cloudCatData.CatDiaryData.AdoptTimestamp.ToDateTime();

        int scoreBased = GetAdoptDays(cloudCatData) * 2 / 5;
        List<string> tmp = new List<string>();

        if (cloudCatData.CatDiaryData.DiarySatietyScore <= scoreBased)
            tmp.Add("501");
        if (cloudCatData.CatDiaryData.DiaryLitterScore <= scoreBased)
            tmp.Add("502");
        if (cloudCatData.CatDiaryData.DiaryMoistureScore <= scoreBased)
            tmp.Add("503");
        if (cloudCatData.CatDiaryData.DiaryFavourbilityScore <= scoreBased)
            tmp.Add("504");

        if (tmp.Count <= 0) return null;

        result.DiaryDate = Timestamp.FromDateTime(GetRandomDate(adoptDate, GetAdoptDays(cloudCatData), 0.73f, 0.83f));
        result.DiaryId = tmp[Random.Range(0, tmp.Count)];

        result.DiaryId = GetDiaryTone(cloudCatData, result.DiaryId);

        return result;
    }

    //生病
    private CloudSave_DiaryData GetDiarySick(CloudCatData cloudCatData)
    {
        if (cloudCatData.CatHealthData.SickId.IsNullOrEmpty())
            return null;
        
        CloudSave_DiaryData result = new CloudSave_DiaryData();
        DateTime adoptDate = cloudCatData.CatDiaryData.AdoptTimestamp.ToDateTime();

        result.DiaryDate = Timestamp.FromDateTime(GetRandomDate(adoptDate, GetAdoptDays(cloudCatData), 0.84f, 0.87f));
        result.DiaryId = (600 + Random.Range(1, 3)).ToString();

        result.DiaryId = GetDiaryTone(cloudCatData, result.DiaryId);

        return result;
    }

    //告別
    private CloudSave_DiaryData GetDiaryBye(CloudCatData cloudCatData)
    {
        CloudSave_DiaryData result = new CloudSave_DiaryData();
        int ageLevel = CatExtension.GetCatAgeLevel(cloudCatData.CatData.SurviveDays);
        result.DiaryDate = Timestamp.FromDateTime(cloudCatData.CatData.DeathTime.ToDateTime());

        if (ageLevel == 2)
            result.DiaryId = (703 + Random.Range(1, 4)).ToString();
        else
            result.DiaryId = (700 + Random.Range(1, 4)).ToString();

        result.DiaryId = GetDiaryTone(cloudCatData, result.DiaryId);

        return result;
    }
}