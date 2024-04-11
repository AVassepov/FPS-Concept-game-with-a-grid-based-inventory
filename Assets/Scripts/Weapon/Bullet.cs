using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public DamagableCreature IgnoreThis;
    [SerializeField] private GameObject Hitmarker;
    [SerializeField] private GameObject DamageText;
    public float damage;


    public float BulletVelocity;
    public Vector3 TargetPosition;
    private float angle;

    private void OnCollisionEnter(Collision collision)
    {
        DamagableCreature creature = collision.gameObject.GetComponent<DamagableCreature>();

        if (creature != IgnoreThis)
        {
            HitTarget(creature);
        }
        if(! collision.gameObject.GetComponent<Bullet>() )
        {
            Destroy(gameObject);

        }
    }


    public virtual void HitTarget(DamagableCreature creature)
    {
        if (creature)
        {
            if (!BlockBullet(creature)) { 
            creature.ChangeHealth(damage);

                if (Hitmarker != null)
                {
                    GameObject HitMarkerInstance = Instantiate(Hitmarker, transform.position, Quaternion.identity);
                    HitMarkerInstance.transform.LookAt(FindObjectOfType<Player>().transform);
                }
                if (DamageText != null)
                {
                    GameObject DamageTextInstance = Instantiate(DamageText, transform.position, Quaternion.identity);
                    DamageTextInstance.transform.LookAt(FindObjectOfType<Player>().transform);
                    DamageTextInstance.GetComponentInChildren<DamageText>().DamageValue = damage;
                }
            }
           Destroy(gameObject);
        }
    }

    public virtual bool BlockBullet( DamagableCreature creature )
    {

        if (creature)
        {
            Player player = creature.GetComponent<Player>();


            if (player != null)
            {
                    Vector3 dir = player.Camera.transform.position - transform.position;
                    dir = player.Camera.transform.InverseTransformDirection(dir);
                    angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
           



                if (!player.IsBlocking)
                {
                    Debug.Log("Hit Player That Wasn't Blocking");
                    return false;
                }

                if (player.IsBlocking && ((angle < 180 && angle > 130) || (angle > -180 && angle < -130)))
                {
                    Debug.Log("Blocked Hit");
                    return true;
                }
                else if (player.IsBlocking)
                {
                    Debug.Log("Failed To block");
                    return false;
                }
            }
            print(angle);

        }

        return false;
    }

}
