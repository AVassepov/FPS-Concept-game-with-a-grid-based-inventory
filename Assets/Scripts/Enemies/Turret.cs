using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Turret : DamagableCreature
{
    private Transform Player;

    [Header("Stats")]
    [SerializeField] private float Range;
    [SerializeField] private float Damage;
    [SerializeField] private float BulletVelocity;
    [SerializeField] private float FireRate;


    [Header("Components")]
    [SerializeField] private GameObject Projectile;
    [SerializeField] private Transform Barrel;
    [SerializeField] private Transform ShootingOrigin;

    private void Awake()
    {
        Player = FindObjectOfType<Player>().transform;
    }

    private void Update()
    {
        if(Barrel!= null)
        { 
            Barrel.transform.LookAt(Player);
        }


    }

    private void Start()
    {
        StartCoroutine(DetectPlayer(2));
    }

    private IEnumerator DetectPlayer(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);


            RaycastHit hit;
            // Check if player is in sight every once in a while 
            if (Physics.Raycast(transform.position, Player.position- transform.position, out hit, Range , ~8) && hit.transform.GetComponent<Player>())
            {
                ShootBullet();
            }
        }
    }



    private void ShootBullet()
    {
      EnemyBullet BulletInstance=  Instantiate(Projectile , ShootingOrigin.position, Quaternion.identity).GetComponent<EnemyBullet>();
        BulletInstance.damage = Damage;
        BulletInstance.BulletVelocity = BulletVelocity;
        BulletInstance.TargetPosition = Player.position;
    }
}
