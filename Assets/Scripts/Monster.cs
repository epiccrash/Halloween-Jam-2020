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
    public float playerMaxVisibleDistance;
    public List<Transform> waypoints;
    public Vector2 waypointIdleTimeRange;
    public Camera monsterCam;
    public float walkSpeed;
    public float runSpeed;
    Transform _currWaypoint;
    Plane[] _planes;
    Camera _playerCam;
    CapsuleCollider _collider;
    NavMeshAgent _agent;

    public enum MonsterState
    {
        ROAM,
        PURSUE,
        STARE
    }

    public MonsterState state;

    // Animation
    Animator animator;

    // if we run into a waypoint, idle for a bit and then go to another waypoint
    private void OnTriggerEnter(Collider other)
    {

        if (state == MonsterState.ROAM)
        {
            if (other.gameObject.tag == "Waypoint")
            {
                if (other.gameObject.transform.position == _currWaypoint.position)
                {
                    // idle for random amt of time and then move to next waypoint
                    IdleGoToWayPoint(Random.Range(waypointIdleTimeRange[0], waypointIdleTimeRange[1]), Random.Range(0, waypoints.Count));
                }
            }
        }
    }

    private void Start()
    {
        _playerCam = Camera.main;
        _collider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        GoToWaypoint(Random.Range(0, waypoints.Count));
        state = MonsterState.ROAM;

        if (state == MonsterState.ROAM) RoamBegin();
        else PersueBegin();
    }

    // called when state changes to PERSUE
    void PersueBegin()
    {
        Run();
        state = MonsterState.PURSUE;
        // TODO: play monster scream sound
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

    private void Update()
    {
        if (state == MonsterState.ROAM)
        {

            // if the player sees the monster, enter PERSUE
            if (PlayerCanSeeMonster() && MonsterCanSeePlayer())
                PersueBegin();

            // if monster sees player, enter STARE
            if (!PlayerCanSeeMonster() && MonsterCanSeePlayer())
                StareBegin();

            // TODO: play roam sound

        } else if (state == MonsterState.PURSUE)
        {
            GoToPlayer();

            // if the player cannot see the monster and the monster also cannot
            // see the player, enter ROAM
            // TODO: add a cooldown in between when the player leaves the
            // monster's vision and when the monster enters ROAM
            if (!MonsterCanSeePlayer())
                RoamBegin();
        } else if (state == MonsterState.STARE)
        {
            Stare();

            // if the player sees the monster, enter PERSUE
            if (PlayerCanSeeMonster() && MonsterCanSeePlayer())
                PersueBegin();

            if (!MonsterCanSeePlayer())
                RoamBegin();
        }

    }

    // ----------------------- Animation and Navigation -----------------------
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
    // ------------------------------------------------------------------------
}
