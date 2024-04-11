using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : DamagableCreature
{
    [Header("Base Attributes")]
    public float MovementSpeed = 1;
    public float MaxAerialVelocity;
    public float SprintingSpeed = 2;
    [SerializeField] private float MouseSensitivity = 1;
    [SerializeField] private float JumpForce = 1000;

   

    [Header("Attacks")]
     public Weapon CurrentWeapon;
    public CurrentEssense LeftEssense;
    public CurrentEssense RightEssense;
    [SerializeField] private Weapon[] Weapons = new Weapon[4]; 

    [Header("Crouching")]
    [SerializeField] private float CrouchHeightChange = 0.5f;
    [SerializeField] private float crouchSpeedChange = 0.005f;

    [Header("Camera")]
    public Camera Camera;
    [Range(40f, 120f)]
    [SerializeField] private float DefaultFOV;
    [Range(40f, 120f)]
    [SerializeField] private float MidAirFOV;
    [Range(40f, 120f)]
    [SerializeField] private float SlidingFOV;
    [Range(0f, 1f)]
    [SerializeField] private float FOVChange;
    [SerializeField] private Transform WeaponPosition;
    [SerializeField] private Transform AimPosition;
    [SerializeField] private float  BobbingScale;
    [SerializeField] private float BobbingBounds;
    private float CurrentbobbingLocation;
    private float  coeficient =1;

    private Transform CurrentAimState;

    [Header("Grabbing")]
    [SerializeField] private Transform heldItemPosition;
    [Range(0f, 10f)]
    [SerializeField] private float GrabRange;
     private GameObject HeldItem;
     private Vector3 HeldItemOffset;
    [Range(10f, 50f)]
    [SerializeField] private float ThrowForce;

    [Header("Sliding")]
    [SerializeField] private float SlidingDampening = 3;
    public float MaximumSlidingForce = 250;
    [SerializeField] private float SlideTimer = 4;
    [SerializeField] private GameObject Sparks;

    [Header("Info")]
    [SerializeField] private bool IsGrounded;
    [SerializeField] private State CurrentState;
    public bool IsBlocking;
    public bool semiAutoAttack = true;
    private Vector3 dashPosition;

    [HideInInspector] public List<GameObject> OrgansToAdd;

    [Header("Additional Stats")]
    public float DashRange = 10;
    public float DashFrames = 30;
    public float KickRange = 3;
    public enum CurrentEssense
    {
        None,Haste,Leech,Javelin,Tar,Mildew
    }


    //Switch from bools to this later
    public enum State
    {
            Standing,
            Walking,
            Sprinting,
            Crouching,
            Sliding
    }

    [Header("Player Specific Attributes")]
    public float Stamina;
        public float MeleePower;
        public float Accuracy;
        public float ReloadSpeed;
        public float ChemResistance;
        public float PhysResistance;
        public float HPRegen;

        public float MaxEssenseCapacity;
        public float EssenseCapacity;
        public float EssensePotency;

    [Header("Components")]
    //components
    public Recoil Recoil;
    public GameObject Reticle;
    private Rigidbody rb;
    private  CapsuleCollider capsule;
    private Transform cameraParent;
    private Camera camera;
    private AudioSource grinding;
    private PlayerUI UI;

    //movement and scale
    private float currentspeed;
    private bool crouching;
    private float targetScale;
    private float savedHeight;
    private float currentSlideTimer;
    private bool isSliding;

    //Instances & States
    private Vector3 input;
    private GameObject SparksInstance;
    private float DesiredFOV;
    private bool isSprinting;
    private float SlidingForce;
    private float TemporarySpeedChange;
    private bool justPickedUpWeapon;
    private bool canKick = true;


    //sway
     [Header("Sway Settings")]
    [SerializeField] private float smooth;
    [SerializeField] private float multiplier;
    // Dash stuff
    
    private int dashCounter;
    private Vector3 LastDashPosition;
    private bool canJump = true;
    private void Awake()
    {
        Recoil = GetComponentInChildren<Recoil>();
        cameraParent = transform.GetChild(0);
        camera = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        grinding = GetComponent<AudioSource>();
        //lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //setup
        currentspeed = MovementSpeed;
        targetScale = capsule.height;
        savedHeight = capsule.height; 
        SlidingForce = MaximumSlidingForce;
        DesiredFOV = DefaultFOV;
        CurrentAimState = WeaponPosition;
        EssenseCapacity = MaxEssenseCapacity;
        UI = Reticle.GetComponent<PlayerUI>();
    }
    private void Start()
    {
        CurrentHP = MaxHP;

        UI.UpdateHealth(MaxHP, CurrentHP);
    }
    private void OnEnable()
    {
        //lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UI.UpdateHealth(MaxHP, CurrentHP);
    }
    private void OnDisable()
    {
        //lock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    void Update()
    {




        //smooth crouching and FOV change
        capsule.height = Mathf.Lerp(capsule.height, targetScale, 0.05f);
        Camera.fieldOfView = Mathf.Lerp(Camera.fieldOfView, DesiredFOV, FOVChange);


        //slope sliding
        UpdateSlopeSliding();



     

        //movement
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (!isSliding && input != new Vector3(0, 0, 0))
        {
            Movement(input.normalized);
            Bobbing();
        }
        if (input == new Vector3(0, 0, 0) && !isSliding)
        {
            Dampening();
        }

        //Camera Look

        Vector3 MouseRotation = cameraParent.transform.rotation.eulerAngles;
        MouseRotation.y += MouseSensitivity * Input.GetAxis("Mouse X");
        MouseRotation.x += MouseSensitivity * -Input.GetAxis("Mouse Y");
        CameraLook(MouseRotation);

        // get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * multiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * multiplier;

        // calculate sway rotation
        Quaternion swayRotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion swayRotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion swayTargetRotation = swayRotationY * swayRotationX;

        // rotate weapon
        if(CurrentWeapon != null)
        {
            CurrentWeapon.transform.localRotation = Quaternion.Slerp(transform.localRotation, swayTargetRotation, smooth * Time.deltaTime);
        }

        //Vaulting
        if (Input.GetKeyDown(KeyCode.Space))
       { 
            AttemptVault(); 
            if (canJump)
            {
                Jump();
                IsGrounded = false;
                SlidingForce = MaximumSlidingForce;
            }
       }

        if (Input.GetKeyDown(KeyCode.V) && canKick)
        {
            canKick = false;
           StartCoroutine(KickCoolDown(1));
            print("Kicked");
            Kick();
        }

        //Dashing
        if (dashPosition != Vector3.zero)
        {
            transform.position = Vector3.Lerp(LastDashPosition, dashPosition, 1/ (DashFrames- dashCounter));
            dashCounter++;
            if (dashCounter > DashFrames)
            {
                dashPosition = Vector3.zero;
                dashCounter = 0;
            }
        }


        //Grab Item
        //Interact
        if (Input.GetKeyDown(KeyCode.F))
        {
            GrabItem();
            Interact();
        }

        //Switch Weapons

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchWeapon(3);
        }
        


        else if (Input.GetKeyUp(KeyCode.F))
        {
            if (HeldItem)
            {
                HeldItem.GetComponent<Rigidbody>().useGravity = true;
                currentspeed -= TemporarySpeedChange;
                TemporarySpeedChange += HeldItem.GetComponent<Rigidbody>().mass;
                HeldItem = null;
            }
        }

        //Use Right Essense
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseEssense(true);
        }
        //Use Left Essense
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseEssense(false);
        }



        if (Input.GetKeyDown(KeyCode.Mouse1) && CurrentWeapon && CurrentWeapon.Mode == Weapon.FiringMode.Melee && !justPickedUpWeapon)
        {
            IsBlocking = true;
            CurrentWeapon.GetComponent<MeleeWeapon>().Parry(IsBlocking);

        } else if (Input.GetKeyUp(KeyCode.Mouse1) && CurrentWeapon && CurrentWeapon.Mode == Weapon.FiringMode.Melee)
        {

            if(!justPickedUpWeapon) { 
            IsBlocking = false;
            CurrentWeapon.GetComponent<MeleeWeapon>().Parry(IsBlocking);
            }
            justPickedUpWeapon = false;
        }

        if (Input.GetKeyDown(KeyCode.R) && CurrentWeapon.SpareAmmo>0 && CurrentWeapon.CurrentAmmo < CurrentWeapon.MagSize && CurrentWeapon.Mode != Weapon.FiringMode.Melee)
        {
            CurrentWeapon.Reload();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && CurrentWeapon && CurrentWeapon.Mode != Weapon.FiringMode.Melee)
        {
            CurrentWeapon.Aiming = true;
            CurrentWeapon.Aim();
            CurrentAimState = AimPosition;
            Reticle.SetActive(false);
        }
        else  if (Input.GetKeyUp(KeyCode.Mouse1) && CurrentWeapon && CurrentWeapon.Mode != Weapon.FiringMode.Melee)
        {
            CurrentWeapon.Aiming = false;
            CurrentWeapon.UnAim();
            CurrentAimState = WeaponPosition;
            Reticle.SetActive(true);
        }

        //hold item
        if (Input.GetKey(KeyCode.F) && HeldItem) {
                HeldItem.transform.position = Vector3.Lerp(HeldItem.transform.position, heldItemPosition.position + HeldItemOffset, 0.02f);
            }


        //Shoot 
        if (Input.GetKey(KeyCode.Mouse0) && CurrentWeapon && !HeldItem)
        {
            if(CurrentWeapon is MeleeWeapon )
            {
                CurrentWeapon.Shoot(transform.transform);
            }
            else if(!(CurrentWeapon is MeleeWeapon))
            {
                CurrentWeapon.Shoot(transform.GetChild(0).GetChild(0).GetChild(0));
            }
        }
        else if (Input.GetKey(KeyCode.Mouse0) && HeldItem)
        {
            Rigidbody temp = HeldItem.GetComponent<Rigidbody>();
            TemporarySpeedChange = temp.mass;
            temp.useGravity = true;
            temp.AddForce(cameraParent.transform.forward * ThrowForce , ForceMode.Impulse);
            currentspeed += TemporarySpeedChange;
            HeldItem = null;
        } 
        
        
        if (Input.GetKeyUp(KeyCode.Mouse0) && CurrentWeapon)
        {
            if (CurrentWeapon is MeleeWeapon)
            {
                CurrentWeapon.GetComponent<MeleeWeapon>().MeleeAttack(cameraParent.transform);
            }

            semiAutoAttack = true;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            semiAutoAttack = true;
        }

       

        if (Input.GetKeyDown(KeyCode.LeftShift) && !crouching)
            {
                currentspeed = SprintingSpeed + TemporarySpeedChange;
            isSprinting = true;
                DesiredFOV = SlidingFOV;
            }else if (Input.GetKeyUp(KeyCode.LeftShift) && !crouching)
            {
                currentspeed = MovementSpeed + TemporarySpeedChange; ; 
                isSprinting= false;

                        if(IsGrounded)
                        {
                            DesiredFOV = DefaultFOV;
                        }
            }


            if (Input.GetKeyDown(KeyCode.LeftControl))
            {

            StartSlide();
            Crouch(-CrouchHeightChange , crouchSpeedChange);
            rb.AddForce(-transform.up*2 , ForceMode.Impulse);
            crouching = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl) )
            {
            Crouch(CrouchHeightChange, MovementSpeed);
            crouching = false ;
            StopSlide();
            //ResetSlidingMomentum();
             }

            if(IsGrounded && !isSliding)
            {
            Speedlimiter(currentspeed);
            }else if(!IsGrounded && !isSliding)
            {
            Speedlimiter(MaxAerialVelocity);
            }


        if (isSliding)
        {
            Slide();
        }


        if (CurrentWeapon != null)
        {
            CurrentWeapon.transform.position = Vector3.Lerp(CurrentWeapon.transform.position, CurrentAimState.position, 0.75f);

        }
    }



    private void Movement(Vector3 Direction)
    {
      Vector3  movement = Quaternion.Euler(0, camera.transform.eulerAngles.y, 0) * Direction;
        //  transform.Translate(movement * currentspeed);

        rb.AddForce(movement* currentspeed, ForceMode.Force);
    
    }

    private void CameraLook(Vector3 Rotation)
    {
        cameraParent.transform.eulerAngles = Rotation;
    }

    private void Jump()
    {
        if (IsGrounded)
        {

            DesiredFOV = MidAirFOV;
            var sphereCastVerticalOffSet = capsule.height / 2 - capsule.radius;
            var castOrigin = transform.position - new Vector3(0, sphereCastVerticalOffSet, 0);

            if (Physics.SphereCast(castOrigin, capsule.radius - 0.01f, Vector3.down, out var hit, 0.05f, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore))
            {
                rb.AddForce(hit.normal * JumpForce , ForceMode.Impulse);
                print("good jump");
            }
            else
            {
                rb.AddForce(Vector3.up* JumpForce, ForceMode.Impulse);
                print("lame jump");
            }

        }
    }


    private void Crouch( float ScaleChange, float speedChange)
    {
        //make smaller, 
        //    targetScale = new Vector3(transform.localScale.x, savedHeight + ScaleChange, transform.localScale.z);
        targetScale = savedHeight + ScaleChange;
        savedHeight += ScaleChange;

        //slow down 
        currentspeed = speedChange +TemporarySpeedChange;
    }

    private void StartSlide ()
    {
        isSliding = true;
       currentSlideTimer =  SlideTimer;
        SparksInstance = Instantiate(Sparks, cameraParent.transform);
        // might change sliding force reset to something else like jump of contacting an angled surface;
        SlidingForce = MaximumSlidingForce;

        grinding.Play();
    }
    private void Slide()
    {
        Vector3 movement = Quaternion.Euler(0, cameraParent.transform.eulerAngles.y, 0) * input.normalized;
        ParticleSystem particles   = SparksInstance.GetComponent<ParticleSystem>();

        if (isSliding)
        {
            DesiredFOV = SlidingFOV;

            if(!IsGrounded)
            {
                particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                if (SlidingForce<MaximumSlidingForce)
                {
                     SlidingForce += SlidingDampening;
                }
                currentSlideTimer += Time.deltaTime;
                grinding.volume = 0;
            }
            else
            {
                rb.AddForce(movement * SlidingForce, ForceMode.Force);
                particles.Play(true);
                SlidingForce -= SlidingDampening;
                currentSlideTimer -= Time.deltaTime;
                grinding.volume = 1;
            }

            if (currentSlideTimer <= 0 || SlidingForce < 0)
            {
                StopSlide();
            }

        }
    }

    private void StopSlide()
    {
        isSliding=false;
        DesiredFOV = DefaultFOV;
        if(SparksInstance!= null)
        {
            SparksInstance.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
            StartCoroutine(DestroyWithDelay(SparksInstance , 2f));
        }
        grinding.Stop();
    }





    private void OnTriggerStay(Collider other)
    {
        if (other)
        {
            IsGrounded = true;
            if (!isSliding || !isSprinting)
            {
                DesiredFOV = DefaultFOV;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
            IsGrounded = false;

            DesiredFOV = MidAirFOV;
    }
    void UpdateSlopeSliding()
    {
        if (IsGrounded)
        {
            var sphereCastVerticalOffSet = capsule.height / 2 - capsule.radius;
            var castOrigin = transform.position - new Vector3(0, sphereCastVerticalOffSet, 0);

            if (Physics.SphereCast(castOrigin, capsule.radius - 0.01f, Vector3.down, out var hit, 0.05f , ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore))
            {
                var collider = hit.collider;
                var angle = Vector3.Angle(Vector3.up, hit.normal);
                Debug.DrawLine(hit.point, hit.point + hit.normal, Color.red, 3f);

                if (angle < 45)
                {
                    var normal = hit.normal;
                    var yInverse = 1f - normal.y;
                    rb.velocity += new Vector3(yInverse * normal.x , yInverse * normal.x, 0) ;


                }


            }
        }
    }

    private void Speedlimiter(float Limiter)
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x , 0 , rb.velocity.z);
        if(flatVelocity.magnitude > Limiter)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * Limiter;
            rb.velocity = new Vector3(limitedVelocity.x , rb.velocity.y, limitedVelocity.z);
        }

    }
    private void Dampening()
    {
        rb.velocity -= new Vector3(rb.velocity.x * 0.02f, 0, rb.velocity.z * 0.02f);
    }


    IEnumerator DestroyWithDelay(GameObject DestroyThis , float Delay)
    {
        yield return new WaitForSeconds(Delay);

        Destroy(DestroyThis);

    }
    IEnumerator KickCoolDown( float Delay )
    {
        yield return new WaitForSeconds(Delay);
        canKick = true;

    }



    private void GrabItem()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(cameraParent.transform.position, cameraParent.transform.forward , out hit, GrabRange, 5) && hit.transform.gameObject.tag== "Can Grab")
        {
            //get helditem
            HeldItem = hit.transform.gameObject;
            HeldItem.GetComponent<Rigidbody>().useGravity = false;
            TemporarySpeedChange -=HeldItem.GetComponent<Rigidbody>().mass;

            currentspeed += TemporarySpeedChange;
            //set position for held item location
            heldItemPosition.parent = null;
            heldItemPosition.localPosition = hit.point;
            HeldItemOffset = HeldItem.transform.position - heldItemPosition.position;
            heldItemPosition.parent = cameraParent.transform;
        }
        else
        {
            Debug.Log("Did not Hit");
        }

        if (Physics.Raycast(cameraParent.transform.position, cameraParent.transform.forward, out hit, GrabRange, 5) && hit.transform.GetComponent<Weapon>())
        {
            bool AddedToList = false;

            for (int i = 0; i < Weapons.Length; i++)
            {
                if (Weapons[i] == null)
                {
                    Weapons[i] = hit.transform.GetComponent<Weapon>();
                    AddedToList = true;
                    break;
                }
            }

            if (AddedToList)
            {
                if(CurrentWeapon != null)
                {
                    CurrentWeapon.gameObject.SetActive(false);
                }

                CurrentWeapon = hit.transform.GetComponent<Weapon>();
                CurrentWeapon.Player = this;

                CurrentWeapon.currentHolder = this as DamagableCreature;
                CurrentWeapon.GetComponent<Collider>().enabled = false;
                CurrentWeapon.transform.parent = WeaponPosition.transform;
                MeleeWeapon melee = CurrentWeapon.GetComponent<MeleeWeapon>();
                if (melee)
                {
                    melee.IgnoreThis.Add(this);
                    melee.animations.enabled = true;
                }

                justPickedUpWeapon = true;
                
        Recoil.ChangeData(CurrentWeapon.Recoil, CurrentWeapon.Snappiness, CurrentWeapon.ReturnSpeed);
            }
        }
    }

    private void AttemptVault()
    {
        if (Physics.Raycast(cameraParent.transform.position, cameraParent.transform.forward, out var firstHit, 1f, ~8))
        {
            print("vaultable in front");
            if (Physics.Raycast(firstHit.point + (cameraParent.transform.forward * capsule.radius) + (Vector3.up * 0.6f * capsule.height), Vector3.down, out var secondHit, capsule.height))
            {
                print("found place to land");
                StartCoroutine(LerpVault(secondHit.point + new Vector3(0,1,0), 0.5f));
            }
        }
    }



    IEnumerator LerpVault(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }

    private void Interact()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(cameraParent.transform.position, cameraParent.transform.forward, out hit, GrabRange, 5) && hit.transform.GetComponent<Interactable>())
        {
            hit.transform.GetComponent<Interactable>().Interaction();
            this.enabled = false ;
        }
        
        if(Physics.Raycast(cameraParent.transform.position, cameraParent.transform.forward, out hit, GrabRange, 5) && hit.transform.GetComponent<OrganInstance>())
        {

            OrgansToAdd.Add(hit.transform.GetComponent<OrganInstance>().Organ.UIGameobject);
            Destroy(hit.transform.gameObject);
        }

    }


    private void UseEssense(bool Right)
    {
        CurrentEssense selectedEssense;


        if (!Right) {  selectedEssense = LeftEssense; } else{ selectedEssense = RightEssense;}


        if(selectedEssense == CurrentEssense.Haste && EssenseCapacity >= 10)
        {
            EssenseCapacity-=10;
            print("Dashed");
            Haste(); 
        }else if (selectedEssense == CurrentEssense.Javelin && EssenseCapacity >= 15)
        {
            EssenseCapacity -= 15;
            print("Shot Bone");
            Javelin();
        }else if (selectedEssense == CurrentEssense.Leech && EssenseCapacity >= 25)
        {
            EssenseCapacity -= 25;
            print("threw leech");
            Leech();
        }else if (selectedEssense == CurrentEssense.Mildew && EssenseCapacity >= 40)
        {
            EssenseCapacity -= 40;
            print("threw leech");
            Mildew();
}

    }

   private void Haste()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(cameraParent.transform.position, cameraParent.transform.forward, out hit, DashRange, ~8))
        {
            dashPosition = hit.point;
            print("Hit something ");
            LastDashPosition = transform.position;

            //   transform.position = dashPosition;
        }
        else
        {

            dashPosition = transform.position + cameraParent.transform.forward * DashRange;
            print("Hit nothing ");

           LastDashPosition = transform.position;
           // transform.position += dashPosition;
        }


    }

    private void SwitchWeapon( int Index)
    {
        if (CurrentWeapon)
        {
            CurrentWeapon.gameObject.SetActive(false);
        }

            CurrentWeapon = Weapons[Index];

        if (CurrentWeapon)
        {
            CurrentWeapon.gameObject.SetActive(true);
        }

        Recoil.ChangeData(CurrentWeapon.Recoil, CurrentWeapon.Snappiness, CurrentWeapon.ReturnSpeed);
    }


    private void Javelin()
    {


    }

    private void Leech()
    {


    }
    private void Mildew()
    {


    }

    private void Kick()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(cameraParent.transform.position, cameraParent.transform.forward, out hit, KickRange) )
        {
            Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
            DamagableCreature damagable = hit.transform.GetComponent<DamagableCreature>();

            if (rb)
            {
                rb.AddForce(cameraParent.transform.forward * 35, ForceMode.Impulse);
            }
            if (damagable)
            {
                damagable.ChangeHealth(15);
            }

        }


    }

    private void Bobbing()
    {

        if (camera.transform.localPosition.y > BobbingBounds - 0.2f)
        {
            coeficient = -1;
        }

        if (camera.transform.localPosition.y < -BobbingBounds + 0.2f)
        {

            coeficient = 1;
        }
        camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, new Vector3(0, BobbingBounds * coeficient, 0) , BobbingScale);  
    }

    public override void ChangeHealth(float Damage)
    {
        base.ChangeHealth(Damage);

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
        if (CurrentHP> MaxHP)
        { 
            CurrentHP= MaxHP;
        }
            UI.UpdateHealth(MaxHP, CurrentHP);


    }


}
