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
    Transform currWaypoint;
    Plane[] _planes;
    Camera _cam;
    SphereCollider _collider;

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
            if (other.gameObject.transform == currWaypoint)
            {

            }
        }
    }
    private void Start()
    {
        _cam = Camera.main;
        _collider = GetComponent<SphereCollider>();
        animator = GetComponent<Animator>();
        GoToWaypoint(Random.Range(0, waypoints.Count));
        state = MonsterState.ROAM;
    }

    void PersueBegin()
    {

    }

    void RoamBegin()
    {

    }

    private void Update()
    {
        if (state == MonsterState.ROAM)
        {
            _idleTimer -= Time.deltaTime;
            if (_idleTimer < 0)
            {
                GoToWaypoint
            }
        } else if (state == MonsterState.PERSUE)
        {

        }

    }

    void Walk()
    {
        animator.SetInteger("Speed", 1);
    }

    void Run()
    {
        animator.SetInteger("Speed", 2);
    }

    // idles for a given amount of time
    void Idle(float time)
    {
        animator.SetInteger("Speed", 0);
    }

    void GoToPlayer()
    {

    }

    // goes to waypoint i in the list of waypoints
    void GoToWaypoint(int index)
    {

    }

    public bool CanSeePlayer()
    {
        Ray ray = new Ray();
        ray.origin = monsterHead.transform.position;
        ray.direction = (GameLogicController.Instance.player.transform.position = monsterHead.transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, playerMaxVisibleDistance))
        {
            if (hit.collider.tag == "Player") // nothing in between monster and player
            {
                _planes = GeometryUtility.CalculateFrustumPlanes(_cam);
                // monster inside of cam frustrum
                if (GeometryUtility.TestPlanesAABB(_planes, _collider.bounds))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
