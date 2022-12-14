using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class PlayerSystem : MvcBehaviour
{
    #region InspectorVariable

    public PlayerDataSetting playerDataSetting;

    #endregion

    #region Callback

    public delegate void ValueChange(object value);
    public delegate void ValueChangeFromTo(object from, object to);

    public ValueChange OnPlayerIdChange;
    public ValueChange OnPlayerNameChange;
    public ValueChange OnLevelChange;
    public ValueChangeFromTo OnExpChange;
    
    public ValueChange OnCoinChange;
    public ValueChange OnAddCoinChange;
    public ValueChange OnReduceCoinChange;
    
    public ValueChange OnDiamondChange;
    public ValueChange OnAddDiamondChange;
    public ValueChange OnReduceDiamondChange;

    public ValueChange OnCatMemoryChange;
    public ValueChange OnAddCatMemoryChange;
    public ValueChange OnReduceCatMemoryChange;
    
    public ValueChange OnCatSlotChange;
    public ValueChange OnPlayerGenderChange;
    public ValueChange OnFriendIdsChange;
    public ValueChange OnReceiveFriendInvitesChange;

    public ValueChange OnUsingIconChange;
    public ValueChange OnUsingAvatarChange;

    public ValueChange OnCatDeadCountChange;

    #endregion

    #region Variable

    private string playerId;
    private string playerName;
    
    private int level;
    private int exp;
    
    private int diamond;
    private int coin;
    private int catMemory;
    
    private int diamondCatSlot;
    private int gridSizeLevel;
    
    private int playerGender = -1; //0:Male 1:Female
    
    private string usingIcon;
    private string usingAvatar;
    
    private int catDeadCount;

    #endregion

    #region Properties

    public string PlayerId
    {
        get => playerId;
        set
        {
            playerId = value;
            OnPlayerIdChange?.Invoke(value);
        }
    }

    public string PlayerName
    {
        get => playerName;
        set
        {
            playerName = value;
            OnPlayerNameChange?.Invoke(value);
        }
    }

    public int Level
    {
        get => level;
        set
        {
            if (value - level == 1 && level != 0)
                App.system.levelUp.Open();
            
            level = value;
            OnLevelChange?.Invoke(value);
        }
    }

    public int Exp
    {
        get => exp;
        set
        {
            float from = exp;
            float to = value;
            OnExpChange?.Invoke(from, to);
            exp = value;
        }
    }

    public int Coin
    {
        get => coin;
        set
        {
            coin = value;
            OnCoinChange?.Invoke(value);
        }
    }

    public int Diamond
    {
        get => diamond;
        set
        {
            diamond = value;
            OnDiamondChange?.Invoke(value);
        }
    }

    public int CatMemory
    {
        get => catMemory;
        set
        {
            catMemory = value;
            OnCatMemoryChange?.Invoke(value);
        }
    }

    public int CatSlot
    {
        get
        {
            int levelCatSlot = playerDataSetting.GetCatSlotByLevel(Level);
            return levelCatSlot + diamondCatSlot;
        }
    }

    public int DiamondCatSlot
    {
        get => diamondCatSlot;
        set => diamondCatSlot = value;
        //Todo ValueChange
    }

    public int GridSizeLevel
    {
        get => gridSizeLevel;
        set => gridSizeLevel = value;
    }

    public int PlayerGender
    {
        get => playerGender;
        set
        {
            playerGender = value;
            OnPlayerGenderChange?.Invoke(value); //TODO Icon
        }
    }

    public string UsingIcon
    {
        get => usingIcon;
        set
        {
            usingIcon = value;
            if (string.IsNullOrEmpty(value))
                return;
            OnUsingIconChange?.Invoke(value);
        }
    }

    public string UsingAvatar
    {
        get => usingAvatar;
        set
        {
            usingAvatar = value;
            if (string.IsNullOrEmpty(value))
                return;
            OnUsingAvatarChange?.Invoke(value);
        }
    }

    public int CatDeadCount
    {
        get => catDeadCount;
        set
        {
            catDeadCount = value;
            OnCatDeadCountChange?.Invoke(value);
        }
    }

    #region Read-Only

    // ????????????
    public int CanAdoptCatCount
    {
        get
        {
            int count = App.system.room.FeatureRoomsCount;
            int slot = CatSlot;
            int cats = App.system.cat.GetCats().Count;

            count -= cats;
            slot -= cats;
            
            int result = 0;
            for (int i = 0; i < (count + slot); i++)
            {
                if (count <= 0) break;
                if (slot <= 0) break;
                result++;
                count--;
                slot--;
            }
            
            return result;
        }
    }

    // ??????????????????
    public int NextLevelExp => playerDataSetting.GetNextLevelUpExp(Level);

    #endregion

    #endregion

    #region Method

    public void AddExp(int value)
    {
        int result = Exp + value;
        int nextExp = playerDataSetting.GetNextLevelUpExp(Level);

        // ?????? ???????????????
        if (result >= nextExp)
        {
            int end = result - nextExp;
            Exp = nextExp;
            Level++;
            Exp = 0;
            Exp = end;
        }
        else
        {
            Exp = result;
        }
    }

    public void AddMoney(int value)
    {
        coin += value;
        OnAddCoinChange?.Invoke(value);
    }
    
    public bool ReduceMoney(int value)
    {
        if (coin - value < 0)
            return false;
        
        coin -= value;
        OnReduceCoinChange?.Invoke(value);
        return true;
    }

    public void AddDiamond(int value)
    {
        diamond += value;
        OnAddDiamondChange?.Invoke(value);
    }

    public bool ReduceDiamond(int value)
    {
        if (diamond - value < 0)
            return false;

        diamond -= value;
        OnReduceDiamondChange?.Invoke(value);
        return true;
    }

    public void AddCatMemory(int value)
    {
        catMemory += value;
        OnAddCatMemoryChange?.Invoke(value);
    }

    public bool ReduceCatMemory(int value)
    {
        if (catMemory - value < 0)
            return false;

        catMemory -= value;
        OnReduceCatMemoryChange?.Invoke(value);
        return true;
    }

    #endregion
}
