using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganTile : MonoBehaviour
{

    private Organ organ;
    private UIManager manager;
    public BodyPartTile CurrentTile;

    private void Awake()
    {
        organ = transform.parent.GetComponent<Organ>();
        manager =  transform.parent.parent.GetComponentInChildren<UIManager>();
        organ.TileList.Add(this);
    }

    private void Update()
    {
        //while the player is dragging an organ make all tiles available for placement 
        if(organ != null && manager.holdingSomething && manager.ManageThis == organ.GetComponent<DraggableUI>()) { LookForTiles(); print("looking for organs"); }
    }
    

    public void LookForTiles()
    {
        //check if overlapping a bodypart tile that this tile could be placed on top of
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.forward * 20);
        Debug.DrawRay(transform.position, Vector3.forward * 20);
        // If it hits something...
        if (hit.collider != null && hit.transform.GetComponent<BodyPartTile>() && (hit.transform.GetComponent<BodyPartTile>().OccupyingTile == null || hit.transform.GetComponent<BodyPartTile>().OccupyingTile == this))
        {
            //If there is a body part or inventory tile below , lock it in and keep it until next check
            CurrentTile = hit.transform.GetComponent<BodyPartTile>();

        }
        else
        {
            //no tile , if organ is placed when one of its tiles is empty it will go to its previous location
            CurrentTile = null;

            print("I tried :C");
        }


    }



}
