using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemyAI : DamagableCreature
{
    [Header("Stats")]
    [SerializeField] private float VisibilityRange ;
    [SerializeField] private float VisibilityHeight ;
    [Range(0, 1)]
    [SerializeField] private float AngThresh ;
    public Weapon HeldWeapon;
    [SerializeField] private Vector3 SpawnChances = Vector3.one;


    [SerializeField] private OrganPool Pool;


    [Header("Info")]
    [SerializeField] private AgressionType Type;
    [SerializeField] private bool CanSearchForWeapons;
    public State CurrentState;


    public enum State
    {
       Idle,
       Moving,
       PickingUp,
       SpottedTarget,
       Attack,
       SpecialAttack
    };

   public enum AgressionType
    {
        PlayerOnly,
        Rabid,
        Predator,

    };

    private List<DamagableCreature> PossibleTargets = new List<DamagableCreature>();
    public List<Weapon> PossibleWeaponPickUps = new List<Weapon>();
    private Vector3 Target;
    private bool hasTarget;

    //components
    private Transform player;
    private NavMeshAgent agent;
    private WedgeTrigger trigger;

    //Data
    private Coroutine roam;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player= FindAnyObjectByType<Player>().transform.GetChild(0);
        trigger=  gameObject.AddComponent<WedgeTrigger>();

        //set trigger attributes
        trigger.AngThresh = AngThresh;
        trigger.Radius = VisibilityRange;
        trigger.Height = VisibilityHeight;
        trigger.Target= player;

        if (Type == AgressionType.Rabid)
        {
            PossibleTargets.AddRange(FindObjectsOfType<DamagableCreature>());
            PossibleTargets.Remove(this);


        }

        if(CanSearchForWeapons)
        {
            PossibleWeaponPickUps.AddRange(FindObjectsOfType<Weapon>());
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(!hasTarget && roam==null)
        {
            // roam
            roam = StartCoroutine(Roam());
        }


        //Behaviour type
        if (Type == AgressionType.PlayerOnly)
        {
            if (DetectPlayer())
            {
                agent.destination = player.position;
                if (roam != null) { StopCoroutine(roam); }
                roam = null;
                hasTarget = false;
            }
        }
        else if (Type == AgressionType.Rabid)
        {
            SearchForSomething(null, PossibleTargets, false);
            if (hasTarget)
            {
                AttackTarget();
                agent.destination = Target;
                hasTarget = false;
                Target = new Vector3(0, 0, 0);
                if (roam !=null){ StopCoroutine(roam);}
                roam = null;
            }


        }


        if (CanSearchForWeapons)
        {
            SearchForSomething(PossibleWeaponPickUps, null, true);
            if (hasTarget)
            {
                agent.destination = Target;
                hasTarget = false;
                Target = new Vector3(0, 0, 0);
                if (roam != null) { StopCoroutine(roam); }
                roam = null;
            }

        }
    }


    public  virtual  bool  DetectPlayer()
    {
        hasTarget = true;
      return   trigger.Constraints(player.position);
        
    }

    public virtual void SearchForSomething(List<Weapon> Weapons , List<DamagableCreature> CreatureList  , bool SearchingForWeapon)
    {
        if(SearchingForWeapon) { 
            for (int i = 0; i < Weapons.Count; i++)
            {
                if (!hasTarget)
                {
                    hasTarget = trigger.Constraints(Weapons[i].transform.position);
                    Target = Weapons[i].transform.position;
                }
            }
        }
        else
        {
            for (int i = 0; i < CreatureList.Count; i++)
            {
                if (!hasTarget && CreatureList[i])
                {
                    hasTarget = trigger.Constraints(CreatureList[i].transform.position);
                    Target = CreatureList[i].transform.position;
                }
                else
                {
                    CreatureList.RemoveAt(i);
                }
            }

        }
    }


    public virtual void AttackTarget()
    {


        if(HeldWeapon &&!(HeldWeapon is MeleeWeapon))
        {
            HeldWeapon.Shoot(transform.GetChild(1));

        }else if (HeldWeapon)
        {
            HeldWeapon.Shoot(transform.GetChild(1));
        }
        else if(!HeldWeapon) {
            print("Attack Without Weapon");
        }


    }

    IEnumerator Roam()
    {
        yield return new WaitForSeconds(3);
        Target =new Vector3(UnityEngine.Random.Range(-5,5) + transform.position.x,0, UnityEngine.Random.Range(-5, 5) + transform.position.z);
        agent.destination = Target;
        hasTarget = false;
        Target = new Vector3(0,0,0);
        roam = null;

    }


    private void OnDestroy()
    {
        if (Pool) { 
        Instantiate(OrganSpawner.ReturnOrgan(OrganSpawner.ChooseRarity(SpawnChances.x, SpawnChances.y, SpawnChances.z, Pool)) ,transform.position, Quaternion.identity);
        }
    }


}
