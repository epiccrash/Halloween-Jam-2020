using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this should be attatched to the player object
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class FPController : MonoBehaviour
{
    [Header("Sound")]
    public AudioSource playerFootstepSource;
    public List<AudioClip> footsteps;
	public float pitch_min = 0.95f;
	public float pitch_max = 1.05f;
	public float minSpeedForFootsteps = 0.8f;
	[Space(10)]

    [Header("Camera")]
    public GameObject cam; // player camera
    public bool mouseInvert; // when set to true, look y is inverted
    public float lookspeed;
    public float camLookMaxAngle; // maximum euler angle x for looking
    [Space(10)]

    [Header("Movement")]
    public float horzMovementSpeed;
    public float forwMovementSpeed;
    public float fallSpeed;
    public float jumpAmount;
    public KeyCode jumpKey;
    public KeyCode sprintKey;
    public float sprintSpeedFactor; // amount to multiple speed by when sprinting

    private Vector3 vertVelocity; // vertical movement vector (gravity and jump)
    private float save_horzMoementSpeed;
    private float save_forwMoementSpeed;
  
    Vector3 movement;


    // Start is called before the first frame update
    void Start()
    {
        save_forwMoementSpeed = forwMovementSpeed;
        save_horzMoementSpeed = horzMovementSpeed;
    }

    void FixedUpdate()
    {
        movement = Vector3.zero;
        Debug.Log(gameObject.GetComponent<CharacterController>().isGrounded);
        Gravity();
        Move();
        Jump();
        gameObject.GetComponent<CharacterController>().Move(movement);
    }

    void Update()
    {
        CameraLook(); // must be called in Update to avoid edge case behavior
    }

    void CameraLook()
    {
        // do not move the camera if the timeScale is zero (game is paused)
        if (!Mathf.Approximately(Time.timeScale, 0f))
        {
            // rotate camera according to mouse input
            float mousex = Input.GetAxis("Mouse X");
            if (!Mathf.Approximately(mousex, 0))
            {
                gameObject.transform.Rotate(0, mousex * lookspeed, 0);
            }

            float mousey = Input.GetAxis("Mouse Y");
            if (!Mathf.Approximately(mousey, 0))
            {
                int inv = mouseInvert ? 1 : -1;
                cam.transform.Rotate(new Vector3(mousey * lookspeed * inv, 0, 0), Space.Self);

                // rotation clamping
                Vector3 eulers = cam.transform.eulerAngles;
                float upperBound = 360 - camLookMaxAngle;
                float lowerBound = camLookMaxAngle;
                // if out of bounds
                if (eulers.x < upperBound && eulers.x > lowerBound)
                {
                    // snap to above
                    if (Mathf.Abs(eulers.x - upperBound) < Mathf.Abs(eulers.x - lowerBound))
                    {
                        cam.transform.SetPositionAndRotation(cam.transform.position,
                                Quaternion.Euler(new Vector3(upperBound, eulers.y, eulers.z)));
                    }
                    else
                    { // snap to below
                        cam.transform.SetPositionAndRotation(cam.transform.position,
                                Quaternion.Euler(new Vector3(lowerBound, eulers.y, eulers.z)));
                    }
                }

                // no z rotation
                if (eulers.z != 0)
                {
                    cam.transform.SetPositionAndRotation(cam.transform.position,
                        Quaternion.Euler(new Vector3(eulers.x, eulers.y, 0)));
                }
            }
        }
    }

    void Move()
    {
        float horz = Input.GetAxis("Horizontal");
        movement += (gameObject.transform.right) * horzMovementSpeed * horz * 0.1f;
        float vert = Input.GetAxis("Vertical");
        movement += (gameObject.transform.forward) * forwMovementSpeed * vert * 0.1f;
		// Footstep sound
		float currentSpeed = (new Vector2(horz, vert)).magnitude;
		if (currentSpeed > minSpeedForFootsteps && !playerFootstepSource.isPlaying) {
			playerFootstepSource.pitch = Random.Range(pitch_min, pitch_max);
			playerFootstepSource.PlayOneShot(footsteps[Random.Range(0, footsteps.Count)]);
		} else if (currentSpeed < minSpeedForFootsteps) {
			playerFootstepSource.Stop();
		}
	}

    void Gravity()
    {
        if (!gameObject.GetComponent<CharacterController>().isGrounded)
        {
            vertVelocity = new Vector3(0, vertVelocity.y - (fallSpeed * Time.deltaTime), 0);
            movement += vertVelocity * Time.deltaTime;
        }
        else
        {
            vertVelocity = new Vector3(0, 0, 0);
        }
    }

    void Jump()
    {
        if (Input.GetKey(jumpKey))
        {
            if (gameObject.GetComponent<CharacterController>().isGrounded)
            {
                vertVelocity = new Vector3(0, jumpAmount, 0);
            }
        }
    }

    void Sprint()
    {
        if (Input.GetKey(sprintKey))
        {
            float sideSpeedFactor = .75f; // side movement increase speed only by factor of .75
            horzMovementSpeed = save_horzMoementSpeed * sprintSpeedFactor;
            forwMovementSpeed = save_forwMoementSpeed * sprintSpeedFactor * sideSpeedFactor;
        }
        else
        {
            horzMovementSpeed = save_horzMoementSpeed;
            forwMovementSpeed = save_forwMoementSpeed;
        }
    }
}