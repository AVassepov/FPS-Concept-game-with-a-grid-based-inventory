using UnityEngine;
using Array2DEditor;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UIElements;


public class Organ : DraggableUI
{
    public OrganScriptableObject Data;

    public PlayerAttributes PlayerAttributes;

    //draggble UI makes this organ draggble kinda like items in S.T.A.L.K.E.R games or EFT    

    [SerializeField] private GameObject TilePrefab;
    private Transform Image;
    public Vector3 SavedLocation;
    public Quaternion SavedRotation;
    public List<OrganTile> TileList;
    public bool SuccessfulPlacement;

    private BodyPartGrid previousBodyPart;
    public BodyPartGrid CurrentBodyPart;
    private Player player;

    private Vector2 Center;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        Image = transform.GetChild(0);
        Center = new Vector2(Data.Dimensions.GridSize.x / 2 * 25, Data.Dimensions.GridSize.y / 2 * 25);
        SavedLocation = transform.position;
        SavedRotation = transform.rotation;
        SetTiles();


        //place all organs innitially to make sure that they are properly alligned in the inventory at first launch
        Invoke("InitialPlacement", 0.08f);



    }

    private void SetTiles()
    {
        //give organ organ tiles for placement purposes
        for (int i = 0; i < Data.Dimensions.GridSize.x; i++)
        {
            for (int j = 0; j < Data.Dimensions.GridSize.y; j++)
            {
                if (Data.Dimensions.GetCell(j, i) == true)
                {

                    GameObject temp = Instantiate(TilePrefab, transform);
                    temp.transform.localPosition = new Vector2(j * 25, i * 25) - Center;


                }
            }
        }
        Image.SetAsLastSibling();
    }

    public void ClearTiles()
    {
        //when picking up an organ make sure that the body part tiles it was occupying are freed
        for (int i = 0; i < TileList.Count; i++)
        {
            if (TileList[i].CurrentTile && TileList[i].CurrentTile.OccupyingTile)
            {
                TileList[i].CurrentTile.OccupyingTile = null;
                TileList[i].CurrentTile = null;
                print("Cleared");
            }
            else
            {
                print("Failed to clear");
            }

        }

        if (CurrentBodyPart != null)
        {
            CurrentBodyPart.OrgansOnTiles.Remove(this);
            CurrentBodyPart = null;
        }
    }

    public bool CheckPlacement()
    {
        //check if all tiles are valid 
        for (int i = 0; i < TileList.Count; i++)
        {
            //check if right body part to organ match
            if (TileList[i].CurrentTile != null)
            {
                BodyPartGrid bodyPartGrid = TileList[i].CurrentTile.GetComponentInParent<BodyPartGrid>();

                if (!bodyPartGrid.CheckBodyPartMatch(this))
                {


                    if (SavedLocation != Vector3.zero)
                    {

                        //failed to get placed , so get back to its previous location where it was placed just fine
                        transform.position = SavedLocation;
                        transform.rotation = SavedRotation;

                    }
                    if (previousBodyPart is not InventoryGrid) { UpdatePlayerAttributes(1); CurrentBodyPart = previousBodyPart; }
                    print("Wrong body part");

                    return false;
                }
            }

            //check if all tiles are on body part tiles
            if (!TileList[i].CurrentTile)
            {


                if (SavedLocation != Vector3.zero)
                {
                    //failed to get placed , so get back to its previous location where it was placed just fine
                    transform.position = SavedLocation;
                    transform.rotation = SavedRotation;

                }
                if (previousBodyPart is not InventoryGrid) { UpdatePlayerAttributes(1); CurrentBodyPart = previousBodyPart; }
                print("some part has no tile");


                return false;
            }

            //check if all tiles are on free tiles
            if (TileList[i].CurrentTile.OccupyingTile != null)
            {
                if (SavedLocation != Vector3.zero)
                {
                    //failed to get placed , so get back to its previous location where it was placed just fine
                    transform.position = SavedLocation;
                    transform.rotation = SavedRotation;

                }
                if (previousBodyPart is not InventoryGrid) { UpdatePlayerAttributes(1); CurrentBodyPart = previousBodyPart; }
                print("trying to get placed on occupied tile");


                return false;
            }
        }

        //put organ where it should be 
        Vector3 positionTotal = new Vector3(0, 0, 0);

        for (int i = 0; i < TileList.Count; i++)
        {
            positionTotal += TileList[i].CurrentTile.transform.position;
        }
        if (TileList.Count > 0)
        {
            transform.position = positionTotal / TileList.Count;
            Image.localPosition = Vector3.zero;
        }

        //place organ tiles on body tiles
        for (int i = 0; i < TileList.Count; i++)
        {
            TileList[i].transform.position = TileList[i].CurrentTile.transform.position;
            TileList[i].CurrentTile.OccupyingTile = TileList[i];


            //update rotation and location if this organ is placed incorrectly in the future
            SavedLocation = transform.position;
            SavedRotation = transform.rotation;

        }
        CurrentBodyPart = TileList[0].CurrentTile.GetComponentInParent<BodyPartGrid>();
        CurrentBodyPart.OrgansOnTiles.Add(this);
        previousBodyPart = CurrentBodyPart;


        if (CurrentBodyPart is not InventoryGrid)
        {
            UpdatePlayerAttributes(1);
        }
        SuccessfulPlacement = true;
        return true;
    }

    // show the info of this organ in UI
    public void PopUp(PopUpData PopUp)
    {
        PopUp.Data = Data;
        PopUp.UpdateInfo();

    }

    public void InitialPlacement()
    {
        for (int i = 0; i < TileList.Count; i++)
        {
            TileList[i].LookForTiles();
        }

        CheckPlacement();
    }

    public void CheckTiles()
    {
        //check positioning for this organ's tiles 
        for (int i = 0; i < TileList.Count; i++)
        {
            TileList[i].LookForTiles();
        }
    }



    public void UpdatePlayerAttributes(float operation)
    {
        // when placed apply organ's effects to player attributes
        for (int i = 0; i < Data.Effects.Count; i++)
        {
            switch (Data.Effects[i])
            {
                case OrganScriptableObject.Effect.HP:
                    PlayerAttributes.HP += operation * Data.EffectValues[i];
                    player.MaxHP += operation * Data.EffectValues[i];
                    print("HP");
                    break;
                case OrganScriptableObject.Effect.Stamina:
                    PlayerAttributes.Stamina += operation * Data.EffectValues[i];
                    player.Stamina += operation * Data.EffectValues[i];
                    print("Stamina");
                    break;
                case OrganScriptableObject.Effect.Speed:
                    PlayerAttributes.Speed += operation * Data.EffectValues[i];
                    player.MovementSpeed += operation * Data.EffectValues[i];
                    player.SprintingSpeed += operation * 1.2f * Data.EffectValues[i];
                    player.MaximumSlidingForce += operation * 1.5f * Data.EffectValues[i];
                    print("Speed");
                    break;
                case OrganScriptableObject.Effect.Melee_Attack:
                    PlayerAttributes.Melee_Attack += operation * Data.EffectValues[i];
                    player.MeleePower += operation * Data.EffectValues[i];
                    print("Melee Attack");
                    break;
                case OrganScriptableObject.Effect.Essense_Capacity:
                    PlayerAttributes.Essense_Capacity += operation * Data.EffectValues[i];
                    player.EssenseCapacity += operation * Data.EffectValues[i];
                    print("Essense Capacity");
                    break;
                case OrganScriptableObject.Effect.Essense_Potency:
                    PlayerAttributes.Essense_Potency += operation * Data.EffectValues[i];
                    player.EssensePotency += operation * Data.EffectValues[i];
                    print("Essense Potency");
                    break;
                case OrganScriptableObject.Effect.Reload_Speed:
                    PlayerAttributes.Reload_Speed += operation * Data.EffectValues[i];
                    print("Reload speed");
                    break;
                case OrganScriptableObject.Effect.Accuracy:
                    PlayerAttributes.Accuracy += operation * Data.EffectValues[i];
                    print("Accuracy");
                    break;
                case OrganScriptableObject.Effect.Physical_Resistance:
                    PlayerAttributes.Physical_Resistance += operation * Data.EffectValues[i];
                    print("Physical Resistance");
                    break;
                case OrganScriptableObject.Effect.Chemical_Resistance:
                    PlayerAttributes.Chemical_Resistance += operation * Data.EffectValues[i];
                    print("Chemical Resistance");
                    break;
                case OrganScriptableObject.Effect.Health_Regeneration:
                    PlayerAttributes.Health_Regeneration += operation * Data.EffectValues[i];
                    print("Health Regen");
                    break;
            }

        }


        //some organs give unique effects and abilities, they are enabled here using bools
        for (int i = 0; i < Data.UniqueEffects.Count; i++)
        {
            switch (Data.UniqueEffects[i])
            {
                /// unique effects after this 
                case OrganScriptableObject.UniqueEffect.Essense_Haste:
                    if (operation == 1)
                    {
                        PlayerAttributes.Essense_Haste = true;

                    }
                    else if (operation == -1)
                    {
                        PlayerAttributes.Essense_Haste = false;
                    }

                    break;
            }
        }

    }


}
