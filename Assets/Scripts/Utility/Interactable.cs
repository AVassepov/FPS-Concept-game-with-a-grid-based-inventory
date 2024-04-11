using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    public GameObject InteractWithThis;


    public virtual void Interaction()
    {
        InteractWithThis.SetActive(true);

    } 


}


