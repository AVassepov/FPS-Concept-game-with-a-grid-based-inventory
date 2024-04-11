using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    [SerializeField]private Vector3 CurrentRotation;
    [SerializeField] private Vector3 TargetRotation;


    // Hipfire
    public Vector3 RecoilVector;

    public float Snappiness;
    public float ReturnSpeed;


    private void Start()
    {
            
    }
    private void Update()
    {
        TargetRotation = Vector3.Lerp(TargetRotation, Vector3.zero, ReturnSpeed * Time.deltaTime);
        CurrentRotation = Vector3.Slerp(CurrentRotation, TargetRotation, Snappiness*Time.fixedDeltaTime);

        transform.localRotation = Quaternion.Euler(CurrentRotation);
    }


    public void RecoilFire()
    {
        TargetRotation += new Vector3(-RecoilVector.x, Random.RandomRange(-RecoilVector.y, RecoilVector.y), Random.RandomRange(-RecoilVector.z, RecoilVector.z));
    }

    public void ChangeData(Vector3 Recoil, float snappiness, float returnSpeed)
    {
        RecoilVector = Recoil;
        Snappiness = snappiness;
        ReturnSpeed = returnSpeed;

    }


}
