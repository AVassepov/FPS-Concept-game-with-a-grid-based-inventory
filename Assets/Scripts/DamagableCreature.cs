using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamagableCreature : MonoBehaviour
{
    public float MaxHP = 100;
    public float CurrentHP;

    private void Start()
    {
        CurrentHP = MaxHP;
    }
    public virtual void ChangeHealth(float Damage)
    {
        CurrentHP -= Damage;

        if (CurrentHP <= 0)
        {
            BaseEnemyAI AI = gameObject.GetComponent<BaseEnemyAI>();

            if (AI)
            {
                Destroy(AI);
            }


            Destroy(this);

        }

    }

}
