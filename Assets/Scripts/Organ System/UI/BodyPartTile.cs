using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyPartTile : MonoBehaviour
{
   public OrganTile OccupyingTile;
    [SerializeField] private Image spritRenderer;


    private void Update()
    {

        //If there is an organ tile above it make it red , otherwise keep it white  , in the future might make it a method that is called when organs are placed
        //to make it a little bit more optimal , but as it stands its fine to use update for this
        if(OccupyingTile == null)
        {
            spritRenderer.color = Color.white;
        }
        else
        {
            spritRenderer.color = Color.red;
        }
    }
}
