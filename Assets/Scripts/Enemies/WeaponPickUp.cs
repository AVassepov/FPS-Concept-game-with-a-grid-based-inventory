using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponPickUp : MonoBehaviour
{
    private BaseEnemyAI AI;

    private WeakSpot[] weakspots;

    private void Start()
    {
        AI = transform.parent.GetComponent<BaseEnemyAI>();
        weakspots = transform.parent.GetComponentsInChildren<WeakSpot>();    
    }
    public virtual void OnTriggerEnter(Collider other)
    {
        Weapon Weapon = other.GetComponent<Weapon>();

        if (Weapon)
        {
            AI.HeldWeapon = Weapon;
            AI.PossibleWeaponPickUps.Remove(Weapon);
            
            Destroy(AI.HeldWeapon.GetComponent<Rigidbody>());
            AI.HeldWeapon.transform.parent = AI.transform.GetChild(0);
            AI.HeldWeapon.transform.localPosition = new Vector3(0, 0, 0);
            AI.HeldWeapon.transform.rotation = new Quaternion(0, 0, 0, 0);
            AI.HeldWeapon.currentHolder = AI;

            if (Weapon is MeleeWeapon)
            {
                MeleeWeapon meleeWeapon = Weapon.GetComponent<MeleeWeapon>();
                meleeWeapon.IgnoreThis.Add( AI);
                for (int i = 0; i < weakspots.Length; i++)
                {
                    meleeWeapon.IgnoreThis.Add(weakspots[i]);
                }
            }
        }
    }

}
