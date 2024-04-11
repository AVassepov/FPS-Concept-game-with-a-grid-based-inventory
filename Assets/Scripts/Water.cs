using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{

    [SerializeField] private float Buoyancy = 5;
    private float volume;


    private void Awake()
    {
        volume = transform.localScale.x * transform.localScale.y * transform.localScale.z;


    }
    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if(rb != null)
        {
            //float multiplier =  10 + (transform.position.y - other.transform.position.y);

            float multiplier = Mathf.Clamp01(-rb.transform.position.y / 1); 

            rb.AddForce(Vector3.up * Mathf.Abs(Physics.gravity.y * multiplier));
            print(multiplier);
        }
    }
}
