using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// the monster has two states
// MonsterState.ROAM and MonsterState.PERSUE
//
// in PERSUE, the monster is running towards the player and screaming.
// when PERSUE state is initiated, PersueBegin is called
//
// in ROAM, the monster randomly selects waypoints to walk to and then
// idles at them for a random range of time (defined by waypointIdleTimeRange).
// when ROAM state is intiated, RoamBegin is called

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
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
    SphereCollider _collider;
    NavMeshAgent _agent;
    bool _isIdling;

    public enum MonsterState
    {
        ROAM,
        PERSUE
    }

    public MonsterState state;


    // Animation
    Animator animator;

    float _idleTimer;

    // if we run into a waypoint, idle for a bit and then go to another waypoint
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Waypoint")
        {
            if (other.gameObject.transform == _currWaypoint)
            {
                // idle for random amt of time and then move to next waypoint
                Idle(Random.Range(waypointIdleTimeRange[0], waypointIdleTimeRange[1]), Random.Range(0, waypoints.Count));
            }
        }
    }

    private void Start()
    {
        _playerCam = Camera.main;
        _collider = GetComponent<SphereCollider>();
        animator = GetComponent<Animator>();
        GoToWaypoint(Random.Range(0, waypoints.Count));
        state = MonsterState.ROAM;
        _agent = GetComponent<NavMeshAgent>();
        RoamBegin();
    }

    // called when state changes to PERSUE
    void PersueBegin()
    {
        Debug.Log("PERSUE begin");
        Run();
        state = MonsterState.PERSUE;
        // TODO: play monster scream sound
    }

    // called when state changes to ROAM
    void RoamBegin()
    {
        Debug.Log("ROAM begin");
        Walk();
        state = MonsterState.ROAM;
        GoToWaypoint(Random.Range(0, waypoints.Count));
    }

    private void Update()
    {
        if (state == MonsterState.ROAM)
        {
            
            // if the player sees the monster, enter PERSUE
            if (PlayerCanSeeMonster())
                PersueBegin();

            // TODO: play roam sound

        } else if (state == MonsterState.PERSUE)
        {
            GoToPlayer();

            // if the player cannot see the monster and the monster also cannot
            // see the player, enter ROAM
            // TODO: add a cooldown in between when the player leaves the
            // monster's vision and when the monster enters ROAM
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

    // idles for a given amount of time and then moves to the next waypoint
    void Idle(float time, int nextWaypoint)
    {
        animator.SetInteger("Speed", 0);
        _isIdling = true;
        _doIdleArgs args; args.time = time; args.waypointNum = nextWaypoint;
        StartCoroutine("DoIdle", args);
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
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        GoToWaypoint(args.waypointNum);
    }

    // moves towards player. must be executed every frame so agent can lock on to
    // the player
    void GoToPlayer()
    {
        _isIdling = false;
        _agent.destination = GameLogicController.Instance.player.transform.position;
    }

    // goes to waypoint i in the list of waypoints
    void GoToWaypoint(int index)
    {
        _isIdling = false;
        Walk();
        _agent.destination = waypoints[index].position;
    }
    // -------------------------------------------------------------------------

    // ------------------------- Helper functions ------------------------------
    public bool MonsterCanSeePlayer()
    {
        Ray ray = new Ray();
        ray.origin = monsterHead.transform.position;
        ray.direction = (GameLogicController.Instance.player.transform.position = monsterHead.transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, playerMaxVisibleDistance))
        {
            if (hit.collider.tag == "Player") // nothing in between monster and player
            {
                _planes = GeometryUtility.CalculateFrustumPlanes(monsterCam);
                // monster inside of cam frustrum
                if (GeometryUtility.TestPlanesAABB(_planes, _collider.bounds))
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
        ray.origin = monsterHead.transform.position;
        ray.direction = (GameLogicController.Instance.player.transform.position = monsterHead.transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, playerMaxVisibleDistance))
        {
            if (hit.collider.tag == "Player") // nothing in between monster and player
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
