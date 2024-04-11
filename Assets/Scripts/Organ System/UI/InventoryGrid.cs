using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGrid : BodyPartGrid
{

    public List<GameObject> OrgansToSpawn;
    [SerializeField]private PlayerAttributes Attributes;
    private Organ CurrentOrgan;


    private float xCounter;
    private float yCounter;
    public override bool CheckBodyPartMatch(Organ checkThis)
    {

        return true;
        //base.CheckBodyPartMatch(checkThis);

    }


    private void OnEnable()
    {
        OrgansToSpawn= FindObjectOfType<Player>().OrgansToAdd;
/*
        if(OrgansToSpawn.Count>0)
        {
            for (int i = 0; i < OrgansToSpawn.Count; i++)
            {
               GameObject OrganInstance = Instantiate(OrgansToSpawn[i], transform.parent);
                OrganInstance.GetComponent<Organ>().PlayerAttributes = Attributes;
                if (OrganInstance.GetComponent<Organ>()) {

                    CurrentOrgan = OrganInstance.GetComponent<Organ>();
                    Invoke("AddOrganToGrid", 0.04f);

                }
            }
        }
        OrgansToSpawn.Clear();

*/

    }

    private void Update()
    {

        //While player is swapping out organs the game checks if the player picked new orgnas up and adds them to inventory grid
        if(OrgansToSpawn.Count > 0 && CurrentOrgan == null)
        {
            GameObject OrganInstance = Instantiate(OrgansToSpawn[0], transform.parent);
            OrganInstance.transform.position = new Vector3(1, 1, 1); 
            OrganInstance.GetComponent<Organ>().PlayerAttributes = Attributes;
            if (OrganInstance.GetComponent<Organ>())
            {

                CurrentOrgan = OrganInstance.GetComponent<Organ>();
                //Move the organ through all the grids from left to right and top to down looking for a spot to spawn it in 
                Invoke("AddOrganToGrid", 0.04f);

            }
        }

    }


    public void AddOrganToGrid()
    {
        // X range - 26->376


        //y range
        while (yCounter > -11) { 
        CurrentOrgan.transform.localPosition = new Vector3(25, 202, 0) + new Vector3(350 / 15 * xCounter, 250 / 10 * yCounter, 0);
           CurrentOrgan.InitialPlacement();
        if (CurrentOrgan.CurrentBodyPart ||  CurrentOrgan.SuccessfulPlacement) { yCounter = -100; }

        xCounter++;

        if (xCounter > 15)
        {
                //x range
            yCounter--;
            xCounter = 0;
        }
        }
        yCounter =0;
        xCounter = 0;
        CurrentOrgan = null;
        OrgansToSpawn.RemoveAt(0);
        // remove organ from list of unspawned organs



        /*
                for (int i = 0; i < Dimensions.x; i++)
                {
                    for (int j = 0; j < Dimensions.y; j++)
                    {

                        CurrentOrgan.transform.localPosition = Start  + new Vector2(Dimensions.x*Step.x , -Dimensions.y * Step.y );
                       bool placement= CurrentOrgan.CheckPlacement();

                        print(CurrentOrgan.transform.localPosition);

                        if (placement)
                        {
                            print("Successful Placement");
                            break;
                        }
                    }
                }*/
    }
}
