using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    [Range(1f, 15f)]
    [SerializeField]private float MaximumSize;
    [SerializeField] private float damage = 50;
    [SerializeField] private Light Light;

    private float newIntensity;

    private bool FirstScaleChange;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SelfDestruct", 1);
    }

    // Update is called once per frame
    void Update()
    {
        FlashingLights();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DamagableCreature>() != null)
        {
            other.GetComponent<DamagableCreature>().ChangeHealth(damage);
            print("Hit Something");

        }

    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
    private void FlashingLights()
    {
        newIntensity += Time.deltaTime;
        float y = Mathf.Abs(1 * Mathf.Sin(3 * newIntensity));
        transform.localScale = new Vector3(y,y,y)*MaximumSize ;
        Light.intensity = y * 10;

    }



}
