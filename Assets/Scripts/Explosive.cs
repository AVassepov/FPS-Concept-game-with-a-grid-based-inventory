using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Explosive : MonoBehaviour
{

    [Range(0.0f, 10.0f)]
    [SerializeField] private float FuzeTime;
    [SerializeField] private Light Light;
    [SerializeField] private GameObject Explosion;
    private Material Material;

    private float newIntensity;
   // Start is called before the first frame update
   void Start()
    {
        Invoke("Explode", FuzeTime);   
        Material = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        FlashingLights();

    }


    private void Explode()
    {
        Instantiate(Explosion , transform.position, Quaternion.identity);
        Destroy(gameObject);
    }


    private void FlashingLights()
    {

         newIntensity += Time.deltaTime;
        float y = Mathf.Abs(1 * Mathf.Sin(3 * newIntensity));

        Material.SetFloat("_Intensity", y);
        Light.intensity = y;
    }


}
