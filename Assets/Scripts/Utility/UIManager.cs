using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public DraggableUI ManageThis;

    private Vector3 offset;

    public bool holdingSomething;
    public PopUpData PopUpData;

    public DraggableUI LastHeld;
  

    // Update is called once per frame
    void Update()
    {
        // if hovering over an organ and holding left mouse, start dragging it 
        if (Input.GetMouseButtonDown(0)  && ManageThis)
        {
            if (ManageThis.GetComponent<Organ>().CurrentBodyPart&& ManageThis.GetComponent<Organ>().CurrentBodyPart.GetComponent<InventoryGrid>() == null)
            {
                //return attributes to normal when taking object out of a body part
                print("Returned stats to previous value");
                ManageThis.GetComponent<Organ>().UpdatePlayerAttributes(-1);
            }
            PopUpData.ShowAttributes();
            //clear tiles of an organ as you pick it up so it can be placed else where
            ManageThis.GetComponent<Organ>().ClearTiles();

            // Drag the object with some offset depending on where player picked it up rather than just putting the centre of object at mouse position
           offset = ManageThis.transform.position - Input.mousePosition;

        }

        //leave organ swapping mode when pressing ESC
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            FindObjectOfType<Player>().enabled = true;
            gameObject.SetActive(false);
        }
        //keep dragging object if you are still holding it 
        if (Input.GetMouseButton(0) && ManageThis)
        {
            ManageThis.transform.position = Input.mousePosition + offset;

            holdingSomething = true;
        }

        // when releasing mouse , release the organ too
        if (Input.GetMouseButtonUp(0) && ManageThis)
        {

            ManageThis.GetComponent<Organ>().SuccessfulPlacement = ManageThis.GetComponent<Organ>().CheckPlacement();
            holdingSomething = false;

            PopUpData.ShowAttributes();
            if (!ManageThis.Selected)
            {
                ManageThis = null;

            }
        }

        //rotate object 90 degrees
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0 && ManageThis)
        {
            rotation(1);
        }
        //rotate object -90 degrees
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0 && ManageThis)
        {
            rotation(-1);
        }
    }

    //unlock mouse when leaving organ swapping UI
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //stop managing an organ when you are not holding it anymore
    public void ClearSelection()
    {
        if (ManageThis && !holdingSomething )
        {
            ManageThis=null; 
        }
    }

    //rotate objects

    //rotation step is direction of rotation and is equal to either 1 or -1
    private void rotation(float rotationStep)
    {
        if (holdingSomething) { 
        ManageThis.transform.Rotate(0, 0, 90 * rotationStep);
        }
    }
}
