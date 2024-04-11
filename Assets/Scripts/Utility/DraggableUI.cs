using UnityEngine;
using UnityEngine.EventSystems;
public class DraggableUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{


    private UIManager manager;
    public bool Selected;
    private PopUpData popUp;

    private void Awake()
    {
        manager = transform.parent.GetComponent<UIManager>();
        popUp = manager.PopUpData;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        //when hovering over organ add it to UI manager and add it to UI to show info of the organ
        if(this as Organ && !manager.holdingSomething)
        {
            print("I am an organ");
            GetComponent<Organ>().PopUp(popUp);
        }
        if (!manager.holdingSomething)
        {
            // if player is is not olding something yet , they should be holding this organ
            manager.ManageThis = this;
            Selected = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        if (this as Organ && !manager.holdingSomething && manager.ManageThis== this)
        {
            print("I am an organ");
            popUp.ShowAttributes();
        }

        //stop showing and holding object if it is not hovered over anymore
        if (!manager.holdingSomething)
        {
            manager.ClearSelection();
            Selected = false;

        }

        // this was the last object held
        manager.LastHeld = this;
    }

}
