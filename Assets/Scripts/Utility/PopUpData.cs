using TMPro;
using UnityEngine;
using System;
public class PopUpData : MonoBehaviour
{

    [Header("Scriptable Objects")]
    public OrganScriptableObject Data;
    public PlayerAttributes PlayerData;



    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Description;
    [SerializeField] private GameObject FreshnessIcon;
    [SerializeField] private TextMeshProUGUI Bonuses;
    [SerializeField] private TextMeshProUGUI Freshness;


    [SerializeField] private TextMeshProUGUI Attributes1;
    [SerializeField] private TextMeshProUGUI Attributes2;

    private void Start()
    {
        ClearInfo();
        ShowAttributes();
    }


    public void UpdateInfo()
    {

        FreshnessIcon.active = true;
        Name.text = Data.name;
        Description.text = Data.Description;

        Bonuses.text = "";
        for (int i = 0; i < Data.Effects.Count; i++)
        {
            string effectName = Data.Effects[i].ToString();
            effectName= effectName.Replace("_"," ");
            Bonuses.text += Data.EffectValues[i] + " "+ effectName + Environment.NewLine;
        }
        for (int i = 0; i < Data.UniqueEffects.Count; i++)
        {
            string effectName = Data.UniqueEffects[i].ToString();
            effectName = effectName.Replace("_", " ");
            Bonuses.text += effectName + Environment.NewLine;
        }

        Freshness.text = Data.Freshness.ToString(); 
        Freshness.text += "%";


        Attributes1.text = "";
        Attributes2.text = "";

    }

    public void ClearInfo()
    {
        FreshnessIcon.active = false;
        Name.text = "";
        Description.text = "";
        Bonuses.text = "";
        Freshness.text ="";

        Attributes1.text = "";
        Attributes2.text = "";
    }

    public void ShowAttributes()
    {
        ClearInfo();

        Attributes1.text += PlayerData.HP.ToString() + " Max HP" + Environment.NewLine;
        Attributes1.text += PlayerData.Stamina.ToString() + " Max Stamina" + Environment.NewLine;
        Attributes1.text += PlayerData.Essense_Capacity.ToString() + " Max Essense" + Environment.NewLine;
        Attributes1.text += PlayerData.Essense_Potency.ToString() + " Essense Potency" + Environment.NewLine;
        Attributes1.text += PlayerData.Melee_Attack.ToString() + " Melee Strength" + Environment.NewLine;
        Attributes2.text += "+" + PlayerData.Health_Regeneration.ToString() + " HP/S" + Environment.NewLine;

        Attributes2.text += PlayerData.Speed.ToString() + " Max Speed" + Environment.NewLine;
        Attributes2.text += PlayerData.Reload_Speed.ToString() + " Reload Speed" + Environment.NewLine;
        Attributes2.text += PlayerData.Accuracy.ToString() + " Accuracy" + Environment.NewLine;
        Attributes2.text += PlayerData.Chemical_Resistance.ToString() + " Chem. Resistance" + Environment.NewLine;
        Attributes2.text += PlayerData.Physical_Resistance.ToString() + " Phys. Resistance" + Environment.NewLine;

    }
}
