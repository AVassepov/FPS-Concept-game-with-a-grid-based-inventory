using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OrganPopUp : MonoBehaviour
{


    public OrganScriptableObject OrganInfo;

    private TextMeshProUGUI Title;
    private TextMeshProUGUI Bonuses;
    private TextMeshProUGUI AdditionalInfo;
    private Image Image;


    public OrganInstance Parent;

    private void Awake()
    {//get components
        Title =transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        Bonuses = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        AdditionalInfo = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        Image = transform.GetChild(4).GetComponent<Image>();
    }

    private void Start()
    {
        //get default info
        Title.text = OrganInfo.name;
        Bonuses.text = OrganInfo.Description;
        AdditionalInfo.text = "Part: " + OrganInfo.Slot.ToString() + " \n" + OrganInfo.Quality.ToString();
        Image.sprite = OrganInfo.OrganSprite;
        UpdateInfo();
    }

    private void Update()
    {
        if(Parent == null) { Destroy(gameObject); }
    }

    public void UpdateInfo()
    {
        //when hovering over organ put it's info on the info UI element
        for (int i = 0; i < OrganInfo.Effects.Count; i++)
        {
            string effectName = OrganInfo.Effects[i].ToString();
            effectName = effectName.Replace("_", " ");
            Bonuses.text += OrganInfo.EffectValues[i] + " " + effectName + Environment.NewLine;
        }
        for (int i = 0; i < OrganInfo.UniqueEffects.Count; i++)
        {
            string effectName = OrganInfo.UniqueEffects[i].ToString();
            effectName = effectName.Replace("_", " ");
            Bonuses.text += effectName + Environment.NewLine;
        }

    }

}
