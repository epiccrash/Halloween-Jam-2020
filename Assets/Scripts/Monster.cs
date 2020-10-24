using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// the monster has three states
// MonsterState.ROAM and MonsterState.PERSUE
//
// in PERSUE, the monster is running towards the player and screaming.
// when PERSUE state is initiated, PersueBegin is called
//
// in ROAM, the monster randomly selects waypoints to walk to and then
// idles at them for a random range of time (defined by waypointIdleTimeRange).
// when ROAM state is intiated, RoamBegin is called
//
// in STARE, the monster is not moving and is staring at the player. if the player
// looks at the monster while in stare mode, PERSUE is automatically engaged

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class Monster : MonoBehaviour
{
    [Header("Visual Effects")]
    public GameObject monsterHead; // for animating head towards player

    [Header("Navigation")]
    [Range(0,50)]
    public float playerMaxVisibleDistance;
    public List<Transform> waypoints;
    public Vector2 waypointIdleTimeRange;
    public Camera monsterCam;
    [Range(0,5)]
    public float walkSpeed;
    [Range(0,5)]
    public float runSpeed;
    [Range(0.1f,5)]
    public float pursueLoseSightCooldown; // the amount of time in seconds that the monster waits
        // between losing sight of the player and returning back to roam mode
    Transform _currWaypoint;
    Plane[] _planes;
    Camera _playerCam;
    CapsuleCollider _collider;
    NavMeshAgent _agent;

    [Header("Damage parameters")]
    public float damageCooldown; // in seconds
    public int damageAmount; // player has 100 health total
    bool _cooldownDmg;

    public enum MonsterState
    {
        ROAM,
        PURSUE,
        STARE
    }

    public MonsterState state;

    // Animation
    Animator animator;

    private void Start()
    {
        _playerCam = Camera.main;
        _collider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        GoToWaypoint(Random.Range(0, waypoints.Count));

        _cooldownDmg = false;

        if (state == MonsterState.ROAM) RoamBegin();
        else PursueBegin();
    }

    // if we run into a waypoint, idle for a bit and then go to another waypoint
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Waypoint")
        {
            if (state == MonsterState.ROAM)
            {
                if (other.gameObject.transform.position == _currWaypoint.position)
                {
                    // idle for random amt of time and then move to next waypoint
                    IdleGoToWayPoint(Random.Range(waypointIdleTimeRange[0], waypointIdleTimeRange[1]), Random.Range(0, waypoints.Count));
                }
            }
        }
    }

    // damage the player if colliding with monster
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!_cooldownDmg)
            {
                DamagePlayer();
            }
        }
    }

    private void Update()
    {
        if (state == MonsterState.ROAM)
        {

            // if the player sees the monster, enter PERSUE
            if (PlayerCanSeeMonster() && MonsterCanSeePlayer())
                PursueBegin();

            // if monster sees player, enter STARE
            if (!PlayerCanSeeMonster() && MonsterCanSeePlayer())
                StareBegin();

        } else if (state == MonsterState.PURSUE)
        {
            GoToPlayer();

        } else if (state == MonsterState.STARE)
        {
            Stare();

            // if the player sees the monster, enter PERSUE
            if (PlayerCanSeeMonster() && MonsterCanSeePlayer())
                PursueBegin();

            if (!MonsterCanSeePlayer())
                RoamBegin();
        }

    }

    // ----------------------- Animation and Navigation -----------------------

    // called when state changes to PERSUE
    void PursueBegin()
    {
        Run();
        state = MonsterState.PURSUE;
        StartCoroutine("PursueCooldown");
        
    }

    // waits "pursueLoseSightCooldown" seconds after losing sight of the player
    // before entering roam mode again
    IEnumerator PursueCooldown()
    {
        float timer = pursueLoseSightCooldown;
        while (timer > 0)
        {
            if (MonsterCanSeePlayer())
                timer = pursueLoseSightCooldown;
            else
                timer -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        RoamBegin();
    }
    // called when state changes to ROAM
    void RoamBegin()
    {
        Walk();
        state = MonsterState.ROAM;
        GoToWaypoint(Random.Range(0, waypoints.Count));
    }

    // called when state changes to STARE
    void StareBegin()
    {
        state = MonsterState.STARE;
        Idle();
    }

    void Walk()
    {
        animator.SetInteger("Speed", 1);
        _agent.speed = walkSpeed;
        // TODO: play the walk sound on loop while the monster is walking
    }

    void Run()
    {
        animator.SetInteger("Speed", 2);
        _agent.speed = runSpeed;
    }


    // wrapper for calling coroutine
    void Stare()
    {
        monsterHead.transform.LookAt(GameLogicController.Instance.player.transform);
    }

    // wrapper for calling DoIdle coroutine
    void Idle()
    {
        animator.SetInteger("Speed", 0);
        _agent.speed = 0;
    }
    struct _doIdleArgs
    {
        public float time;
        public int waypointNum;
    }
    // waits for 'time' seconds and then sets the navmesh destination
    // to "destination"
    IEnumerator DoIdle(_doIdleArgs args)
    {
        float timer = args.time;
        while (timer > 0 && state == MonsterState.ROAM)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        if (state == MonsterState.PURSUE)
            yield break;
        GoToWaypoint(args.waypointNum);
    }

    // tells the monster to roam to the player
    public void Alert()
    {
        if (state == MonsterState.ROAM)
        {
            Walk();
            _currWaypoint = GameLogicController.Instance.player.transform;
            _agent.destination = _currWaypoint.position;
        } else if (state == MonsterState.STARE)
        {
            PursueBegin();
        }
    }
    // moves towards player. must be executed every frame so agent can lock on to
    // the player
    void GoToPlayer()
    {
        _agent.destination = GameLogicController.Instance.player.transform.position;
    }

    // goes to waypoint i in the list of waypoints
    void GoToWaypoint(int index)
    {
        Walk();
        _currWaypoint = waypoints[index];
        _agent.destination = _currWaypoint.position;
    }

    // idles for a given amount of time and then moves to the next waypoint
    void IdleGoToWayPoint(float time, int nextWaypoint)
    {
        Idle();
        _doIdleArgs args; args.time = time; args.waypointNum = nextWaypoint;
        StartCoroutine("DoIdle", args);
    }

    // -------------------------------------------------------------------------

    // ------------------------- Helper functions ------------------------------
    public bool MonsterCanSeePlayer()
    {
        Ray ray = new Ray();
        ray.origin = monsterHead.transform.position;
        ray.direction = (GameLogicController.Instance.player.transform.position - monsterHead.transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, playerMaxVisibleDistance))
        {
            if (hit.collider.tag == "Player") // nothing in between monster and player
            {
                _planes = GeometryUtility.CalculateFrustumPlanes(monsterCam);
                // player inside of cam frustrum
                if (GeometryUtility.TestPlanesAABB(_planes, GameLogicController.Instance.player.GetComponent<CapsuleCollider>().bounds))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool PlayerCanSeeMonster()
    {
        Ray ray = new Ray();
        ray.origin = GameLogicController.Instance.player.transform.position;
        ray.direction = (monsterHead.transform.position - ray.origin).normalized;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.tag == "Monster") // nothing in between monster and player
            {
                _planes = GeometryUtility.CalculateFrustumPlanes(_playerCam);
                // monster inside of cam frustrum
                if (GeometryUtility.TestPlanesAABB(_planes, _collider.bounds))
                {
                    return true;
                }
            }
        }
        return false;
    }
    // -------------------------------------------------------------------------
    //
    // ------------------------- Damaging player -------------------------------

    // damages the player my "damageAmount" and applied cooldown for "damageCooldown" seconds
    void DamagePlayer()
    {
        GameLogicController.Instance.player.GetComponent<Health>().Damage(damageAmount);
        StartCoroutine("DamageCooldown");
    }

    IEnumerator DamageCooldown()
    {
        _cooldownDmg = true;
        yield return new WaitForSeconds(damageCooldown);
        _cooldownDmg = false;
    }
    // -------------------------------------------------------------------------

}
