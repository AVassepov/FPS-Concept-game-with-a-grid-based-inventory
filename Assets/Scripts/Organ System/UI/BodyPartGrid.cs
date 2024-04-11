using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BodyPartGrid : MonoBehaviour 
{


    public Vector2 Dimensions;
    [SerializeField] private Canvas Canvas;

[SerializeField] private GameObject OrganTilePrefab;
    public List<Organ> OrgansOnTiles;
    public bodyPart BodyPart;
    public enum bodyPart
    {
        Head = 1,
        Eyes = 2,
        Torso = 3,
        LeftArm = 4,
        RightArm = 5,
        Legs = 6

    }

    // Start is called before the first frame update

    void Start()
    {
        MakeGrid();
    }



    private void MakeGrid()
    {
        // make  a grid of tiles on start
        for (int i = 0; i < Dimensions.x; i++)
        {
            for (int j = 0; j < Dimensions.y; j++)
            {
                GameObject temp = Instantiate(OrganTilePrefab , transform);
                temp.transform.localPosition = new Vector3(i*25, j*25, 0);
            }
        }



    }


    public virtual bool CheckBodyPartMatch(Organ checkThis)
    {
        //check if the organ placed is matching with the body part , IE: head organs can only be placed in head
        if((int) checkThis.Data.Slot == (int)BodyPart || (int)checkThis.Data.Slot  == 0 || ((int)checkThis.Data.Slot == 4 && (int)BodyPart == 5) )
        {
            return true;
        }

        return false;
    }


}
