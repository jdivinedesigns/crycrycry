using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusWaypointDriver : MonoBehaviour
{
    [Header("Path waypoints (in order)")]
    public Transform[] waypoints;

    [Header("Movement")]
    public float speed = 6f;
    public float rotationSpeed = 5f;

    [Header("Stop at bus stop")]
    public int stopIndex = 1;
    public float dwellSeconds = 2f;
    public bool autoPlayOnStart = true;

    Coroutine routine;

    void Start()
    {
        if (autoPlayOnStart)
            Play();
    }

    public void Play()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(DrivePath());
    }

    IEnumerator DrivePath()
    {
        if (waypoints == null || waypoints.Length < 2)
            yield break;

        Vector3 pos = waypoints[0].position;
        float groundY = pos.y;
        transform.position = pos;

        for (int i = 1; i < waypoints.Length; i++)
        {
            Transform target = waypoints[i];

            // --- rotate once toward the target ---
            Vector3 dir = (target.position - transform.position);
            dir.y = 0;
            Quaternion targetRot = Quaternion.LookRotation(dir);

            while (Quaternion.Angle(transform.rotation, targetRot) > 0.5f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime
                );
                yield return null;
            }

            transform.rotation = targetRot;

            // --- move straight ---
            while (Vector3.Distance(
                new Vector3(transform.position.x, 0, transform.position.z),
                new Vector3(target.position.x, 0, target.position.z)) > 0.05f)
            {
                transform.position += transform.forward * speed * Time.deltaTime;

                transform.position = new Vector3(
                    transform.position.x, groundY, transform.position.z
                );

                yield return null;
            }

            transform.position = new Vector3(target.position.x, groundY, target.position.z);

            if (i == stopIndex)
                yield return new WaitForSeconds(dwellSeconds);
        }
    }
}
