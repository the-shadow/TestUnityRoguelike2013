using UnityEngine;
using System.Collections;

public enum InteractType
{
    InteractConsumable = 0,
    InteractChest,
    InteractDoor
}
;

public enum HungerLevel
{
    Starving = 0,
    Hungry,
    Satisfied,
    Full,
    Stuffed
}
;

//Describes our Animation States "Player_State" in the animator
enum pcAnim
{
    PC_RunDown = 0,
    PC_RunLeft,
    PC_RunUp,
    PC_RunRight,
    PC_FaceDown,
    PC_FaceLeft,
    PC_FaceUp,
    PC_FaceRight,
    PC_AttackDown,
    PC_AttackLeft,
    PC_AttackRight,
    PC_AttackUp,
    PC_HurtLeft
}
;

enum spacialOrientation
{
    vertical = 0,
    horizontal
}
;

enum WallCollision
{
    Wall_Below = 0,
    Wall_Left,
    Wall_Right,
    Wall_Above,
    Wall_Corner_LeftTop,
    Wall_Corner_RightTop,
    Wall_Corner_LeftBottom,
    Wall_Corner_RightBottom,
    Wall_None
}
;

public class BlueOrc_PC : Pathfinding_NavnodeAgent
{
    Button_Quit buttonQuit;

    Animator anim;
    SpriteRenderer spriteRenderer;
    //CameraFollow m_Camera;
    public float fMaxSpeed;
    private Vector2 m_vMove;
    public int m_nPlayerState;          //temp public
    private bool m_bAttackButton;
    private bool m_bAttacking;
    BoxCollider2D boxCollider;

    //BasicShadow
    Basic_Shadow m_BasicShadow;

    //Heath and Damage children
    Health_Bar m_HealthBar;
    Damage_Bar m_DamageBar;
    Health_Container m_HealthContainer;
    bool m_bHealthDisplay;
    private float m_fHealth;
    private float m_fMaxHealth;
    //float[] m_fRegenerationRates;
    //float m_fRegenCountdown;
    //float m_fResetRegenTime;

    //Hunger
    //		float m_fFood;
    //		float m_fMaxFood;
    //		float m_fFoodDecayRate;
    //		public HungerLevel m_eHungerLevel;
    //		HungerLevel m_ePreviousHungerLevel;
    //Food_Display m_FoodDisplay;

    //WallClamping
    WallCollision m_eWallClamp;

    //Interact
    GameObject m_InteractTarget;
    Interact_Text_Container m_InteractContainer;
    private Vector3 m_vDamagedDirection;
    private bool m_bRecovering;
    private bool m_bGracePeriod;        //Time before we can get hit again
    private bool m_bStartGrace;
    private int m_nGraceFlashCount;
    private int m_nGraceFlashMaxCount;
    private float m_fMaxRecovery;
    private float m_fRecoveryTimer;
    private float m_fHalfMaxRecovery;
    private bool m_bEndPointCheck;
    private Vector3 m_vShadeCollPos;
    Vector3 m_vEndPoint;
    Vector3 m_vMidPoint;
    Vector3 m_vStartPoint;
    private bool m_bLerpKnockback;
    private float m_fKnockbackLerp;
    //Vector3 m_vHitPosition;     //place where we were hit
    //Vector3 m_vHitRecoveryLocation; //place we want to land at
    //float m_fRecoveryLerp;
    public bool m_bAttackDisabled;  //Flag for when we should not be able to attack

    //Room Management Info - Moved to pathfinder
    //public int m_nCurrentRoomID;         //current room this object is in
    //public IndoorDungeonGenerator m_IndoorDungeonGenerator;

    public struct customRect
    {
        public Vector2 _size;
        public Vector2 _center;

        public Vector2 size
        {
            get { return _size; }
            set { _size = value; }
        }

        public Vector2 center
        {
            get { return _center; }
            set { _center = value; }
        }
    };

    customRect[] collRects;

    ////Effects
    //Sword_Sparkle m_SwordSparkle;

    //Sound
    AudioSource audioSource;

    //SFX
    public AudioClip SFX_Footstep_Stone;
    public AudioClip SFX_Sword_Slash;
    public AudioClip SFX_Player_Hit;
    public AudioClip SFX_Breathing;
    bool m_bBreathingTrigger;

    //Animation SFX Trigger
    public void SFX_FootstepStone()
    {
        audioSource.PlayOneShot(SFX_Footstep_Stone);
    }

    public void SFX_SwordSlash()
    {
        audioSource.PlayOneShot(SFX_Sword_Slash);
    }

    public void SFX_PlayerHit()
    {
        audioSource.PlayOneShot(SFX_Player_Hit);
    }

    public void SFX_BreathingTrigger()
    {
        audioSource.PlayOneShot(SFX_Breathing);
    }
    //END AUDIO


    //Effects
    //public void StartSwordEmitter()
    //{
    //	m_SwordSparkle.SwordSparkle_Start ();
    //}

    //public void StopSwordEmitter()
    //{
    //	m_SwordSparkle.SwordSparkle_Stop ();
    //}
    //END EFFECTS

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material.EnableKeyword("_EMISSION");
        spriteRenderer.material.EnableKeyword("_ALBEDO");
        buttonQuit = GameObject.FindGameObjectWithTag("Button_Quit").GetComponent<Button_Quit>();
        buttonQuit.gameObject.SetActive(false);

        boxCollider = GetComponent<BoxCollider2D>();

        m_InteractContainer = GameObject.FindGameObjectWithTag("Interact_Container").GetComponent<Interact_Text_Container>();
        //m_SwordSparkle = GameObject.FindGameObjectWithTag ("Sword_Sparkle").GetComponent<Sword_Sparkle> ();
        SetType(Pathfinding_NavNodeAgentType.Player);

        m_nCurrentRoomID = 0;

        fMaxSpeed = .75f;
        m_vMove = new Vector2(0f, 0f);
        m_bAttackButton = false;
        m_bAttacking = false;

        collRects = new customRect[2];

        collRects[0].size = new Vector2(.1f, .16f);
        collRects[0].center = new Vector2(-.018f, -.047f);

        collRects[1].size = new Vector2(.09f, .16f);
        collRects[1].center = new Vector2(-.03f, -.047f);

        //m_BasicShadow = GameObject.FindGameObjectWithTag ("Basic_Shadow").GetComponent<Basic_Shadow>();
        m_BasicShadow = gameObject.GetComponentInChildren<Basic_Shadow>().GetComponent<Basic_Shadow>();
        m_BasicShadow.SetLocalY(-0.08884621f);

        m_HealthBar = GameObject.FindGameObjectWithTag("Health_Bar").GetComponent<Health_Bar>();
        m_DamageBar = GameObject.FindGameObjectWithTag("Damage_Bar").GetComponent<Damage_Bar>();
        m_HealthContainer = GameObject.FindGameObjectWithTag("Health_Container").GetComponent<Health_Container>();
        m_bHealthDisplay = false;

        m_fMaxHealth = m_fHealth = 100f;
        //m_HealthBar.SetHealth (m_fMaxHealth);
        //				m_fFood = 120f;
        //				m_fMaxFood = 200f;
        //				m_fFoodDecayRate = 1f;
        //				m_fRegenerationRates = new float[System.Enum.GetValues (typeof(HungerLevel)).Length];
        //				
        //
        //				m_fRegenerationRates [(int)HungerLevel.Starving] = -1.5f;
        //				m_fRegenerationRates [(int)HungerLevel.Hungry] = -.5f;
        //				m_fRegenerationRates [(int)HungerLevel.Satisfied] = 1f;
        //				m_fRegenerationRates [(int)HungerLevel.Full] = 2f;
        //				m_fRegenerationRates [(int)HungerLevel.Stuffed] = 3.5f;

        //m_ePreviousHungerLevel = m_eHungerLevel = HungerLevel.Satisfied;

        //m_fRegenCountdown = m_fResetRegenTime = 1f;

        m_bRecovering = false;

        m_fMaxRecovery = m_fRecoveryTimer = 3f;
        m_fHalfMaxRecovery = m_fMaxRecovery * .5f;

        m_bGracePeriod = false;
        m_bStartGrace = false;
        m_nGraceFlashMaxCount = m_nGraceFlashCount = 2;
        m_bBreathingTrigger = false;

        m_bEndPointCheck = false;
        m_vShadeCollPos = new Vector3(0f, 0f, 0f);
        m_vStartPoint = new Vector3(0f, 0f, 0f);
        m_vMidPoint = new Vector3(0f, 0f, 0f);
        m_vEndPoint = new Vector3(0f, 0f, 0f);

        m_bLerpKnockback = false;
        m_fKnockbackLerp = 0f;

        m_bAttackDisabled = false;
        m_InteractTarget = null;

        m_eWallClamp = WallCollision.Wall_None;

        //m_FoodDisplay = GameObject.FindGameObjectWithTag("Food_Display").GetComponent<Food_Display>();

        GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    /// <summary>
    /// Updates an ID that records which room the player is currently in for pathfinding
    /// </summary>
    void CheckRoom()
    {
        if (!m_IndoorDungeonGenerator)
            return;

        //first, check the room we're currently in, fast exit if we're still in that room
        if (PlayerIsInRoom(m_nCurrentRoomID))
            return;
        
        for(int nRoom = 0; nRoom < m_IndoorDungeonGenerator.m_Rooms.Count; nRoom++)
        {
            if (m_nCurrentRoomID == nRoom)
                continue;

            if(PlayerIsInRoom(nRoom))
            {
                m_nCurrentRoomID = nRoom;
                return;
            }
        }
    }

    bool PlayerIsInRoom(int nRoom)
    {
        return m_IndoorDungeonGenerator.m_Rooms[nRoom].PointInRoomTest(transform.position);      
    }

    void Update()
    {

        //Check the room
        CheckRoom();

        m_nPlayerState = anim.GetInteger("Player_State");

        //HungerCheck ();

        if (m_bRecovering)
            Recover();
        else if (m_bStartGrace)
            GraceFlash();

        if (CheckButtonInput())
            return;
    }

    // Update is called once per frame.  Using FixedUpdate for rigidbodies.  Not sure if Input should be done here...buttondown doesn't register properly here
    void FixedUpdate()
    {
        m_nPlayerState = anim.GetInteger("Player_State");

        if (m_bLerpKnockback)
        {
            LerpKnockBack();
            return;
        }

        SetCollisionType();

        PlayerStateResolution();

        if (m_bEndPointCheck)
        {
            FindEndPoint();
            return;
        }

        //Only move when we are not attacking
        if (!m_bAttacking)
            MovePlayer();
        else
            HoldPosition();

        //Camera Smooth Follow :D why here? because Fixed update can be called multiple times
        //m_Camera.CameraSync ();
    }

    void FindEndPoint()
    {
        //Target - tower
        m_vDamagedDirection = transform.position - m_vShadeCollPos;

        Vector2 vPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 vdirection = new Vector2(m_vDamagedDirection.x, m_vDamagedDirection.y);
        RaycastHit2D hit = Physics2D.Raycast(vPos, vdirection, m_vDamagedDirection.magnitude * 2.7f, (1 << LayerMask.NameToLayer("Walls")));

        if (hit.collider != null)
        {
            //we have a new endpoint
            //if hitpointx is greater than us, pad to the left, else pad to the right
            //if hit pointy is greater than us, pad to positive, else pad to negative
            //Include Padding for boxCollider2D
            if (hit.point.x > transform.position.x)
                m_vEndPoint.x = hit.point.x - boxCollider.size.x * .6f;
            else if (hit.point.x < transform.position.x)
                m_vEndPoint.x = hit.point.x + boxCollider.size.x * 1.1f;
            else
                m_vEndPoint.x = hit.point.x;

            if (hit.point.y > transform.position.y)
                m_vEndPoint.y = hit.point.y - boxCollider.size.y * .6f;
            else if (hit.point.y < transform.position.y)
                m_vEndPoint.y = hit.point.y + boxCollider.size.y * .9f;
            else
                m_vEndPoint.y = hit.point.y;

            Debug.Log("RayCast -> Wall Collision");
        }
        else
        {
            //m_vDamagedDirection = our endpoint
            m_vEndPoint = transform.position + m_vDamagedDirection * 2.7f;
        }
        Knockback();
    }

    //Handles moving the player and setting the running animation
    void MovePlayer()
    {
        //For Responsive movement, use GetAxisRaw.
        //Regular GetAxis smooths the values, resulting in acceleration style behavior
        float DPadX = Input.GetAxisRaw("DPad_X") * fMaxSpeed;
        float DPadY = Input.GetAxisRaw("DPad_Y") * fMaxSpeed;
        float LStickX = Input.GetAxisRaw("Horizontal") * fMaxSpeed;
        float LStickY = Input.GetAxisRaw("Vertical") * fMaxSpeed;
        if (DPadX != 0f || DPadY != 0f)
            m_vMove = new Vector2(DPadX, DPadY);
        else
            m_vMove = new Vector2(LStickX, LStickY);

        //m_vMove = new Vector2 (Input.GetAxisRaw ("Horizontal") * fMaxSpeed, Input.GetAxisRaw ("Vertical") * fMaxSpeed);

        //Determine which direction has the greater magnitude, then move either only on x or on y.
        if (Mathf.Abs(m_vMove.x) > Mathf.Abs(m_vMove.y))
            m_vMove.y = 0f;
        else
            m_vMove.x = 0f;

        //Removes the Variable Running Speed
        m_vMove.Normalize();

        m_vMove *= fMaxSpeed;



        //Input - Entirely done using velocity...wow!
        if (m_vMove.y < 0f)
            anim.SetInteger("Player_State", (int)pcAnim.PC_RunDown);
        else if (m_vMove.x < 0f)
            anim.SetInteger("Player_State", (int)pcAnim.PC_RunLeft);
        else if (m_vMove.y > 0f)
            anim.SetInteger("Player_State", (int)pcAnim.PC_RunUp);
        else if (m_vMove.x > 0f)
            anim.SetInteger("Player_State", (int)pcAnim.PC_RunRight);
        else
        {
            //stop immediately.  Prevents character sliding
            m_vMove.x = 0.0f;
            m_vMove.y = 0.0f;
            GetComponent<Rigidbody2D>().angularVelocity = 0.0f;

            //set standing still animation
            if (m_nPlayerState == (int)pcAnim.PC_RunDown)
                anim.SetInteger("Player_State", (int)pcAnim.PC_FaceDown);
            else if (m_nPlayerState == (int)pcAnim.PC_RunLeft)
                anim.SetInteger("Player_State", (int)pcAnim.PC_FaceLeft);
            else if (m_nPlayerState == (int)pcAnim.PC_RunUp)
                anim.SetInteger("Player_State", (int)pcAnim.PC_FaceUp);
            else if (m_nPlayerState == (int)pcAnim.PC_RunRight)
                anim.SetInteger("Player_State", (int)pcAnim.PC_FaceRight);
        }
        RayCastWallSafety();
        WallClampManager();

        GetComponent<Rigidbody2D>().velocity = m_vMove;

        InteractCheck();
    }

    //0 for vertical, 1 for horizontal
    void BodyCollision(spacialOrientation direction)
    {
        boxCollider.size = collRects[(int)direction].size;
        boxCollider.offset = collRects[(int)direction].center;
    }

    void SetCollisionType()
    {
        switch (m_nPlayerState)
        {
            case (int)pcAnim.PC_RunDown://down
                {
                    BodyCollision(spacialOrientation.vertical);
                    break;
                }
            case (int)pcAnim.PC_RunLeft://left
                {
                    BodyCollision(spacialOrientation.horizontal);
                    break;
                }
            case (int)pcAnim.PC_RunUp://up
                {
                    BodyCollision(spacialOrientation.vertical);
                    break;
                }
            case (int)pcAnim.PC_RunRight://right
                {
                    BodyCollision(spacialOrientation.horizontal);
                    break;
                }
            case (int)pcAnim.PC_FaceDown://down
                {
                    BodyCollision(spacialOrientation.vertical);
                    break;
                }
            case (int)pcAnim.PC_FaceLeft://left
                {
                    BodyCollision(spacialOrientation.horizontal);
                    break;
                }
            case (int)pcAnim.PC_FaceUp://up
                {
                    BodyCollision(spacialOrientation.vertical);
                    break;
                }
            case (int)pcAnim.PC_FaceRight://right
                {
                    BodyCollision(spacialOrientation.horizontal);
                    break;
                }
            case (int)pcAnim.PC_AttackDown://down
                {
                    BodyCollision(spacialOrientation.vertical);
                    break;
                }
            case (int)pcAnim.PC_AttackLeft://left
                {
                    BodyCollision(spacialOrientation.horizontal);
                    break;
                }
            case (int)pcAnim.PC_AttackRight://right
                {
                    BodyCollision(spacialOrientation.horizontal);
                    break;
                }
            case (int)pcAnim.PC_AttackUp://up
                {
                    BodyCollision(spacialOrientation.vertical);
                    break;
                }
        }
    }

    //Keeps The Player in Place (ex. during an attack animation)
    void HoldPosition()
    {
        m_vMove.x = 0.0f;
        m_vMove.y = 0.0f;
        GetComponent<Rigidbody2D>().angularVelocity = 0.0f;

        GetComponent<Rigidbody2D>().velocity = m_vMove;
    }

    //FlashRed
    void Recover()
    {

        if (m_fRecoveryTimer > m_fHalfMaxRecovery)
            spriteRenderer.material.SetColor("_EmissionColor", new Color(Mathf.Lerp(0.0f, .6f, 1 - ((m_fRecoveryTimer - m_fHalfMaxRecovery) / m_fHalfMaxRecovery)), 0f, 0f));
        else
            spriteRenderer.material.SetColor("_EmissionColor", new Color(Mathf.Lerp(0.6f, 0.0f, 1 - (m_fRecoveryTimer / m_fHalfMaxRecovery)), 0f, 0f));

        m_fRecoveryTimer -= 10f * Time.deltaTime;

        if (m_fRecoveryTimer <= 0f)
        {
            m_fRecoveryTimer = m_fMaxRecovery;
            m_bRecovering = false;

            if (m_fHealth <= 0f)
            {
                //don't do anything for now
                //death animation here..
                m_fHealth = m_fMaxHealth;
            }
        }
    }

    //FlashTransparent 2 times
    void GraceFlash()
    {

        if (m_bBreathingTrigger)
        {
            m_bBreathingTrigger = false;
            SFX_BreathingTrigger();
        }

        if (m_fRecoveryTimer > m_fHalfMaxRecovery)
            spriteRenderer.material.SetColor("_Color", new Color(1f, 1f, 1f, Mathf.Lerp(1f, .25f, 1 - ((m_fRecoveryTimer - m_fHalfMaxRecovery) / m_fHalfMaxRecovery))));
        else
            spriteRenderer.material.SetColor("_Color", new Color(1f, 1f, 1f, Mathf.Lerp(.25f, 1f, 1 - (m_fRecoveryTimer / m_fHalfMaxRecovery))));

        m_fRecoveryTimer -= 7f * Time.deltaTime;


        if (m_fRecoveryTimer <= 0f)
        {
            m_nGraceFlashCount -= 1;
            if (m_nGraceFlashCount <= 0)
            {
                m_bGracePeriod = false;
                m_bStartGrace = false;
                m_nGraceFlashCount = m_nGraceFlashMaxCount;
            }
            m_fRecoveryTimer = m_fMaxRecovery;

        }
    }

    void Knockback()
    {
        //first find our end point via raycast..then use this function
        //Quadratic Bezier curve
        //need 3 points
        //start
        //middle
        //end
        //B(t) = (1-t)^2 P_0 + 2(1-t)t P_1 + t^2 P_2
        //if end point goes through a wall, use raycast to find collision point...That's our new end point

        //find start
        m_vStartPoint = transform.position;
        //find middle
        m_vMidPoint = new Vector3((m_vStartPoint.x + m_vEndPoint.x) * .5f, (m_vStartPoint.y + m_vEndPoint.y) * .5f + .2f, 0f);

        m_bLerpKnockback = true;
        m_bAttackDisabled = true;
        //gameObject.collider2D.enabled = false;
        boxCollider.isTrigger = true;
    }

    void LerpKnockBack()
    {
        transform.position = ((1f - m_fKnockbackLerp) * (1f - m_fKnockbackLerp) * m_vStartPoint) + (2f * (1f - m_fKnockbackLerp) * m_fKnockbackLerp * m_vMidPoint) + (m_fKnockbackLerp * m_fKnockbackLerp * m_vEndPoint);

        //Rescale the Shadow, pass in knockbacklerp
        m_BasicShadow.Resize(m_fKnockbackLerp);


        if (m_fKnockbackLerp >= 1f)
        {
            m_bLerpKnockback = false;
            m_bEndPointCheck = false;
            m_bAttackDisabled = false;
            //gameObject.collider2D.enabled = true;
            boxCollider.isTrigger = false;
            m_fKnockbackLerp = 0f;
            m_bStartGrace = true;

            ResetWallClamp();
            return;
        }
        m_fKnockbackLerp += 1f * Time.deltaTime;
    }

    //Called by shades that bump into the player
    void Shade_Collide(float damage)
    {
        if (m_bGracePeriod)
            return;

        //if a shade runs into us...
        Debug.Log("Player1 run into by Shade", gameObject);

        //take damage
        m_fHealth -= damage;

        //Play SFX for PlayerHit
        SFX_PlayerHit();

        if (m_fHealth <= 0f)
            m_fHealth = 0f;


        //recover - flash red
        m_bRecovering = true;

        //then flash transparent
        m_bGracePeriod = true;
        m_bBreathingTrigger = true;


        //.5 second grace period before we can be hit again
        //show and set health and damage bars
        ReportDamage(damage);

        InteractOff();
    }

    void Shade_Pushback(Vector3 shadePosition)
    {
        if (m_bGracePeriod)
            return;
        //cast a ray - For Knockback
        m_bEndPointCheck = true;

        m_vShadeCollPos = shadePosition;
    }

    void ReportDamage(float fDamage)
    {
        //how much damage did we lose
        float fDamagePercentage = fDamage * .01f;
        float fHBLocation = m_fHealth * .01f;
        m_HealthBar.SetHealth(fHBLocation);
        m_DamageBar.SetStartingDamage(fDamagePercentage, fHBLocation);

        if (!m_HealthContainer.GetTransitioning() && !m_HealthContainer.GetDisplaying())
        {
            m_HealthContainer.FadeInAll();
        }
        else if (m_HealthContainer.GetTransitioning() && !m_HealthContainer.GetDisplaying())
        {
            m_HealthContainer.InterruptFadeOut();
        }
        m_HealthContainer.SetAutoHideTimerOn();

    }

    //Check for Gamepad Buttons and change player state accordingly
    bool CheckButtonInput()
    {
        if (Input.GetButtonDown("PS4PadButton") || Input.GetKeyDown("escape"))
        {
            buttonQuit.gameObject.SetActive(!buttonQuit.gameObject.activeSelf);
        }
        //If we are not attacking / In Grace Period / The attack button isn't already pressed / and the attack button is pressed, set the attack button to true
        if (!m_bAttackDisabled && !m_bAttacking && !m_bAttackButton && (Input.GetButtonDown("Attack") || Input.GetKeyDown("space")))
            m_bAttackButton = true;

        //temp for health display
        if (Input.GetButtonDown("L1"))
        {
            //currently toggling Health Display on button press
            m_bHealthDisplay = !m_bHealthDisplay;
            m_HealthContainer.InterruptAutoHide();

            if (!m_HealthContainer.GetTransitioning())
            {
                if (!m_HealthContainer.GetDisplaying())
                {
                    //Manual Fade in Health
                    m_HealthContainer.FadeInAll();
                }
                else
                {
                    //fade out
                    m_HealthContainer.FadeOutAll();
                }
            }
        }

        //Interact - example:Pick up Food, Talk to NPC, open chest, push button/switch
        if (!m_bAttackDisabled && (Input.GetButtonDown("Interact") || (Input.GetKeyDown("x"))))
        {
            //Interact
            //If we are interacting

            if (m_InteractTarget != null)
            {
                Debug.Log("Our Interact Target: ", m_InteractTarget);
                Debug.Log(m_InteractTarget.GetComponent<Interact_ReturnType>().m_InteractTypeID);
                //Get Interact Behavior of our target game object interactable
                switch (m_InteractTarget.GetComponent<Interact_ReturnType>().m_InteractTypeID)
                {
                    case InteractType.InteractConsumable:
                        {
                            m_InteractTarget.GetComponent<Interact_Consumable>().Interact();
                            break;
                        }
                    case InteractType.InteractChest:
                        {
                            m_InteractTarget.GetComponent<Treasure_Chest>().Interact();
                            break;
                        }
                }
            }
        }
        return false;
    }

    void InteractOn(GameObject interactableObj)
    {
        m_InteractTarget = interactableObj;
    }

    void InteractOff()
    {
        m_InteractContainer.SendMessage("Use_Message_Off");
        m_InteractTarget = null;

    }

    //		public void Food (float fFoodIncrease)
    //		{
    //				m_fFood += fFoodIncrease;
    //				//need to add a visual display for this including what the food was
    //		}

    //		void HungerCheck ()
    //		{
    //				m_fFood -= m_fFoodDecayRate * Time.deltaTime;
    //				if (m_fFood <= 0f)
    //						m_fFood = 0f;
    //			
    //				if (m_fFood >= 200f)
    //						m_fFood = 200f;
    //
    //				//update hunger level
    //				float fFoodPercent = m_fFood / m_fMaxFood;
    //				if (fFoodPercent > .8f)
    //						m_eHungerLevel = HungerLevel.Stuffed;
    //				else if (fFoodPercent <= .8f && fFoodPercent > .6f)
    //						m_eHungerLevel = HungerLevel.Full;
    //				else if (fFoodPercent <= .6f && fFoodPercent > .4f)
    //						m_eHungerLevel = HungerLevel.Satisfied;
    //				else if (fFoodPercent <= .4f && fFoodPercent > .2f)
    //						m_eHungerLevel = HungerLevel.Hungry;
    //				else if (fFoodPercent <= .2f)
    //						m_eHungerLevel = HungerLevel.Starving;
    //
    //				if (m_ePreviousHungerLevel != m_eHungerLevel) {
    //						Debug.Log (m_eHungerLevel);
    //						m_ePreviousHungerLevel = m_eHungerLevel;
    //						//Update our Food Display
    //						m_FoodDisplay.DisplayHunger();
    //				}
    //				RegenerateHealth ();
    //		}

    //		Based on our Hunger Level we regenerate X amount of health
    //		starving -4
    //		hungry -2
    //		satisfied +2
    //		full +4
    //		stuffed +5
    //		void RegenerateHealth ()
    //		{
    //				//every second, step the health
    //				m_fRegenCountdown -= Time.deltaTime;
    //
    //				if (m_fRegenCountdown <= 0f) {
    //						m_fRegenCountdown = m_fResetRegenTime;
    //
    //						m_fHealth += m_fRegenerationRates [(int)m_eHungerLevel];
    //					
    //						if (m_fHealth > m_fMaxHealth)
    //								m_fHealth = m_fMaxHealth;
    //
    //						float fHBLocation = m_fHealth * .01f;
    //						m_HealthBar.SetHealth (fHBLocation);
    //				}
    //		}

    void PlayerStateResolution()
    {
        //if we are not attacking and the attack button is pressed, based on our player state, set the correct attacking animation.
        if (!m_bAttacking && m_bAttackButton)
        {

            //reset attack button, we are now attacking
            m_bAttackButton = false;
            m_bAttacking = true;

            switch (m_nPlayerState)
            {
                case (int)pcAnim.PC_RunDown://rundown
                    {
                        anim.SetInteger("Player_State", (int)pcAnim.PC_AttackDown);
                        HoldPosition();
                        return;
                    }
                case (int)pcAnim.PC_FaceDown:
                    {
                        anim.SetInteger("Player_State", (int)pcAnim.PC_AttackDown);
                        HoldPosition();
                        return;
                    }
                case (int)pcAnim.PC_RunLeft://runleft
                    {
                        anim.SetInteger("Player_State", (int)pcAnim.PC_AttackLeft);
                        HoldPosition();
                        return;
                    }
                case (int)pcAnim.PC_FaceLeft:
                    {
                        anim.SetInteger("Player_State", (int)pcAnim.PC_AttackLeft);
                        HoldPosition();
                        return;
                    }
                case (int)pcAnim.PC_RunRight://runright
                    {
                        anim.SetInteger("Player_State", (int)pcAnim.PC_AttackRight);
                        HoldPosition();
                        return;
                    }
                case (int)pcAnim.PC_FaceRight:
                    {
                        anim.SetInteger("Player_State", (int)pcAnim.PC_AttackRight);
                        HoldPosition();
                        return;
                    }
                case (int)pcAnim.PC_RunUp://runup
                    {
                        anim.SetInteger("Player_State", (int)pcAnim.PC_AttackUp);
                        HoldPosition();
                        return;
                    }
                case (int)pcAnim.PC_FaceUp:
                    {
                        anim.SetInteger("Player_State", (int)pcAnim.PC_AttackUp);
                        HoldPosition();
                        return;
                    }
            }
        }
    }

    //Set the Player State back to one of the Facing States
    void EndAttack()
    {
        m_bAttacking = false;

        switch (m_nPlayerState)
        {
            case (int)pcAnim.PC_AttackDown://attackdown
                {
                    anim.SetInteger("Player_State", (int)pcAnim.PC_FaceDown);
                    HoldPosition();
                    break;
                }
            case (int)pcAnim.PC_AttackLeft://attackleft
                {
                    anim.SetInteger("Player_State", (int)pcAnim.PC_FaceLeft);
                    HoldPosition();
                    break;
                }
            case (int)pcAnim.PC_AttackRight://attackright
                {
                    anim.SetInteger("Player_State", (int)pcAnim.PC_FaceRight);
                    HoldPosition();
                    break;
                }
            case (int)pcAnim.PC_AttackUp://attackup
                {
                    anim.SetInteger("Player_State", (int)pcAnim.PC_FaceUp);
                    HoldPosition();
                    break;
                }
        }
    }

    //Raycast Check for Interactable object
    void InteractCheck()
    {
        //Interaction Check - Find our Foward Ray

        float fDistance = .07f; //slightly further than our wallraycasts below...
        Vector2 vDirecton = Vector2.zero;
        Vector2 vStartPosition = Vector2.zero;

        switch (m_nPlayerState)
        {
            case (int)pcAnim.PC_FaceUp:
                {
                    vDirecton = Vector2.up;
                    vStartPosition = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y + boxCollider.bounds.extents.y);
                    break;
                }
            case (int)pcAnim.PC_RunUp:
                {
                    vDirecton = Vector2.up;
                    vStartPosition = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y + boxCollider.bounds.extents.y);
                    break;
                }
            case (int)pcAnim.PC_AttackUp:
                {
                    vDirecton = Vector2.up;
                    vStartPosition = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y + boxCollider.bounds.extents.y);
                    break;
                }
            case (int)pcAnim.PC_FaceLeft:
                {
                    vDirecton = new Vector2(-1f, 0f);
                    vStartPosition = new Vector2(boxCollider.bounds.center.x - boxCollider.bounds.extents.x, boxCollider.bounds.center.y);
                    break;
                }

            case (int)pcAnim.PC_RunLeft:
                {
                    vDirecton = new Vector2(-1f, 0f);
                    vStartPosition = new Vector2(boxCollider.bounds.center.x - boxCollider.bounds.extents.x, boxCollider.bounds.center.y);
                    break;
                }
            case (int)pcAnim.PC_AttackLeft:
                {
                    vDirecton = new Vector2(-1f, 0f);
                    vStartPosition = new Vector2(boxCollider.bounds.center.x - boxCollider.bounds.extents.x, boxCollider.bounds.center.y);
                    break;
                }
            case (int)pcAnim.PC_FaceRight:
                {
                    vDirecton = Vector2.right;
                    vStartPosition = new Vector2(boxCollider.bounds.center.x + boxCollider.bounds.extents.x, boxCollider.bounds.center.y);
                    break;
                }
            case (int)pcAnim.PC_RunRight:
                {
                    vDirecton = Vector2.right;
                    vStartPosition = new Vector2(boxCollider.bounds.center.x + boxCollider.bounds.extents.x, boxCollider.bounds.center.y);
                    break;
                }
            case (int)pcAnim.PC_AttackRight:
                {
                    vDirecton = Vector2.right;
                    vStartPosition = new Vector2(boxCollider.bounds.center.x + boxCollider.bounds.extents.x, boxCollider.bounds.center.y);
                    break;
                }
            case (int)pcAnim.PC_FaceDown:
                {
                    vDirecton = new Vector2(0f, -1f);
                    vStartPosition = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y - boxCollider.bounds.extents.y);
                    break;
                }
            case (int)pcAnim.PC_RunDown:
                {
                    vDirecton = new Vector2(0f, -1f);
                    vStartPosition = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y - boxCollider.bounds.extents.y);
                    break;
                }
            case (int)pcAnim.PC_AttackDown:
                {
                    vDirecton = new Vector2(0f, -1f);
                    vStartPosition = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y - boxCollider.bounds.extents.y);
                    break;
                }
        }
        RaycastHit2D ourRay = Physics2D.Raycast(vStartPosition, vDirecton, fDistance, (1 << LayerMask.NameToLayer("Walls")));

        if (ourRay.collider != null)
        {
            if (ourRay.collider.gameObject.tag == "Treasure_Chest")
            {
                m_InteractContainer.SendMessage("Use_Message_On", InteractMessageType.Open_Treasure_Chest);
                InteractOn(ourRay.collider.gameObject);

            }
            else if (m_InteractTarget != null)
                InteractOff();
        }
        else
            InteractOff();
    }

    void RayCastWallSafety()
    {
        //TODO: Use Raycasting 1 up/left/right/down to keep us 1 pixel away from walls to prevent the player -> enemy collision issue.  Try using this for Chest checks also.

        //Must find All these points first
        WallCollision eWallCollision = m_eWallClamp;

        float fCheckDistance = .02f;
        Vector3 vReposition = Vector3.zero;

        Vector2 vBoxTopCenter = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y + boxCollider.bounds.extents.y);
        Vector2 vUp = Vector2.up;
        RaycastHit2D hitUp = Physics2D.Raycast(vBoxTopCenter, vUp, fCheckDistance, (1 << LayerMask.NameToLayer("Walls")));

        if (hitUp.collider != null)
        {
            vReposition.y = -fCheckDistance;
            //Debug.Log (vReposition);
            eWallCollision = WallCollision.Wall_Above;
        }

        Vector2 vBoxDownCenter = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y - boxCollider.bounds.extents.y);
        Vector2 vDown = new Vector2(0f, -1f);
        RaycastHit2D hitDown = Physics2D.Raycast(vBoxDownCenter, vDown, fCheckDistance, (1 << LayerMask.NameToLayer("Walls")));

        if (hitDown.collider != null)
        {
            vReposition.y = fCheckDistance;
            //Debug.Log (vReposition);
            eWallCollision = WallCollision.Wall_Below;
        }

        Vector2 vBoxLeftCenter = new Vector2(boxCollider.bounds.center.x - boxCollider.bounds.extents.x, boxCollider.bounds.center.y);
        Vector2 vLeft = new Vector2(-1f, 0f);
        RaycastHit2D hitLeft = Physics2D.Raycast(vBoxLeftCenter, vLeft, fCheckDistance, (1 << LayerMask.NameToLayer("Walls")));

        if (hitLeft.collider != null)
        {
            vReposition.x = fCheckDistance;
            //Debug.Log (vReposition);
            if (eWallCollision == WallCollision.Wall_Above)
                eWallCollision = WallCollision.Wall_Corner_LeftTop;
            else if (eWallCollision == WallCollision.Wall_Below)
                eWallCollision = WallCollision.Wall_Corner_LeftBottom;
            else
                eWallCollision = WallCollision.Wall_Left;
        }

        Vector2 vBoxRightCenter = new Vector2(boxCollider.bounds.center.x + boxCollider.bounds.extents.x, boxCollider.bounds.center.y);
        Vector2 vRight = Vector2.right;
        RaycastHit2D hitRight = Physics2D.Raycast(vBoxRightCenter, vRight, fCheckDistance, (1 << LayerMask.NameToLayer("Walls")));

        if (hitRight.collider != null)
        {
            vReposition.x = -fCheckDistance;
            //Debug.Log (vReposition);

            if (eWallCollision == WallCollision.Wall_Above)
                eWallCollision = WallCollision.Wall_Corner_RightTop;
            else if (eWallCollision == WallCollision.Wall_Below)
                eWallCollision = WallCollision.Wall_Corner_RightBottom;
            eWallCollision = WallCollision.Wall_Right;
        }

        transform.position += vReposition;

        SetWallClamp(eWallCollision);
    }

    void SetWallClamp(WallCollision ourClamp)
    {
        m_eWallClamp = ourClamp;
    }

    void ResetWallClamp()
    {
        m_eWallClamp = WallCollision.Wall_None;
    }

    void WallClampManager()
    {
        switch (m_eWallClamp)
        {
            case WallCollision.Wall_Below:
                {
                    if (m_nPlayerState == (int)pcAnim.PC_FaceDown || m_nPlayerState == (int)pcAnim.PC_RunDown)
                        m_vMove.y = 0f;
                    else
                        m_eWallClamp = WallCollision.Wall_None;
                    break;
                }
            case WallCollision.Wall_Above:
                {
                    if (m_nPlayerState == (int)pcAnim.PC_FaceUp || m_nPlayerState == (int)pcAnim.PC_RunUp)
                        m_vMove.y = 0f;
                    else
                        m_eWallClamp = WallCollision.Wall_None;
                    break;
                }
            case WallCollision.Wall_Left:
                {
                    if (m_nPlayerState == (int)pcAnim.PC_FaceLeft || m_nPlayerState == (int)pcAnim.PC_RunLeft)
                        m_vMove.x = 0f;
                    else
                        m_eWallClamp = WallCollision.Wall_None;
                    break;
                }
            case WallCollision.Wall_Right:
                {
                    if (m_nPlayerState == (int)pcAnim.PC_FaceRight || m_nPlayerState == (int)pcAnim.PC_RunRight)
                        m_vMove.x = 0f;
                    else
                        m_eWallClamp = WallCollision.Wall_None;
                    break;
                }
            case WallCollision.Wall_Corner_LeftTop:
                {
                    if (m_nPlayerState == (int)pcAnim.PC_FaceLeft || m_nPlayerState == (int)pcAnim.PC_RunLeft)
                        m_vMove.x = 0f;
                    else if (m_nPlayerState == (int)pcAnim.PC_FaceUp || m_nPlayerState == (int)pcAnim.PC_RunUp)
                        m_vMove.y = 0f;
                    else
                        m_eWallClamp = WallCollision.Wall_None;
                    break;
                }
            case WallCollision.Wall_Corner_RightTop:
                {
                    if (m_nPlayerState == (int)pcAnim.PC_FaceRight || m_nPlayerState == (int)pcAnim.PC_RunRight)
                        m_vMove.x = 0f;
                    else if (m_nPlayerState == (int)pcAnim.PC_FaceUp || m_nPlayerState == (int)pcAnim.PC_RunUp)
                        m_vMove.y = 0f;
                    else
                        m_eWallClamp = WallCollision.Wall_None;
                    break;
                }
            case WallCollision.Wall_Corner_LeftBottom:
                {
                    if (m_nPlayerState == (int)pcAnim.PC_FaceLeft || m_nPlayerState == (int)pcAnim.PC_RunLeft)
                        m_vMove.x = 0f;
                    else if (m_nPlayerState == (int)pcAnim.PC_FaceDown || m_nPlayerState == (int)pcAnim.PC_RunDown)
                        m_vMove.y = 0f;
                    else
                        m_eWallClamp = WallCollision.Wall_None;
                    break;
                }
            case WallCollision.Wall_Corner_RightBottom:
                {
                    if (m_nPlayerState == (int)pcAnim.PC_FaceRight || m_nPlayerState == (int)pcAnim.PC_RunRight)
                        m_vMove.x = 0f;
                    else if (m_nPlayerState == (int)pcAnim.PC_FaceDown || m_nPlayerState == (int)pcAnim.PC_RunDown)
                        m_vMove.y = 0f;
                    else
                        m_eWallClamp = WallCollision.Wall_None;
                    break;
                }
        }
    }

    //Needed for a case on which the treasure chest is destroyed and the player is clamped to it
    void ForceClampOff()
    {
        m_eWallClamp = WallCollision.Wall_None;
    }
}
