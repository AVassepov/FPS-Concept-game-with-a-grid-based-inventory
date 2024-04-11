using System.Collections.Generic;
using UnityEngine;





[CreateAssetMenu(fileName = "Organ", menuName = "ScriptableObjects/Organ Pool", order = 2)]
public class OrganPool : ScriptableObject
{
  public List<GameObject> CommonDrops;
    public List<GameObject> RareDrops;
    public List<GameObject> LegendaryDrops;






}
