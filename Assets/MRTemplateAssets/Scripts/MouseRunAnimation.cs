using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRunAnimation : MonoBehaviour
{
    public Animator animator;           // assign the Animator from the child model
    public float speed = 2.0f;          // normal m/s
    public float sprintSpeed = 5.0f;    // when player is near
    public Transform[] waypoints;       // set 2+ points
    public bool loop = true;
    public bool faceDirection = true;
    public bool lockYHeight = true;     // keep mouse flat on ground

    [Header("Player Trigger Settings")]
    public Transform player;            // your avatar hips/camera rig
    public float triggerRadius = 2.0f;

    int i;
    float currentSpeed;
    float groundY;

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (animator) animator.Play("Mouse_Run", 0, 0f);
        currentSpeed = speed;
        groundY = transform.position.y;   // remember start height
    }

    void Update()
    {
        // proximity speed swap
        if (player)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            currentSpeed = dist < triggerRadius ? sprintSpeed : speed;
        }

        // move along waypoints
        if (waypoints != null && waypoints.Length > 0)
        {
            Vector3 target = waypoints[i].position;
            Vector3 to = target - transform.position;

            // step in world XZ only
            Vector3 horiz = new Vector3(to.x, 0f, to.z);
            Vector3 step = Vector3.ClampMagnitude(horiz, currentSpeed * Time.deltaTime);

            // face only around Y (no tipping)
            if (faceDirection && step.sqrMagnitude > 1e-6f)
            {
                Quaternion look = Quaternion.LookRotation(step, Vector3.up);
                // keep yaw only
                Vector3 e = look.eulerAngles;
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.Euler(0f, e.y, 0f),
                    10f * Time.deltaTime
                );
            }

            transform.position += step;

            // arrived?
            if (horiz.magnitude <= 0.05f)
            {
                i++;
                if (i >= waypoints.Length) i = loop ? 0 : waypoints.Length - 1;
            }
        }
        else
        {
            // no waypoints? run forward in local Z, but don’t tilt
            Vector3 forwardXZ = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
            transform.position += forwardXZ * currentSpeed * Time.deltaTime;
        }

        // keep fixed Y height (prevents slow float/sinking)
        if (lockYHeight)
        {
            Vector3 p = transform.position;
            transform.position = new Vector3(p.x, groundY, p.z);
        }
    }
}
