using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Array2DEditor;
[CreateAssetMenu(fileName = "Organ", menuName = "ScriptableObjects/Organs", order = 1)]
public class OrganScriptableObject : ScriptableObject
{

    public Array2DBool Dimensions;

    public string Name;
    public string Description;
    public Rarity Quality;
    public slot Slot;
    public int Freshness;
    public Sprite OrganSprite;
    public List<Effect> Effects;
    public List<float> EffectValues;
    public GameObject UIGameobject;
    public List<UniqueEffect> UniqueEffects;
    public enum Rarity
    {
        Rotten,
        Stale,
        Fresh,
    }





    public enum Effect
    {
        HP = 0,
        Speed = 1, 
        Stamina = 2,
        Melee_Attack = 3,
        Essense_Capacity = 4,
        Essense_Potency = 5,
        Reload_Speed = 6,
        Accuracy = 7,
        Physical_Resistance = 8,
        Chemical_Resistance = 9,
        Health_Regeneration = 10
    }
    public enum UniqueEffect
    {
        Sight = 0,
        Sprint =1 ,
        Double_Jump = 2,
        Wall_Run = 3,
        Essense_Tar = 4,
        Essense_Leech = 5,
        Essense_Spike = 6,
        Essense_Haste = 7,
        Essense_Bloat = 8
    }
    public enum slot{
        Any = 0,
        Head = 1,
        Eyes = 2,
        Torso = 3,
        Arm = 4,
        Legs = 6
            //skip 5 due to arm shinanigans

    }



}
