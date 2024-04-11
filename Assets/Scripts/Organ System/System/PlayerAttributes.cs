using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attributes", menuName = "ScriptableObjects/PlayerAttributes", order = 1)]
public class PlayerAttributes : ScriptableObject
{
    [Header("Attributes")]
    public float HP = 1;
    public float Speed = 15; 
    public float Stamina = 20;
    public float Melee_Attack = 5;
    public float Essense_Capacity = 100;
    public float Essense_Potency = 1;
    public float Reload_Speed = 1;
    public float Accuracy = 0;
    public float Physical_Resistance = 0;
    public float Chemical_Resistance = 0;
    public float Health_Regeneration = 0;


    [Header("Unique Powers")]
    public bool Sight;
    public bool Sprint;
    public bool Double_Jump;
    public bool Wall_Run;
    public bool Essense_Tar;
    public bool Essense_Leech;
    public bool Essense_Spike;
    public bool Essense_Haste;
    public bool Essense_Bloat;

}
