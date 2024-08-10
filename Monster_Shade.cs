using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Monster_Shade : Pathfinding_NavnodeAgent
{

    Animator anim;
    SpriteRenderer spriteRenderer;
    //ParticleSystem partSystem;
    //List<CollisionLineRender> lineRenderers;
    //int m_nRendererLines;

    //public IndoorDungeonGenerator m_IndoorDungeonGenerator;

    //Regulate Turning Frequency
    private float m_fMaxSpeed;
    private float m_fTurnTimer;         //How frequent a shade can change directions, prevents diagonal movement
    private float m_fNextTurn;
    private float m_fRecoveryTimer;     //after being hit, how long before the shade continues moving again
    private float m_fMaxRecovery;
    private float m_fHalfMaxRecovery;
    private bool m_bTurn;
    private bool m_bRecovering;         //used for recovery
    private bool m_bSpawning;           //used for spawning
    private Vector3 m_vDirectionToMove;
    private Vector3 m_vSecondaryDirectionToMove;
    List<Direction> m_vDirectionOptions;  //used for collision avoidance to determine which direction to actually move in to reach the target while avoiding obstacles 
    public int m_nShadeState;
    private int m_nHealth;
    private int m_nMaxHealth;
    Vector2 m_vDamagedDirection;
    private bool m_bPushback;
    public bool m_bDead;                //only update if we're alive
    float m_fLookDistanceConstant;
    Vector2 m_vColliderOffset;
    Vector2 m_vColliderHalfSize;
    float m_fRayOffset;
    float m_fStartingPointRayOffset;

    int m_nSpawnerID;
    //public int m_nCurrentRoomID;         //current room this object is in

    //Sound
    AudioSource audioSource;
    //SFX
    public AudioClip SFX_Death;


    enum shadeAnim
    {
        Shade_RunDown = 0,
        Shade_RunLeft,
        Shade_RunUp,
        Shade_RunRight,
        Shade_Death,
        Shade_Spawn,
        Shade_Empty
    }
    ;

    //Animation SFX Trigger
    public void SFX_DeathAnim()
    {
        audioSource.PlayOneShot(SFX_Death);
    }

    // Use this for initialization
    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        anim = gameObject.GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        //m_nRendererLines = 12;
        m_fStartingPointRayOffset = .01f;
        m_fLookDistanceConstant = .16f; //half a tile
        m_vColliderOffset = GetComponent<BoxCollider2D>().offset;
        m_vColliderHalfSize = GetComponent<BoxCollider2D>().size * .5f;

        //lineRenderers = new List<CollisionLineRender>();
        ////create our linerenderers for debugging?
        //for (int nLines = 0; nLines < m_nRendererLines; nLines++)
        //{

        //    CollisionLineRender lineRenderToCreate = Instantiate(Resources.Load("Prefabs/CollisionLineRender", typeof(CollisionLineRender)), transform.position, Quaternion.identity) as CollisionLineRender;
        //    lineRenderToCreate.transform.parent = transform;
        //    lineRenderers.Add(lineRenderToCreate);
        //}
        //Debug.Log("Created " + lineRenderers.Count + " lines");
        //LineRenderer targetLineRenderer = lineRenderers[0].GetComponent<LineRenderer>();
        ////right
        ////middle ray
        //targetLineRenderer.SetPosition(0, new Vector3(m_vColliderOffset.x + m_vColliderHalfSize.x + m_fStartingPointRayOffset, 0f, 0f));
        //targetLineRenderer.SetPosition(1, new Vector3(m_vColliderOffset.x + m_vColliderHalfSize.x + m_fStartingPointRayOffset + m_fLookDistanceConstant, 0f, 0f));
        ////Edge 1: if up/down (left side) if left/right (bottom side)
        //targetLineRenderer = lineRenderers[1].GetComponent<LineRenderer>();
        //targetLineRenderer.SetPosition(0, new Vector3(m_vColliderOffset.x + m_vColliderHalfSize.x + m_fStartingPointRayOffset, -(m_vColliderOffset.y + m_vColliderHalfSize.y + m_fRayOffset), 0f));
        //targetLineRenderer.SetPosition(1, new Vector3(m_vColliderOffset.x + m_vColliderHalfSize.x + m_fStartingPointRayOffset + m_fLookDistanceConstant, -(m_vColliderOffset.y + m_vColliderHalfSize.y + m_fRayOffset), 0f));
        ////Edge 2: if up/down (right side) if left/right (top side)
        //targetLineRenderer = lineRenderers[2].GetComponent<LineRenderer>();
        //targetLineRenderer.SetPosition(0, new Vector3(m_vColliderOffset.x + m_vColliderHalfSize.x + m_fStartingPointRayOffset, (m_vColliderOffset.y + m_vColliderHalfSize.y + m_fRayOffset), 0f));
        //targetLineRenderer.SetPosition(1, new Vector3(m_vColliderOffset.x + m_vColliderHalfSize.x + m_fStartingPointRayOffset + m_fLookDistanceConstant, (m_vColliderOffset.y + m_vColliderHalfSize.y + m_fRayOffset), 0f));

        ////top
        ////middle ray
        //targetLineRenderer = lineRenderers[3].GetComponent<LineRenderer>();
        //targetLineRenderer.SetPosition(0, new Vector3(0f, m_vColliderOffset.y + m_vColliderHalfSize.y + m_fStartingPointRayOffset, 0f));
        //targetLineRenderer.SetPosition(1, new Vector3(0f, m_vColliderOffset.y + m_vColliderHalfSize.y + m_fStartingPointRayOffset + m_fLookDistanceConstant, 0f));
        ////Edge 1: if up/down (left side) if left/right (bottom side)
        //targetLineRenderer = lineRenderers[4].GetComponent<LineRenderer>();
        //targetLineRenderer.SetPosition(0, new Vector3(-(m_vColliderOffset.x + m_vColliderHalfSize.x + m_fRayOffset), m_vColliderOffset.y + m_vColliderHalfSize.y + m_fStartingPointRayOffset, 0f));
        //targetLineRenderer.SetPosition(1, new Vector3(-(m_vColliderOffset.x + m_vColliderHalfSize.x + m_fRayOffset), m_vColliderOffset.y + m_vColliderHalfSize.y + m_fStartingPointRayOffset + m_fLookDistanceConstant, 0f));
        ////Edge 2: if up/down (right side) if left/right (top side)
        //targetLineRenderer = lineRenderers[5].GetComponent<LineRenderer>();
        //targetLineRenderer.SetPosition(0, new Vector3((m_vColliderOffset.x + m_vColliderHalfSize.x + m_fRayOffset), m_vColliderOffset.y + m_vColliderHalfSize.y + m_fStartingPointRayOffset, 0f));
        //targetLineRenderer.SetPosition(1, new Vector3((m_vColliderOffset.x + m_vColliderHalfSize.x + m_fRayOffset), m_vColliderOffset.y + m_vColliderHalfSize.y + m_fStartingPointRayOffset + m_fLookDistanceConstant, 0f));

        //We are TYPE: Shade
        SetType(Pathfinding_NavNodeAgentType.Shade);
        m_fRayOffset = .06f;
        m_fMaxSpeed = .5f;
        m_fTurnTimer = .5f;
        m_fNextTurn = 0f;
        m_fMaxRecovery = m_fRecoveryTimer = .3f;
        m_fHalfMaxRecovery = m_fMaxRecovery * .5f;
        m_bTurn = true;
        m_bPushback = false;
        m_bRecovering = false;

        m_vDirectionOptions = new List<Direction>();
        for (int nCardinalDirection = 0; nCardinalDirection < 4; nCardinalDirection++)
            m_vDirectionOptions.Add(Direction.None);

        m_bDead = true;

        GetComponent<Rigidbody2D>().angularVelocity = 0.0f;

        m_nMaxHealth = m_nHealth = 3;

        m_vDamagedDirection = new Vector2(0f, 0f);

        

        //m_Target = GameObject.FindGameObjectWithTag("Player1").transform;
       
        m_Path = new Queue<NavNode>();
        m_nGoalPreviousRoomID = -1;

        spriteRenderer.material.EnableKeyword("_EMISSION");
        anim.SetInteger("Shade_State", (int)shadeAnim.Shade_Empty);
        //gameObject.SetActive (false);
    }

    public void SetSpawnID(int spawnID)
    {
        m_nSpawnerID = spawnID;
    }

    public void Spawn()
    {
        m_nHealth = m_nMaxHealth;
        m_bDead = false;
        m_bTurn = true;
        anim.enabled = true;
        anim.SetInteger("Shade_State", (int)shadeAnim.Shade_Spawn);
        gameObject.GetComponent<Rigidbody2D>().mass = 999;        
        m_bSpawning = true;
    }

    void SpawnComplete()
    {
        m_bSpawning = false;
        m_bDead = false;
        gameObject.GetComponent<Rigidbody2D>().mass = 1;
        m_IndoorDungeonGenerator.m_PathfindingManager.m_Objects.Add(GetComponent<Pathfinding_NavnodeAgent>());
        anim.SetInteger("Shade_State", (int)shadeAnim.Shade_Empty);
    }

    /// <summary>
    /// Updates an ID that records which room the player is currently in for pathfinding
    /// </summary>
    void CheckRoom()
    {
        if (!m_IndoorDungeonGenerator)
            return;

        //first, check the room we're currently in, fast exit if we're still in that room
        if (MonsterIsInRoom(m_nCurrentRoomID))
            return;

        for (int nRoom = 0; nRoom < m_IndoorDungeonGenerator.m_Rooms.Count; nRoom++)
        {
            if (m_nCurrentRoomID == nRoom)
                continue;

            if (MonsterIsInRoom(nRoom))
            {
                m_nCurrentRoomID = nRoom;
                return;
            }
        }
    }

    bool MonsterIsInRoom(int nRoom)
    {
        return m_IndoorDungeonGenerator.m_Rooms[nRoom].PointInRoomTest(transform.position);
    }

    void UpdateTarget()
    {
        if(m_Path.Count > 0)
        {
            //if we're less than one tile from the target node, let's switch to the next node
            if(Vector3.Distance(transform.position, m_Target.position) <= .1f)
            {
                m_Target = m_Path.Dequeue().transform;
            }
        }
        else
        {
            m_Target = m_TargetGoal;
        }
    }

    // Update is called once per frame
    void Update()
    {            
        m_nShadeState = anim.GetInteger("Shade_State");
       
        if (m_bDead == true)
            return;

        //If we reach this point, we are not spawning, update our room
        CheckRoom();

        //set our target, if there's something in m_Path, update our path
        UpdateTarget();

        //Regulate how often we're allowed to turn
        if (!m_bTurn)
        {
            WaitToTurn();
        }

        if (m_bRecovering)
        {
            Recover();
        }
    }

    //For use on Rigidbody2D movement
    void FixedUpdate()
    {
        m_nShadeState = anim.GetInteger("Shade_State");

        if (m_bSpawning == true)
        {
            if (m_nShadeState == (int)shadeAnim.Shade_Empty)
                anim.SetInteger("Shade_State", (int)shadeAnim.Shade_Spawn);
            return;
        }

        if (m_bDead == true)
            return;
        //if we're spawning do nothing
        if (m_bSpawning == true)
            return;

        if (m_bPushback)
        {
            m_bPushback = false;
            GetComponent<Rigidbody2D>().AddForce(m_vDamagedDirection, ForceMode2D.Impulse);
            return;
        }

        if (!m_bRecovering && m_nShadeState != (int)shadeAnim.Shade_Death)
            PlotPath();
    }

    /// <summary>
    /// The purpose of this function is to decide what the shade's target is, the player? or a navigation node that will put the shade on a path to the player's room
    /// </summary>
    void Navigation()
    {
        //OK, time for real pathfinding...
        //Step 1: Check to see if the player is in the shade's room
        //The player should always know which room he's in...
        //Every shade needs to know which room it's in also
        //How To do this:
        //IndoorDungeonGenerator has a ReturnRoomLocation Function that tells the gameobject which room it is in
        //A way to speed this up is to start by checking to see if it's still in the same room that it already was in!!!!!
        //Also when an object is placed, we should just go ahead and assign its room number right there

    }

    /// <summary>
    /// True if we ran into something
    /// </summary>
    /// <param name="directionToCheck"></param>
    /// <returns></returns>
    bool CheckCollisionAvoidanceDirection(Direction directionToCheck)
    {

        Vector2 vDirection = Vector2.zero;
        Vector2 vPosition = transform.position;
        //Middle Ray
        switch (directionToCheck)
        {
            case Direction.Up:
                {
                    vDirection.y = 1f;
                    vPosition.y += (m_vColliderOffset.y + m_vColliderHalfSize.y + m_fStartingPointRayOffset);
                    break;
                }
            case Direction.Left:
                {
                    vDirection.x = -1f;
                    vPosition.x -= (m_vColliderOffset.x + m_vColliderHalfSize.x + m_fStartingPointRayOffset);
                    break;
                }
            case Direction.Right:
                {
                    vDirection.x = 1f;
                    vPosition.x += (m_vColliderOffset.x + m_vColliderHalfSize.x + m_fStartingPointRayOffset);
                    break;
                }
            case Direction.Down:
                {
                    vDirection.y = -1f;
                    vPosition.y -= (m_vColliderOffset.y + m_vColliderHalfSize.y + m_fStartingPointRayOffset);
                    break;
                }
        }
        RaycastHit2D hit = Physics2D.Raycast(vPosition, vDirection, m_fLookDistanceConstant, (1 << LayerMask.NameToLayer("Walls")));

        if (hit.collider != null)
        {
            return true;
        }

        //otherwise, check one of the edges of the agent
        vPosition = transform.position;
        //Edge 1: if up/down (left side) if left/right (bottom side)
        switch (directionToCheck)
        {
            case Direction.Up:
                {
                    vDirection.y = 1f;
                    vPosition.y += (m_vColliderOffset.y + m_vColliderHalfSize.y + m_fStartingPointRayOffset);

                    vPosition.x -= (m_vColliderOffset.x + m_vColliderHalfSize.x + m_fRayOffset);
                    break;
                }
            case Direction.Left:
                {
                    vDirection.x = -1f;
                    vPosition.x -= (m_vColliderOffset.x + m_vColliderHalfSize.x + m_fStartingPointRayOffset);

                    vPosition.y -= (m_vColliderOffset.y + m_vColliderHalfSize.y + m_fRayOffset);
                    break;
                }
            case Direction.Right:
                {
                    vDirection.x = 1f;
                    vPosition.x += (m_vColliderOffset.x + m_vColliderHalfSize.x + m_fStartingPointRayOffset);

                    vPosition.y -= (m_vColliderOffset.y + m_vColliderHalfSize.y + m_fRayOffset);
                    break;
                }
            case Direction.Down:
                {
                    vDirection.y = -1f;
                    vPosition.y -= (m_vColliderOffset.y + m_vColliderHalfSize.y + m_fStartingPointRayOffset);

                    vPosition.x -= (m_vColliderOffset.x + m_vColliderHalfSize.x + m_fRayOffset);
                    break;
                }
        }

        hit = Physics2D.Raycast(vPosition, vDirection, m_fLookDistanceConstant, (1 << LayerMask.NameToLayer("Walls")));

        if (hit.collider != null)
        {
            //then we need to change direction and we can exit early
            return true;
        }
        //otherwise, check one of the edges of the agent
        //reset our variables
        vPosition = transform.position;

        //Edge 2: if up/down (right side) if left/right (top side)
        switch (directionToCheck)
        {
            case Direction.Up:
                {
                    vDirection.y = 1f;
                    vPosition.y += (m_vColliderOffset.y + m_vColliderHalfSize.y + m_fStartingPointRayOffset);

                    vPosition.x += (m_vColliderOffset.x + m_vColliderHalfSize.x + m_fRayOffset);
                    break;
                }
            case Direction.Left:
                {
                    vDirection.x = -1f;
                    vPosition.x -= (m_vColliderOffset.x + m_vColliderHalfSize.x + m_fStartingPointRayOffset);

                    vPosition.y += (m_vColliderOffset.y + m_vColliderHalfSize.y + m_fRayOffset);
                    break;
                }
            case Direction.Right:
                {
                    vDirection.x = 1f;
                    vPosition.x += (m_vColliderOffset.x + m_vColliderHalfSize.x + m_fStartingPointRayOffset);

                    vPosition.y += (m_vColliderOffset.y + m_vColliderHalfSize.y + m_fRayOffset);
                    break;
                }
            case Direction.Down:
                {
                    vDirection.y = -1f;
                    vPosition.y -= (m_vColliderOffset.y + m_vColliderHalfSize.y + m_fStartingPointRayOffset);

                    vPosition.x += (m_vColliderOffset.x + m_vColliderHalfSize.x + m_fRayOffset);
                    break;
                }
        }

        hit = Physics2D.Raycast(vPosition, vDirection, m_fLookDistanceConstant, (1 << LayerMask.NameToLayer("Walls")));

        if (hit.collider != null)
        {
            //then we need to change direction and we can exit early
            return true;
        }
        return false;
    }

    /// <summary>
    /// cast 3 rays in our desired direction, if any of them hits something, move in the next best direction
    /// return: index of safe direction (4 direction options - 0,1,2,3)
    /// </summary>
    int CollisionAvoidance()
    {

        //Direction desiredDirection = Direction.None;

        if (m_vDirectionToMove.x > 0f) //face right
        {
            m_vDirectionOptions[0] = Direction.Right;
            m_vDirectionOptions[2] = Direction.Left;
        }
        else if (m_vDirectionToMove.x < 0f)//face left
        {
            m_vDirectionOptions[0] = Direction.Left;
            m_vDirectionOptions[2] = Direction.Right;
        }
        else if (m_vDirectionToMove.y > 0f)//face up
        {
            m_vDirectionOptions[0] = Direction.Up;
            m_vDirectionOptions[2] = Direction.Down;
        }
        else if (m_vDirectionToMove.y < 0f)//face down
        {
            m_vDirectionOptions[0] = Direction.Down;
            m_vDirectionOptions[2] = Direction.Up;
        }

        //First Option
        if (CheckCollisionAvoidanceDirection(m_vDirectionOptions[0]))
        {
            //we need to check a new direction
            //use the secondary direction
            if (m_vSecondaryDirectionToMove.x > 0f) //face right
            {
                m_vDirectionOptions[1] = Direction.Right;
                m_vDirectionOptions[3] = Direction.Left;
            }
            else if (m_vSecondaryDirectionToMove.x < 0f)//face left
            {
                m_vDirectionOptions[1] = Direction.Left;
                m_vDirectionOptions[3] = Direction.Right;
            }
            else if (m_vSecondaryDirectionToMove.y > 0f)//face up
            {
                m_vDirectionOptions[1] = Direction.Up;
                m_vDirectionOptions[3] = Direction.Down;
            }
            else if (m_vSecondaryDirectionToMove.y < 0f)//face down
            {
                m_vDirectionOptions[1] = Direction.Down;
                m_vDirectionOptions[3] = Direction.Up;
            }
         
            //Second Option
            if(CheckCollisionAvoidanceDirection(m_vDirectionOptions[1]))
            {
                //we need to check for a third option
                //already set opposite direction to option 0
                //Third Option
                if(CheckCollisionAvoidanceDirection(m_vDirectionOptions[2]))
                {
                    //we need to check option 4
                    //Fourth Option - Final
                    if(CheckCollisionAvoidanceDirection(m_vDirectionOptions[3]))
                    {
                        //can't move anywhere...need to tell this unit to stand still 
                        m_vDirectionToMove = Vector3.zero;
                    }
                    else
                    {
                        //safe to move in m_vDirectionOptions[3]
                        return 3;

                    }
                }
                else
                {
                    //safe to move in m_vDirectionOptions[2]
                    return 2;
                }
            }
            else
            {
                //safe to move in m_vDirectionOptions[1]
                return 1;
            }
        }
        else
        {
            //safe to move in m_vDirectionOptions[0]
            return 0;
        }
        return -1; //something went wrong...
    }
    //Create a path to the Target that avoids obstacles
    void PlotPath()
    {
        //Find the furthest collision first, place a point that creates a line segment that goes around the obstacle, 
        //then check the new line segments to make sure that it avoids all obstacles. Always furthest line segment first.

        //It's okay to change directions now
        if (m_bTurn)
        {
            //Target - tower
            m_vDirectionToMove = m_Target.position - transform.position;
            m_vSecondaryDirectionToMove = Vector3.zero;

            if (Mathf.Abs(m_vDirectionToMove.x) > Mathf.Abs(m_vDirectionToMove.y))
            {               
                m_vSecondaryDirectionToMove.y = m_vDirectionToMove.y;
                m_vDirectionToMove.y = 0f;
            }
            else
            {
                m_vSecondaryDirectionToMove.x = m_vDirectionToMove.x;
                m_vDirectionToMove.x = 0f;
            }

            //after complicated stuff, move
            m_vDirectionToMove.Normalize();
            m_vSecondaryDirectionToMove.Normalize();

            m_vDirectionToMove *= m_fMaxSpeed;
            m_vSecondaryDirectionToMove *= m_fMaxSpeed;

            //wait before changing directions
            m_bTurn = false;

            //we have our desired direction
            //now we have to check for collision avoidance.  If something's in the way, we need to alter our direction
            int nChosenDirection = CollisionAvoidance();
            if (nChosenDirection != -1)
            {
                switch (m_vDirectionOptions[nChosenDirection])
                {
                    case Direction.Left:
                        {
                            m_vDirectionToMove = Vector3.zero;
                            m_vDirectionToMove.x = -1f;
                            m_vDirectionToMove.Normalize();
                            m_vDirectionToMove *= m_fMaxSpeed;
                            break;
                        }
                    case Direction.Down:
                        {
                            m_vDirectionToMove = Vector3.zero;
                            m_vDirectionToMove.y = -1f;
                            m_vDirectionToMove.Normalize();
                            m_vDirectionToMove *= m_fMaxSpeed;
                            break;
                        }
                    case Direction.Right:
                        {
                            m_vDirectionToMove = Vector3.zero;
                            m_vDirectionToMove.x = 1f;
                            m_vDirectionToMove.Normalize();
                            m_vDirectionToMove *= m_fMaxSpeed;
                            break;
                        }
                    case Direction.Up:
                        {
                            m_vDirectionToMove = Vector3.zero;
                            m_vDirectionToMove.y = 1f;
                            m_vDirectionToMove.Normalize();
                            m_vDirectionToMove *= m_fMaxSpeed;
                            break;
                        }
                }
            }
            SetAnimation();
        }

        GetComponent<Rigidbody2D>().velocity = m_vDirectionToMove * m_fMaxSpeed;
    }

    void SetAnimation()
    {
        if (m_vDirectionToMove.x > 0f) //face right
            anim.SetInteger("Shade_State", (int)shadeAnim.Shade_RunRight);
        else if (m_vDirectionToMove.x < 0f)//face left
            anim.SetInteger("Shade_State", (int)shadeAnim.Shade_RunLeft);
        else if (m_vDirectionToMove.y > 0f)//face up
            anim.SetInteger("Shade_State", (int)shadeAnim.Shade_RunUp);
        else if (m_vDirectionToMove.y < 0f)//face down
            anim.SetInteger("Shade_State", (int)shadeAnim.Shade_RunDown);
    }

    //listen for this collision function from the sword
    void Sword_hit(int damage)
    {
        //if we're dieing, spawning or recovering...don't do anything
        if (m_nShadeState == (int)shadeAnim.Shade_Death || m_bRecovering || m_nShadeState == (int)shadeAnim.Shade_Spawn)
            return;

        Debug.Log("Shade hit by Sword", gameObject);

        //lets do 4 things
        //1. subtract health
        m_nHealth -= damage;

        //3. pushback the shade
        PushBack();

        //4. After Recovery, use the death animation if health is 0..can probably trigger from animation keyframe
    }

    //Timer to prevent diagonal movement
    void WaitToTurn()
    {
        m_fNextTurn += Time.deltaTime;

        if (m_fTurnTimer <= m_fNextTurn)
        {
            m_fNextTurn = 0f;

            m_bTurn = true;
        }
    }

    //Timer to prevent shade movement until he recovers from being damaged
    void Recover()
    {
        //while we're recovering we want to lerp our material to red and then back to clear.

        //lerp to red for the first half off the timer...then lerp back to clear
        //				if (m_fRecoveryTimer > m_fHalfMaxRecovery)
        //						spriteRenderer.material.SetFloat ("_FlashAmount", Mathf.Lerp (0.0f, .6f, 1 - ((m_fRecoveryTimer - m_fHalfMaxRecovery) / m_fHalfMaxRecovery)));
        //				else
        //						spriteRenderer.material.SetFloat ("_FlashAmount", Mathf.Lerp (0.6f, 0.0f, 1 - (m_fRecoveryTimer / m_fHalfMaxRecovery)));


        if (m_fRecoveryTimer > m_fHalfMaxRecovery)
            spriteRenderer.material.SetColor("_EmissionColor", new Color(Mathf.Lerp(0.0f, .6f, 1 - ((m_fRecoveryTimer - m_fHalfMaxRecovery) / m_fHalfMaxRecovery)), 0f, 0f));

        else
            spriteRenderer.material.SetColor("_EmissionColor", new Color(Mathf.Lerp(0.6f, 0.0f, 1 - (m_fRecoveryTimer / m_fHalfMaxRecovery)), 0f, 0f));


        //				if (m_fRecoveryTimer > m_fHalfMaxRecovery)
        //						spriteRenderer.material.SetColor ("_Emission", new Color (Mathf.Lerp (0.0f, .6f, 1 - ((m_fRecoveryTimer - m_fHalfMaxRecovery) / m_fHalfMaxRecovery)), 0f, 0f));
        //				else
        //						spriteRenderer.material.SetColor ("_Emission", new Color (Mathf.Lerp (0.6f, 0.0f, 1 - (m_fRecoveryTimer / m_fHalfMaxRecovery)), 0f, 0f));
        //				if (m_fRecoveryTimer > m_fHalfMaxRecovery)
        //						spriteRenderer.material.color = new Color (Mathf.Lerp (0.0f, .6f, 1 - ((m_fRecoveryTimer - m_fHalfMaxRecovery) / m_fHalfMaxRecovery)), 0f, 0f);
        //				else
        //						spriteRenderer.material.color = new Color (Mathf.Lerp (0.6f, 0.0f, 1 - (m_fRecoveryTimer / m_fHalfMaxRecovery)), 0f, 0f);


        m_fRecoveryTimer -= Time.deltaTime;

        if (m_fRecoveryTimer <= 0f)
        {
            m_bRecovering = false;
            m_fRecoveryTimer = m_fMaxRecovery;

            //option: maybe turn off particles during hit
            //GetComponentInChildren<ParticleSystem> ().renderer.enabled = true;
            //GetComponentInChildren<ParticleSystem> ().Play ();

            if (m_nHealth <= 0)
            {
                HoldPosition();
                anim.SetInteger("Shade_State", (int)shadeAnim.Shade_Death);
                m_bDead = true;
            }
        }
    }

    //Direction to push the Shade in when it is damaged
    void PushBack()
    {
        //Target - tower
        m_vDamagedDirection = m_Target.position - transform.position;

        //after complicated stuff, move
        m_vDamagedDirection.Normalize();

        m_vDamagedDirection.x = -m_vDamagedDirection.x;
        m_vDamagedDirection.y = -m_vDamagedDirection.y;

        m_bPushback = true;
        m_bRecovering = true;
        //option: maybe turn off particles during hit
        //GetComponentInChildren<ParticleSystem> ().renderer.enabled = false;
        //GetComponentInChildren<ParticleSystem> ().Pause ();
        //GetComponentInChildren<ParticleSystem> ().Clear ();

    }

    void HoldPosition()
    {
        m_vDirectionToMove.x = 0.0f;
        m_vDirectionToMove.y = 0.0f;
        GetComponent<Rigidbody2D>().angularVelocity = 0.0f;

        GetComponent<Rigidbody2D>().velocity = m_vDirectionToMove;
    }

    void ResolveDeath()
    {

        //gameObject.SetActive (false);
        anim.SetInteger("Shade_State", (int)shadeAnim.Shade_Empty);
        transform.parent.gameObject.GetComponent<Spawner_Shade>().AddToRespawner(m_nSpawnerID);
        m_IndoorDungeonGenerator.m_PathfindingManager.m_Objects.Remove(GetComponent<Pathfinding_NavnodeAgent>());

    }

    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player1")
        {
            coll.gameObject.SendMessage("Shade_Pushback", transform.position);
            coll.gameObject.SendMessage("Shade_Collide", 25f);
        }
    }

}
