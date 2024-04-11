using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleaning : MonoBehaviour
{
    [SerializeField] private float delay;
    void Start()
    {
        StartCoroutine(SelfDestruct());
    }


    IEnumerator SelfDestruct()
    {

        yield return new WaitForSeconds(delay);
        Destroy(gameObject);

    }
}
