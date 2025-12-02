using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusSimplePath : MonoBehaviour
{
    [Header("Points")]
    public Transform startPoint;   // off-screen right
    public Transform stopPoint;    // in front of bus stop
    public Transform exitPoint;    // where it drives off

    [Header("Timing")]
    public float inSpeed = 6f;
    public float outSpeed = 6f;
    public float dwellSeconds = 2f;
    public bool autoPlayOnStart = true;

    Coroutine driveRoutine;

    void Start()
    {
        // snap bus to start
        if (startPoint != null)
        {
            transform.position = startPoint.position;
            transform.rotation = startPoint.rotation;
        }

        if (autoPlayOnStart)
            Play();
    }

    public void Play()
    {
        if (driveRoutine != null) StopCoroutine(driveRoutine);
        driveRoutine = StartCoroutine(DriveSequence());
    }

    IEnumerator DriveSequence()
    {
        if (!startPoint || !stopPoint || !exitPoint) yield break;

        // make sure we start at the start
        transform.position = startPoint.position;
        transform.rotation = startPoint.rotation;

        float groundY = transform.position.y;

        // 1) drive in to the stop
        yield return StartCoroutine(MoveTowards(stopPoint.position, inSpeed, groundY));

        // 2) pause
        yield return new WaitForSeconds(dwellSeconds);

        // 3) turn + drive out toward exit
        yield return StartCoroutine(MoveTowards(exitPoint.position, outSpeed, groundY));
    }

    IEnumerator MoveTowards(Vector3 targetPos, float speed, float groundY)
    {
        targetPos.y = groundY;

        while (Vector3.Distance(
                   new Vector3(transform.position.x, 0f, transform.position.z),
                   new Vector3(targetPos.x, 0f, targetPos.z)) > 0.05f)
        {
            // direction on flat ground
            Vector3 dir = targetPos - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.0001f)
            {
                // rotate only around Y so it doesn't twist
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    5f * Time.deltaTime
                );
            }

            // move forward in whatever way we're facing
            transform.position += transform.forward * speed * Time.deltaTime;

            // lock to ground height
            transform.position = new Vector3(
                transform.position.x,
                groundY,
                transform.position.z
            );

            yield return null;
        }

        // snap exactly
        transform.position = targetPos;
    }
}
