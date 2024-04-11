using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganInstance : MonoBehaviour
{
    public OrganScriptableObject Organ;

    public bool Inspecting;
    
    private Player player;

    [SerializeField] private GameObject PopUpUI;
    private GameObject PopUpInstance;

    public void Start()
    {
       Rigidbody body = GetComponent<Rigidbody>();

        body.AddForce(new Vector3( Random.Range(-1,1),1.25f, Random.Range(-1, 1)) * 6, ForceMode.Impulse);

        player = FindObjectOfType<Player>();    
    }


    private void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < 3)
        {
            Inspect();
        } else if (PopUpInstance != null && Vector3.Distance(transform.position, player.transform.position) > 3)
        {
            StopInspecting();
        }   
    }

    public void Inspect()
    {
        print("Looking At Organ");
        if (PopUpInstance == null) { 
        PopUpInstance =   Instantiate(PopUpUI , transform.position , Quaternion.identity);
            
            //position and rotation stuff
            PopUpInstance.transform.position += player.Camera.transform.right*2;
            PopUpInstance.transform.position += player.Camera.transform.forward * 1.1f;
            PopUpInstance.transform.position += Vector3.up*2;
            PopUpInstance.transform.LookAt(player.transform.position);
            PopUpInstance.transform.Rotate(0, 180, 0);

            //Setup
            PopUpInstance.GetComponent<OrganPopUp>().Parent = this;
            PopUpInstance.GetComponent<OrganPopUp>().OrganInfo = Organ;
        }
    }

    public void StopInspecting()
    {
        Destroy(PopUpInstance);
        print("Stopped Looking At Organ");
    }

}
