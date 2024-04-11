using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public static class OrganSpawner 
{
  

    public static List<GameObject> ChooseRarity(float commonChance , float rareChance, float legendaryChance , OrganPool pool)
    {

        float total = commonChance + rareChance + legendaryChance;

        Debug.Log("Total = " +total.ToString());
        float roll = Random.Range(0, total);

        Debug.Log("Rolled a  = " + roll.ToString());

        if (roll <= commonChance) { Debug.Log("Got a common" );  return pool.CommonDrops;  }
        else if (roll <= commonChance+ rareChance) { Debug.Log("Got a Rare"); return pool.RareDrops; }
        else { Debug.Log("Got a legendary");   return pool.LegendaryDrops; }
    }

    public static GameObject ReturnOrgan(List<GameObject> PossibleDrops)
    {
        return PossibleDrops[Random.Range(0, PossibleDrops.Count)];  
    }



}
