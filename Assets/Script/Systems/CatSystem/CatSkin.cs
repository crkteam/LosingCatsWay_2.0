using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CatSkin : MvcBehaviour
{
    public Vector2 startScale = Vector2.one;
    public bool isGUI;

    public SkeletonMecanim skeletonMecanim;
    public SkeletonGraphic skeletonGraphic;
    [ShowIf("isGUI")] public SkeletonDataAsset catDataAsset;
    private Vector2 catGuiPosition;

    [ShowIf("isGUI"), Title("KittyCat")] public SkeletonDataAsset kittyCatDataAsset;
    [ShowIf("isGUI"), SerializeField] private Vector2 kittyGuiPosition;

    #region Slot

    private string slot_beard = "Beard";
    private string slot_body = "Body";
    private string slot_earRight = "Ear_Left";
    private string slot_earLeft = "Ear_Right";
    private string slot_eyeLeft = "Eye_Left";
    private string slot_eyeRight = "Eye_Right";
    private string slot_leftFootBehind = "LeftFoot_Behind";
    private string slot_leftFootFront = "LeftFoot_Front";
    private string slot_mouthAndNose = "Mouth_And_Nose";
    private string slot_mouthMeat = "MouthMeat";
    private string slot_rightFootBehind = "RightFoot_Behind";
    private string slot_rightFootFront = "RightFoot_Front";
    private string slot_tail = "Tail";

    private string slot_pupilLeft = "Pupil_Left";
    private string slot_pupilRight = "Pupil_Right";

    private string slot_faceCold = "Face_Cold";
    private string slot_faceAngry = "Face_Angry";
    private string slot_faceCry = "Face_Cry";
    private string slot_faceLove = "Face_Love";
    private string slot_faceDocile = "Face_Docile";
    
    // private string slot_faceSinisterSmile = "Face_Sinister Smile";

    #endregion

    #region Key

    private string key_beard = "Beard_";
    private string key_body = "Body_";
    private string key_earLeft = "Ear_Right_";
    private string key_earRight = "Ear_Left_";
    private string key_eyeLeft = "Eye_Left_";
    private string key_eyeRight = "Eye_Right_";
    private string key_eyeLeftColor = "_Color_";
    private string key_eyeRightColor = "_Color_";
    private string key_leftFootBehind = "LeftFoot_Behind_";
    private string key_leftFootFront = "LeftFoot_Front_";
    private string key_mouthAndNose = "MouthAndNose_";
    private string key_mouthMeat = "MouthMeat_";
    private string key_rightFootBehind = "RightFoot_Behind_";
    private string key_rightFootFront = "RightFoot_Front_";
    private string key_tail = "Tail_";

    private string key_pupilLeft = "Pupil_Left";
    private string key_pupilRight = "Pupil_Right";

    private string key_faceAngry = "Face_Angry";
    private string key_faceCold = "Face_Cold";
    private string key_faceCry = "Face_Cry";
    private string key_faceLove = "Face_Love";
    
    // private string key_sinisterSmile = "Face_Sinister Smile";

    private string key_docile = "Face_Docile";

    #endregion

    #region Sick

    private string ice_Bag = "Ice Bag";
    private string sick_Expression_Eye = "Sick_Expression_Eye";
    private string sick_Expression_Flush = "Sick_Expression_Flush";
    private string thermometer = "thermometer";
    private String ringworm_1 = "Ringworm_1";
    private String ringworm_2 = "Ringworm_2";
    private String ringworm_3 = "Ringworm_3";

    #endregion

    public GameObject[] extraSkins;

    [SerializeField] private GameObject wormEffect;
    
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
    
    public void ChangeSkin(CloudCatData cloudCatData)
    {
        if (catGuiPosition == Vector2.zero)
            catGuiPosition = transform.localPosition;
        
        if (CatExtension.GetCatAgeLevel(cloudCatData.CatData.SurviveDays) != 0)
        {
            ChangeCatSkin(cloudCatData);
            
            if (string.IsNullOrEmpty(cloudCatData.CatHealthData.SickId))
                SetSkin(cloudCatData);
            else
                SetSkinSlotNull();
        }
        else
            ChangeKittySkin(cloudCatData);
        
        if (wormEffect != null)
            wormEffect.SetActive(cloudCatData.CatHealthData.IsBug);
    }

    public void ChangeSkin(CloudLosingCatData cloudLosingCatData)
    {
        if (catGuiPosition == Vector2.zero)
            catGuiPosition = transform.localPosition;

        CloudCatData cloudCatData = new CloudCatData();
        cloudCatData.CatData = cloudLosingCatData.CatData;
        cloudCatData.CatSkinData = cloudLosingCatData.CatSkinData;
        ChangeLosingCatSkin(cloudCatData);
    }

    private void ChangeLosingCatSkin(CloudCatData cloudCatData)
    {
        if (CatExtension.GetCatAgeLevel(cloudCatData.CatData.SurviveDays) != 0)
        {
            SetSkeletonDataAsset(false);
        
            var catData = cloudCatData.CatData;
            var catSkinData = cloudCatData.CatSkinData;
            Skeleton catSkeleton = skeletonGraphic.Skeleton;
            
            var variety = catData.Variety.Replace('_', '-');

            // ???????????????ID?????????
            if (variety.Contains("Siamese") && !variety.Contains("GT") && !variety.Contains("CT"))
            {
                var t = variety.Split('-');
                variety = t[1] + '-' + t[0];
            }
            
            catSkeleton.SetSkin("Normal_Cat/" + variety);
            SetSkinAttachment(catSkeleton, catSkinData);
            transform.localPosition = catGuiPosition;
            SetCatBodyScale(cloudCatData);
        }
        else
        {
            SetSkeletonDataAsset(true);
            transform.localPosition = kittyGuiPosition;
            SetKittyCatBodyScale();
        }
    }

    private void ChangeCatSkin(CloudCatData cloudCatData)
    {
        Skeleton catSkeleton = null;

        var catData = cloudCatData.CatData;
        var catSkinData = cloudCatData.CatSkinData;

        if (isGUI)
        {
            SetSkeletonDataAsset(false);
            catSkeleton = skeletonGraphic.Skeleton;
            transform.localPosition = catGuiPosition;
        }
        else
            catSkeleton = skeletonMecanim.Skeleton;

        var variety = catData.Variety.Replace('_', '-');

        // ???????????????ID?????????
        if (variety.Contains("Siamese") && !variety.Contains("GT") && !variety.Contains("CT"))
        {
            var t = variety.Split('-');
            variety = t[1] + '-' + t[0];
        }

        catSkeleton.SetSkin("Normal_Cat/" + variety);
        SetSkinAttachment(catSkeleton, catSkinData);

        SetSickSlotNull(true);
        if (!string.IsNullOrEmpty(cloudCatData.CatHealthData.SickId) && !cloudCatData.CatHealthData.IsBug)
            SetCatSick(cloudCatData, catSkeleton);
        
        SetCatBodyScale(cloudCatData);
    }

    private void SetSkinAttachment(Skeleton catSkeleton, CloudSave_CatSkinData catSkinData)
    {
        catSkeleton.SetAttachment(slot_beard, key_beard + (catSkinData.BeardIndex + 1));
        catSkeleton.SetAttachment(slot_body, key_body + (catSkinData.BodyIndex + 1));
        catSkeleton.SetAttachment(slot_earLeft, key_earLeft + (catSkinData.EarLeftIndex + 1));
        catSkeleton.SetAttachment(slot_earRight, key_earRight + (catSkinData.EarRightIndex + 1));
        catSkeleton.SetAttachment(slot_eyeLeft,
            key_eyeLeft + (catSkinData.EyeTypeIndex + 1) + key_eyeLeftColor + (catSkinData.LeftEyeColorIndex + 1));
        catSkeleton.SetAttachment(slot_eyeRight,
            key_eyeRight + (catSkinData.EyeTypeIndex + 1) + key_eyeRightColor + (catSkinData.RightEyeColorIndex + 1));
        catSkeleton.SetAttachment(slot_leftFootBehind, key_leftFootBehind + (catSkinData.LeftFootBehindsIndex + 1));
        catSkeleton.SetAttachment(slot_leftFootFront, key_leftFootFront + (catSkinData.LeftFootFrontsIndex + 1));
        catSkeleton.SetAttachment(slot_mouthAndNose, key_mouthAndNose + (catSkinData.MouthAndNosesIndex + 1));
        catSkeleton.SetAttachment(slot_mouthMeat, key_mouthMeat + (catSkinData.MouthMeatIndex + 1));
        catSkeleton.SetAttachment(slot_rightFootBehind, key_rightFootBehind + (catSkinData.RightFootBehindsIndex + 1));
        catSkeleton.SetAttachment(slot_rightFootFront, key_rightFootFront + (catSkinData.RightFootFrontsIndex + 1));
        catSkeleton.SetAttachment(slot_tail, key_tail + (catSkinData.TailIndex + 1));

        catSkeleton.SetAttachment(slot_pupilLeft, key_pupilLeft);
        catSkeleton.SetAttachment(slot_pupilRight, key_pupilRight);

        catSkeleton.SetAttachment(slot_faceCold, null);
        catSkeleton.SetAttachment(slot_faceAngry, null);
        catSkeleton.SetAttachment(slot_faceCry, null);
        // catSkeleton.SetAttachment(slot_faceSinisterSmile, null);
    }

    private void CloseEye()
    {
        Skeleton catSkeleton = GetCatSkeleton();
        
        catSkeleton.SetAttachment(slot_eyeLeft, null);
        catSkeleton.SetAttachment(slot_eyeRight, null);
        catSkeleton.SetAttachment(slot_pupilLeft, null);
        catSkeleton.SetAttachment(slot_pupilRight, null);
    }

    public void OpenEye(CloudCatData cloudCatData)
    {
        Skeleton catSkeleton = GetCatSkeleton();

        if (CatExtension.GetCatAgeLevel(cloudCatData.CatData.SurviveDays) == 0)
        {
            catSkeleton.SetAttachment(slot_eyeLeft, slot_eyeLeft);
            catSkeleton.SetAttachment(slot_eyeRight, slot_eyeRight);
            catSkeleton.SetAttachment(slot_pupilLeft, key_pupilLeft);
            catSkeleton.SetAttachment(slot_pupilRight, key_pupilRight);
            return;
        }
        
        //???
        catSkeleton.SetAttachment(slot_eyeLeft,
            key_eyeLeft + (cloudCatData.CatSkinData.EyeTypeIndex + 1) + key_eyeLeftColor + (cloudCatData.CatSkinData.LeftEyeColorIndex + 1));
        catSkeleton.SetAttachment(slot_eyeRight,
            key_eyeRight + (cloudCatData.CatSkinData.EyeTypeIndex + 1) + key_eyeRightColor + (cloudCatData.CatSkinData.RightEyeColorIndex + 1));
        
        //??????
        catSkeleton.SetAttachment(slot_pupilLeft, key_pupilLeft);
        catSkeleton.SetAttachment(slot_pupilRight, key_pupilRight);
    }

    public void CloseSickEye()
    {
        Skeleton catSkeleton = GetCatSkeleton();
        catSkeleton.SetAttachment(sick_Expression_Eye, null);
    }

    private void SetSickSlotNull(bool isAdult)
    {
        CloseSickEye();
        
        if (!isAdult)
            return;
        
        Skeleton catSkeleton = GetCatSkeleton();
        
        catSkeleton.SetAttachment(thermometer, null);
        catSkeleton.SetAttachment(ice_Bag, null);
        
        catSkeleton.SetAttachment(ringworm_1, null);
        catSkeleton.SetAttachment(ringworm_2, null);
        catSkeleton.SetAttachment(ringworm_3, null);
    }
    
    private void SetCatSick(CloudCatData cloudCatData, Skeleton catSkeleton)
    {
        CloseEye();
        
        if (cloudCatData.CatHealthData.IsBug)
        {
            catSkeleton.SetAttachment(sick_Expression_Eye, sick_Expression_Eye);
            catSkeleton.SetAttachment(sick_Expression_Flush, sick_Expression_Flush);
        }
        
        if (string.IsNullOrEmpty(cloudCatData.CatHealthData.SickId))
            return;
        
        var sickId = cloudCatData.CatHealthData.SickId;
        var sickLevel = App.factory.sickFactory.GetSickLevel(sickId);

        if (sickLevel >= 0 || sickLevel == -1)
        {
            catSkeleton.SetAttachment(sick_Expression_Eye, sick_Expression_Eye);
            catSkeleton.SetAttachment(sick_Expression_Flush, sick_Expression_Flush);
        }

        if (sickLevel >= 1 || sickLevel == -1)
            catSkeleton.SetAttachment(thermometer, thermometer);

        if (sickLevel >= 2 || sickLevel == -1)
            catSkeleton.SetAttachment(ice_Bag, ice_Bag);

        switch (sickId)
        {
            case "SK011":
                catSkeleton.SetAttachment(ringworm_1, ringworm_1);
                break;
            case "SK021":
                catSkeleton.SetAttachment(ringworm_2, ringworm_2);
                break;
            case "SK022":
                catSkeleton.SetAttachment(ringworm_3, ringworm_3);
                break;
        }
    }

    private void ChangeKittySkin(CloudCatData cloudCatData)
    {
        if (isGUI)
        {
            SetSkeletonDataAsset(true);
            transform.localPosition = kittyGuiPosition;
        }
        
        SetSickSlotNull(false);
        
        Skeleton catSkeleton = null;
        if (isGUI)
            catSkeleton = skeletonGraphic.Skeleton;
        else
            catSkeleton = skeletonMecanim.Skeleton;
        
        var catSickId = cloudCatData.CatHealthData.SickId;
        if (!String.IsNullOrEmpty(catSickId) || cloudCatData.CatHealthData.IsBug)
        {
            CloseEye();
            
            catSkeleton.SetAttachment(sick_Expression_Eye, sick_Expression_Eye);
            catSkeleton.SetAttachment(sick_Expression_Flush, sick_Expression_Flush);
            
            // if (catSickId == "SK011")
            //     catSkeleton.SetAttachment(ringworm_1, ringworm_1);
        }
        else
            OpenEye(cloudCatData);

        SetKittyCatBodyScale();
    }

    private void SetCatBodyScale(CloudCatData cloudCatData)
    {
        var newBodyScale = startScale * cloudCatData.CatData.BodyScale;
        transform.localScale = newBodyScale;

        if (!isGUI)
            GetComponent<DirectionChecker>().SetOriginalScaleX(newBodyScale.x);
    }

    private void SetKittyCatBodyScale()
    {
        var newBodyScale = startScale * 0.5f;
        transform.localScale = newBodyScale;

        if (!isGUI)
            GetComponent<DirectionChecker>().SetOriginalScaleX(newBodyScale.x);
    }

    private void SetSkeletonDataAsset(bool isKitty)
    {
        if (!isKitty)
        {
            skeletonGraphic.initialSkinName = "Normal_Cat/Benz";

            skeletonGraphic.skeletonDataAsset = catDataAsset;
            skeletonGraphic.Initialize(true);
            skeletonGraphic.Skeleton.SetSkin("Normal_Cat/Benz");
            
            if (skeletonGraphic.AnimationState.GetCurrent(0) == null)
                skeletonGraphic.AnimationState.SetAnimation(0, "AI_Main/IDLE_Ordinary01", true);
        }
        else
        {
            skeletonGraphic.initialSkinName = "Ordinary";

            skeletonGraphic.skeletonDataAsset = kittyCatDataAsset;
            skeletonGraphic.Initialize(true);
            skeletonGraphic.Skeleton.SetSkin("Ordinary");
            
            if (skeletonGraphic.AnimationState.GetCurrent(0) == null)
                skeletonGraphic.AnimationState.SetAnimation(0, "AI_Main/IDLE_Ordinary01", true);
        }
    }

    #region Status

    public void SetAngry()
    {
        Skeleton catSkeleton = null;

        if (isGUI)
            catSkeleton = skeletonGraphic.Skeleton;
        else
            catSkeleton = skeletonMecanim.Skeleton;

        CloseFace();
        catSkeleton.SetAttachment(slot_faceAngry, key_faceAngry);
    }
    
    public void SetCold()
    {
        Skeleton catSkeleton = null;
        
        if (isGUI)
            catSkeleton = skeletonGraphic.Skeleton;
        else
            catSkeleton = skeletonMecanim.Skeleton;
        
        CloseFace();
        catSkeleton.SetAttachment(slot_faceCold, key_faceCold);
    }

    public void SetCry()
    {
        Skeleton catSkeleton = null;
        
        if (isGUI)
            catSkeleton = skeletonGraphic.Skeleton;
        else
            catSkeleton = skeletonMecanim.Skeleton;
        
        CloseFace();
        catSkeleton.SetAttachment(slot_faceCry, key_faceCry);
    }
    
    public void SetSinisterSmile()
    {
        Skeleton catSkeleton = null;
        
        if (isGUI)
            catSkeleton = skeletonGraphic.Skeleton;
        else
            catSkeleton = skeletonMecanim.Skeleton;
        
        CloseFace();
        // catSkeleton.SetAttachment(slot_faceSinisterSmile, key_sinisterSmile);
    }

    public void SetLove()
    {
        Skeleton catSkeleton = null;
        
        if (isGUI)
            catSkeleton = skeletonGraphic.Skeleton;
        else
            catSkeleton = skeletonMecanim.Skeleton;
        
        CloseFace();
        catSkeleton.SetAttachment(slot_faceLove, key_faceLove);
    }

    public void SetDocile()
    {
        Skeleton catSkeleton = null;
        
        if (isGUI)
            catSkeleton = skeletonGraphic.Skeleton;
        else
            catSkeleton = skeletonMecanim.Skeleton;
        
        CloseFace();
        catSkeleton.SetAttachment(slot_faceDocile, key_docile);
    }

    public void CloseFace()
    {
        Skeleton catSkeleton = GetCatSkeleton();
        
        catSkeleton.SetAttachment(slot_eyeLeft, null);
        catSkeleton.SetAttachment(slot_eyeRight, null);
        catSkeleton.SetAttachment(slot_mouthAndNose, null);
        catSkeleton.SetAttachment(slot_mouthMeat, null);
        catSkeleton.SetAttachment(slot_pupilLeft, null);
        catSkeleton.SetAttachment(slot_pupilRight, null);
    }

    #endregion

    #region Skin

    public void SetSkin(CloudCatData cloudCatData)
    {
        var useSkinId = cloudCatData.CatSkinData.UseSkinId;

        SetSkinSlotNull();
        
        if (String.IsNullOrEmpty(useSkinId))
        {
            if (!isGUI)
                for (int i = 0; i < extraSkins.Length; i++)
                    extraSkins[i].SetActive(false);
            
            return;
        }
        
        Skeleton catSkeleton = null;
        
        if (isGUI)
            catSkeleton = skeletonGraphic.Skeleton;
        else
            catSkeleton = skeletonMecanim.Skeleton;

        // ????????????
        switch (useSkinId)
        {
            case "Pumpkin":
                catSkeleton.SetAttachment("EA_Pumpkin", "EA_Pumpkin");
                catSkeleton.SetAttachment("EA_Pumpkin_z1", "EA_Pumpkin_z1");
                catSkeleton.SetAttachment("EA_Pumpkin_z2", "EA_Pumpkin_z2");
                catSkeleton.SetAttachment("EA_Pumpkin_z3", "EA_Pumpkin_z3");
                break;
            case "Spell":
                catSkeleton.SetAttachment("EA_Spell", "EA_Spell");
                break;
            case "Magic_Hat":
                catSkeleton.SetAttachment("EA_Magic Hat", "EA_Magic Hat");
                catSkeleton.SetAttachment("EA_Magic Hat_Behind", "EA_Magic Hat_Behind");
                catSkeleton.SetAttachment("EA_Magic Hat_Chain", "EA_Magic Hat_Chain");
                break;
            case "SantaHat":
                catSkeleton.SetAttachment("SantaHat", "SantaHat");
                break;
            case "Flyfish":
                catSkeleton.SetAttachment("Flyfish", "Flyfish");
                break;
            case "Robot":
                catSkeleton.SetAttachment("Robot_Card", "Robot_Card");
                catSkeleton.SetAttachment("Robot_Wire", "Robot_Wire");
                break;
        }
        
        // ???????????????
        
        if (isGUI)
            return;
        
        for (int i = 0; i < extraSkins.Length; i++)
            extraSkins[i].SetActive(false);
        
        switch (useSkinId)
        {
            case "Little_Yumi":
                extraSkins[0].SetActive(true);
                break;
            case "Magic_Hat":
                extraSkins[1].SetActive(true);
                break;
            case "Ghost_Black":
                extraSkins[2].SetActive(true);
                break;
            case "Ghost_White":
                extraSkins[3].SetActive(true);
                break;
        }
    }

    private void SetSkinSlotNull()
    {
        Skeleton catSkeleton = GetCatSkeleton();
        
        //Pumpkin
        catSkeleton.SetAttachment("EA_Pumpkin", null);
        catSkeleton.SetAttachment("EA_Pumpkin_z1", null);
        catSkeleton.SetAttachment("EA_Pumpkin_z2", null);
        catSkeleton.SetAttachment("EA_Pumpkin_z3", null);
        
        //Spell
        catSkeleton.SetAttachment("EA_Spell", null);
        
        //Magic_Hat
        catSkeleton.SetAttachment("EA_Magic Hat", null);
        catSkeleton.SetAttachment("EA_Magic Hat_Behind", null);
        catSkeleton.SetAttachment("EA_Magic Hat_Chain", null);
        
        //SantaHat
        catSkeleton.SetAttachment("SantaHat", null);
        
        //Flyfish
        catSkeleton.SetAttachment("Flyfish", null);
        
        //Robot
        catSkeleton.SetAttachment("Robot_Card", null);
        catSkeleton.SetAttachment("Robot_Wire", null);
    }

    #endregion

    private Skeleton GetCatSkeleton()
    {
        Skeleton catSkeleton = null;
        
        if (isGUI)
            catSkeleton = skeletonGraphic.Skeleton;
        else
            catSkeleton = skeletonMecanim.Skeleton;
        
        return catSkeleton;
    }
}