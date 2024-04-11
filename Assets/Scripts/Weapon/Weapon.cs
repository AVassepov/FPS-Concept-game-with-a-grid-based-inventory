using System.Collections;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Coroutine reloading;
    public enum FiringMode
    {
        PumpAction,
        SemiAuto,
        FullAuto,
        Burst,
        Laser, 
        Melee
    };
    [Header("Stats")]
    public float Damage;
    public float ProjectileCount;
    public float ROF;
    public float BulletVelocity;
    public float Innacuracy;
    public float AimedInnacuracy;
    public FiringMode Mode;
    public int layer = 7;


    [Header("Ammo")]
    public int MagSize;
    public int CurrentAmmo;
    public int MaxSpareAmmo;
    public int SpareAmmo;
    [SerializeField] private float ReloadTime;


    private float currentInnacuracy;


    public DamagableCreature currentHolder;
    public Player Player;

    private AudioSource ShootingSound;
    private AudioSource RackingSound;

    [Header("Polish")]
    public GameObject Bullet;
    public GameObject EnemyBullet;
    [SerializeField] private GameObject Casing;
    public bool Aiming;
    [SerializeField] private GameObject TrailPrefab;
    private PlayerUI playerUI; 


    [Header("Recoil")] 
 
    public Vector3 Recoil;
    public float Snappiness;
    public float ReturnSpeed;


    public Vector3 AimedRecoil;
    public float AimedSnappiness;
    public float AimedReturnSpeed;



    private float TimeSinceFired;


    private void Awake()
    {
        if (GetComponents<AudioSource>().Length> 0)
        {
            ShootingSound = GetComponents<AudioSource>()[0];
        }
        if (GetComponents<AudioSource>().Length > 1)
        {
            RackingSound = GetComponents<AudioSource>()[1];
        }
        currentInnacuracy = Innacuracy;
        CurrentAmmo = MagSize;
    }

    public virtual void Shoot(Transform ShootingPosition)
    {

        if (Time.time > TimeSinceFired && CurrentAmmo > 0)
        {

            TimeSinceFired = Time.time + ROF;
            CurrentAmmo--;

            if (Mode == FiringMode.Laser)
            {
                ShootRay(ShootingPosition);
                currentHolder.GetComponent<Player>().Recoil.RecoilFire();
                print("Shooting"); 
                
                if (currentHolder as Player && Player && Player.semiAutoAttack)
                {
                    currentHolder.GetComponent<Player>().Recoil.RecoilFire();
                    ShootRay(ShootingPosition);
                    print("Shooting");
                }
                else
                {
                    ShootRay(ShootingPosition);
                    print("Opp Shooting");
                }
            }
            else if (Mode == FiringMode.PumpAction)
            {

                if (currentHolder as Player && Player && Player.semiAutoAttack )
                {
                    ShootShotgun(ShootingPosition, Bullet);
                    currentHolder.GetComponent<Player>().Recoil.RecoilFire(); 
                    Player.semiAutoAttack = false;
                    print("Player Shooting");
                }
                else
                {
                    ShootShotgun(ShootingPosition, EnemyBullet);
                    print("Opp Shooting");
                }
            }
        }
    }




    private void ShootRay(Transform ShootingPosition)
    {
        RaycastHit hit;
        Vector3 innacuracy = Random.insideUnitSphere * currentInnacuracy;
        if (Physics.Raycast(ShootingPosition.transform.position, ShootingPosition.transform.forward + innacuracy, out hit, 10000 , ~currentHolder.gameObject.layer ))
        {
            Debug.DrawRay(ShootingPosition.transform.position, ShootingPosition.transform.forward, Color.green);
            DamagableCreature creature = hit.transform.gameObject.GetComponent<DamagableCreature>();
            if (creature)
            {
                creature.ChangeHealth(Damage);
            }
            print(currentHolder.gameObject.layer);
            BulletTrail(hit.transform.position);
        }
        else
        {
            BulletTrail((ShootingPosition.forward + innacuracy)*10000);
        }
    }

    private void ShootShotgun(Transform ShootingPosition , GameObject BulletUsed)
    {
        for (int i = 0; i < ProjectileCount; i++)
        {
            GameObject BulletInstance = Instantiate(BulletUsed, ShootingPosition.position, Quaternion.identity);
            //BulletInstance.layer = layer;
            BulletInstance.GetComponent<Rigidbody>().velocity = BulletVelocity * ShootingPosition.forward + Random.insideUnitSphere * currentInnacuracy;
            BulletInstance.GetComponent<Bullet>().damage = Damage; 
            BulletInstance.GetComponent<Bullet>().IgnoreThis = currentHolder;
        }


        if(ShootingSound != null)
        {
            ShootingSound.Play();
        }

        if (RackingSound)
        {
            StartCoroutine(PlaySoundWithDelay(RackingSound,0.5f , ShootingPosition)) ;
        }


    }

    IEnumerator PlaySoundWithDelay(AudioSource Sound , float delay , Transform ShootingPosition)
    {

        yield return new WaitForSeconds(delay);
        Sound.Play();
        //casing
        if (Casing != null)
        {
            GameObject CasingInstance = Instantiate(Casing, ShootingPosition.position + new Vector3(0.1f, 0.1f, 0.1f), Random.rotation);
            CasingInstance.GetComponent<Rigidbody>().AddForce((transform.up + transform.right) * 1.2f, ForceMode.Impulse);
        }
    }




    public void Aim()
    {
        Player.Recoil.ChangeData(AimedRecoil, AimedSnappiness,AimedReturnSpeed);
        currentInnacuracy = AimedInnacuracy;
    }
    public void UnAim()
    {
        Player.Recoil.ChangeData(Recoil, Snappiness, ReturnSpeed); 
        currentInnacuracy = Innacuracy;
    }

    private void BulletTrail(Vector3 TargetPosition )
    {
        GameObject trailInstance=  Instantiate(TrailPrefab , transform.position , Quaternion.identity);
        trailInstance.GetComponent<BulletTrail>().Target= TargetPosition;

    }

      private IEnumerator StartReload()
      {
          yield return new WaitForSeconds(ReloadTime);
          FinishReload();
      }


    public void Reload()
    {
        if (reloading == null) { 
         reloading=  StartCoroutine(StartReload());
        }
    }

    private void FinishReload()
    {

        SpareAmmo += CurrentAmmo;

        if(SpareAmmo >= MagSize) {
            CurrentAmmo = MagSize;
            SpareAmmo -= CurrentAmmo;
        }
        else
        {
            CurrentAmmo = SpareAmmo;
            SpareAmmo = 0;
        }


        reloading = null; 
    }

}



