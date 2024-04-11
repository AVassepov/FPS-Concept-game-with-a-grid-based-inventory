using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakSpot : DamagableCreature
{
    private DamagableCreature target;

    private void Awake()
    {
        target = transform.parent.GetComponent<DamagableCreature>();
    }

    public override void ChangeHealth(float Damage)
    {
        base.ChangeHealth(Damage);

        float multiplier = Damage * 10;
        target.ChangeHealth(multiplier);


    }
}
