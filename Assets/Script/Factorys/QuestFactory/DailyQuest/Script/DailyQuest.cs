using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyQuest : Quest
{
    public override int TargetCount
    {
        get
        {
            return targetCountBasedOnLevel[LevelIndex];
        }
    }

    public override Reward[] Rewards
    {
        get
        {
            return rewardsBasedOnLevel[LevelIndex];
        }
    }

    public int LevelIndex
    {
        get
        {
            int level = App.system.player.Level;

            if (level <= 9)
                return 0;

            if (level <= 39)
                return 1;

            if (level <= 69)
                return 2;

            if (level <= 84)
                return 3;

            return 4;
        }
    }
}