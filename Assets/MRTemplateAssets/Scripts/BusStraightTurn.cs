using System.Collections;
using UnityEngine;

public class BusStraightTurn : MonoBehaviour
{
    [Header("Z Movement")]
    public float startZ = -50f;  // where the bus starts
    public float stopZ = 0f;     // where it stops
    public float endZ = 30f;     // where it leaves to

    [Header("Lane X Positions")]
    public float mainLaneX = 18f;    // driving lane (center of road)
    public float busLaneX = 15f;    // curb/bus bay lane (a bit closer to sidewalk)

    [Header("Lateral Shift Distances (in Z meters)")]
    public float pullInDistance = 8f;  // how far before stopZ we start sliding into bus lane
    public float mergeOutDistance = 8f;  // how far after stopZ we finish sliding back out

    [Header("Timing")]
    public float speedIn = 6f;
    public float speedOut = 6f;
    public float dwellSeconds = 2f;
    public bool autoPlayOnStart = true;

    [Header("Audio")]
    public AudioSource audioSource;   // AudioSource on the bus
    public AudioClip busSound;        // single .wav with pullup+stop+idle+drive-off

    float groundY;
    Coroutine routine;

    void Start()
    {
        Vector3 p = transform.position;
        groundY = p.y;

        // put bus at startZ in the main lane, keep whatever rotation you set
        transform.position = new Vector3(mainLaneX, groundY, startZ);

        if (autoPlayOnStart)
            Play();
    }

    public void Play()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        // 🔊 start full bus sound (pulling up + braking + idle + drive off)
        if (audioSource != null && busSound != null)
        {
            audioSource.PlayOneShot(busSound);
        }

        // drive in and pull into bus lane
        yield return ApproachWithPullIn();

        // wait at the stop (your clip already has idle in it)
        yield return new WaitForSeconds(dwellSeconds);

        // drive out and merge back into main lane
        yield return DepartWithMerge();

        routine = null;
    }

    IEnumerator ApproachWithPullIn()
    {
        float dir = Mathf.Sign(stopZ - startZ);   // +1 if moving forward, -1 if backwards
        float z = transform.position.z;

        while ((dir > 0 && z < stopZ) || (dir < 0 && z > stopZ))
        {
            z += dir * speedIn * Time.deltaTime;

            // distance remaining to the stop
            float distToStop = Mathf.Abs(stopZ - z);

            // start sliding sideways only in the last pullInDistance meters
            float tLat = Mathf.InverseLerp(pullInDistance, 0f, distToStop);
            tLat = Mathf.Clamp01(tLat);

            float x = Mathf.Lerp(mainLaneX, busLaneX, tLat);

            transform.position = new Vector3(x, groundY, z);
            yield return null;
        }

        // snap exactly into bus lane at the stop
        transform.position = new Vector3(busLaneX, groundY, stopZ);
    }

    IEnumerator DepartWithMerge()
    {
        float dir = Mathf.Sign(endZ - stopZ);
        float z = transform.position.z;
        float zStart = z;  // where we started moving again

        while ((dir > 0 && z < endZ) || (dir < 0 && z > endZ))
        {
            z += dir * speedOut * Time.deltaTime;

            // distance travelled since leaving the stop
            float distFromStop = Mathf.Abs(z - zStart);

            // slide back to main lane over mergeOutDistance
            float tLat = Mathf.InverseLerp(0f, mergeOutDistance, distFromStop);
            tLat = Mathf.Clamp01(tLat);

            float x = Mathf.Lerp(busLaneX, mainLaneX, tLat);

            transform.position = new Vector3(x, groundY, z);
            yield return null;
        }

        // snap to final position in the main lane
        transform.position = new Vector3(mainLaneX, groundY, endZ);
    }
}
