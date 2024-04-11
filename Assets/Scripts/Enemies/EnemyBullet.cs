using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBullet : Bullet
{
    // Update is called once per frame
    void Update()
    { 
        transform.position = Vector3.MoveTowards(transform.position, TargetPosition, BulletVelocity);

        if(transform.position == TargetPosition )
        {
            Destroy(gameObject);
        }

    }


}
