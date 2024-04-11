using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{


    public Vector3 position;
     public Animator animations;


    public float ChargeUpRate;
    public float MaxChargeUp;

    public float Range=3;
    [SerializeField] private Transform origin;
    private int AttackSequence;

    private float TimeSinceFired;


    public List<DamagableCreature> IgnoreThis;
    private Coroutine RecoveryInstance;


    private float ChargeUp =1 ;

    Collider m_Collider;
    RaycastHit m_Hit;
    bool m_HitDetect;
    // Start is called before the first frame update
    void Start()
    {
        animations = GetComponentInChildren<Animator>();
    }


    void OnDrawGizmos()
    {
        DrawBoxCast(
            origin.position,
            origin.position + Range * origin.forward,
            origin.lossyScale,
            origin.rotation
        );
    }

    // Update is called once per frame
    void Update()
        {
            //Parry(true);
        }

    public override void Shoot(Transform ShootingPosition)
    {
        base.Shoot(ShootingPosition);
        //Do melee Attack instead

        /*   if (Time.time > TimeSinceFired)
           {
               TimeSinceFired = Time.time + ROF;
               AttackSequence++;
               if (AttackSequence > 3)
               {
                   AttackSequence = 1;

               }
               animations.ResetTrigger("Recover");
               MeleeAttack( ShootingPosition);

           }*/

        if(ChargeUp < MaxChargeUp)
        {
            ChargeUp += ChargeUpRate;
        }

    }

    public void Parry(bool State)
    {
        if (State)
        {
            animations.SetTrigger("Block");
        }
        else
        {
            animations.SetTrigger("Unblock");

            animations.ResetTrigger("Attack1");
            animations.ResetTrigger("Attack2");
            animations.ResetTrigger("Attack3");
            animations.ResetTrigger("Recover");
        }

    }
    public void MeleeAttack(Transform ShootingPosition)
    {
        print("Do melee");

        origin = ShootingPosition;

        m_HitDetect = Physics.BoxCast(ShootingPosition.position - ShootingPosition.forward,
                ShootingPosition.lossyScale * 0.5f,
                ShootingPosition.forward, out m_Hit,
                ShootingPosition.rotation,
              Range);

        DamagableCreature target = null;
        if (m_Hit.collider) { target = m_Hit.collider.GetComponent<DamagableCreature>(); }


        if (m_HitDetect && target && !IgnoreThis.Contains(target))
        {
            //Output the name of the Collider your Box hit
            Debug.Log("Hit : " + m_Hit.collider.name);
            target.ChangeHealth(Damage * ChargeUp);
        }
        ChargeUp = 1;



    }
    void DrawBoxCast(Vector3 start, Vector3 end, Vector3 size, Quaternion rotation)
    {
        Gizmos.color = Color.green;
        // Cache the Gizmos matrix.
        Matrix4x4 currentMatrix = Gizmos.matrix;
        // Draw Cubes
        Gizmos.matrix = Matrix4x4.TRS(start, rotation, size);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = Matrix4x4.TRS(end, rotation, size);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        // Draw Connecting Lines
        Vector3 x = Vector3.right * size.x * 0.5f;
        Vector3 y = Vector3.up * size.y * 0.5f;
        Vector3 z = Vector3.forward * size.z * 0.5f;
        Gizmos.matrix = Matrix4x4.TRS(start, rotation, Vector3.one);
        Gizmos.DrawRay(Vector3.zero - x - y - z, Vector3.forward * Range);
        Gizmos.DrawRay(Vector3.zero - x + y - z, Vector3.forward * Range);
        Gizmos.DrawRay(Vector3.zero + x - y - z, Vector3.forward * Range);
        Gizmos.DrawRay(Vector3.zero + x + y - z, Vector3.forward * Range);
        // Reset the Gizmos matrix.
        Gizmos.matrix = currentMatrix;
    }

}
