using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Item_", menuName = "Factory/Create Item")]
public class Item : ScriptableObject
{
    public string id;
    [Space(10)] [EnumPaging] public ItemType itemType;
    [EnumPaging] public ItemBoughtType itemBoughtType;

    #region Feed

    [ShowIf("itemType", ItemType.Feed)] [BoxGroup("Feed")] [EnumPaging]
    public ItemFeedType itemFeedType;

    [ShowIf("itemType", ItemType.Feed)] [BoxGroup("Feed")] [EnumPaging]
    public FoodType foodType;

    [ShowIf("itemFeedType", ItemFeedType.Snack)] [BoxGroup("Feed")] [EnumPaging]
    public SnackType snackType;

    [Title("LikeValue")]
    [ShowIf("itemType", ItemType.Feed)] [BoxGroup("Feed")] public int likeSatiety;
    [ShowIf("itemType", ItemType.Feed)] [BoxGroup("Feed")] public int likeMoisture;
    [ShowIf("itemType", ItemType.Feed)] [BoxGroup("Feed")] public int likeFun;

    [Title("NormalValue")]
    [ShowIf("itemType", ItemType.Feed)] [BoxGroup("Feed")] public int normalSatiety;
    [ShowIf("itemType", ItemType.Feed)] [BoxGroup("Feed")] public int normalMoisture;
    [ShowIf("itemType", ItemType.Feed)] [BoxGroup("Feed")] public int normalFun;
    
    [Title("HateValue")]
    [ShowIf("itemType", ItemType.Feed)] [BoxGroup("Feed")] public int hateSatiety;
    [ShowIf("itemType", ItemType.Feed)] [BoxGroup("Feed")] public int hateMoisture;
    [ShowIf("itemType", ItemType.Feed)] [BoxGroup("Feed")] public int hateFun;

    #endregion

    #region Tool

    [ShowIf("itemType", ItemType.Tool)] [EnumPaging]
    public ItemToolType itemToolType = ItemToolType.Normal;

    #endregion

    #region Litter

    [ShowIf("itemType", ItemType.Litter)] [EnumPaging]
    public ItemLitterType itemLitterType;

    #endregion

    [Space(10)] public int price;

    [InlineEditor(InlineEditorModes.GUIAndPreview)] public Sprite icon;
    [InlineEditor(InlineEditorModes.GUIAndPreview)] public Sprite content;

    public bool canUse;
    public bool notShowAtStore;
    public bool notShowAtBag;
    
    #region Properties

    public int Count
    {
        get
        {
            MyApplication app = FindObjectOfType<MyApplication>();

            int value = 0;

            switch (itemType)
            {
                case ItemType.Feed:
                    value = app.system.inventory.FoodData[id];
                    break;
                case ItemType.Tool:
                    value = app.system.inventory.ToolData[id];
                    break;
                case ItemType.Litter:
                    value = app.system.inventory.LitterData[id];
                    break;
                case ItemType.Room:
                    value = app.system.inventory.RoomData[id];
                    break;
                case ItemType.Play:
                    value = 1;
                    break;
                case ItemType.CatSkin:
                    value = app.system.inventory.SkinData[id];
                    break;
                default:
                    break;
            }

            return value;
        }
        set
        {
            MyApplication app = FindObjectOfType<MyApplication>();
            int fromValue = 0;

            switch (itemType)
            {
                case ItemType.Feed:
                    fromValue = app.system.inventory.FoodData[id];
                    app.system.inventory.FoodData[id] = value;
                    break;
                case ItemType.Tool:
                    fromValue = app.system.inventory.ToolData[id];
                    app.system.inventory.ToolData[id] = value;
                    break;
                case ItemType.Litter:
                    fromValue = app.system.inventory.LitterData[id];
                    app.system.inventory.LitterData[id] = value;
                    break;
                case ItemType.Room:
                    fromValue = app.system.inventory.RoomData[id];
                    app.system.inventory.RoomData[id] = value;
                    break;
                case ItemType.CatSkin:
                    fromValue = app.system.inventory.SkinData[id];
                    app.system.inventory.SkinData[id] = value;
                    break;
                default:
                    break;
            }

            if (fromValue > 0) return;
            if (value > fromValue)
                CanBuyAtStore = true;
        }
    }

    public string Name
    {
        get
        {
            MyApplication app = FindObjectOfType<MyApplication>();
            return app.factory.stringFactory.GetItemName(id);
        }
    }

    public string Description
    {
        get
        {
            MyApplication app = FindObjectOfType<MyApplication>();
            return app.factory.stringFactory.GetItemDescription(id);
        }
    }

    // 吃的
    public bool ForSatiety
    {
        get => (likeSatiety > 0);
    }

    // 喝的
    public bool ForMoisture
    {
        get => (likeMoisture > 0);
    }

    // 心情的
    public bool ForFun
    {
        get => (likeFun > 0);
    }

    public bool CanBuyAtStore
    {
        get
        {
            MyApplication app = FindObjectOfType<MyApplication>();
            return app.system.inventory.itemsCanBuyAtStore[id];
        }
        set
        {
            MyApplication app = FindObjectOfType<MyApplication>();
            app.system.inventory.itemsCanBuyAtStore[id] = value;
        }
    }

    #endregion
}