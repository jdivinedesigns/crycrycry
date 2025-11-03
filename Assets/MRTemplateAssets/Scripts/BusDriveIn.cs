using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusDriveIn : MonoBehaviour
{
    [Header("Path")]
    public Transform pointIn;
    public Transform pointStop;
    public Transform pointOut;

    [Header("Timing")]
    public float inSpeed = 6f;      // m/s moving in & to stop
    public float outSpeed = 8f;     // m/s when leaving
    public float dwellSeconds = 3f; // how long it waits at the stop
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool autoPlayOnStart = false;

    Coroutine running;

    void Start()
    {
        if (autoPlayOnStart) Play();
        // optional: start exactly at pointIn
        if (pointIn) transform.position = pointIn.position;
    }

    public void Play()
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        if (pointIn) yield return MoveTo(pointIn.position, inSpeed);
        if (pointStop) yield return MoveTo(pointStop.position, inSpeed);
        yield return new WaitForSeconds(dwellSeconds);
        if (pointOut) yield return MoveTo(pointOut.position, outSpeed);
        running = null;
    }

    IEnumerator MoveTo(Vector3 dest, float speed)
    {
        Vector3 start = transform.position;
        float dist = Vector3.Distance(start, dest);
        float dur = dist / Mathf.Max(0.01f, speed);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / dur;
            float k = ease.Evaluate(Mathf.Clamp01(t));
            transform.position = Vector3.Lerp(start, dest, k);

            // face movement direction (yaw only)
            Vector3 dir = (dest - transform.position); dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);

            yield return null;
        }

        transform.position = dest;
    }
}
