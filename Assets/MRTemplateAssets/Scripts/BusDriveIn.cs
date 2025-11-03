using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusDriveIn : MonoBehaviour
{
    [Header("Path (X values only)")]
    public float startX = -50f;     // where bus begins (off-screen right)
    public float stopX = 0f;       // where it stops (bus stop position)
    public float endX = 50f;      // where it exits (off-screen left)

    [Header("Timing")]
    public float inSpeed = 6f;
    public float outSpeed = 8f;
    public float dwellSeconds = 3f;
    public bool autoPlayOnStart = false;

    Coroutine routine;

    void Start()
    {
        if (autoPlayOnStart) Play();
        Vector3 pos = transform.position;
        transform.position = new Vector3(startX, pos.y, pos.z); // ensure starting X
    }

    public void Play()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Drive());
    }

    IEnumerator Drive()
    {
        yield return MoveX(stopX, inSpeed);
        yield return new WaitForSeconds(dwellSeconds);
        yield return MoveX(endX, outSpeed);
    }

    IEnumerator MoveX(float targetX, float speed)
    {
        Vector3 pos = transform.position;
        float dir = Mathf.Sign(targetX - pos.x);
        while ((dir > 0 && pos.x < targetX) || (dir < 0 && pos.x > targetX))
        {
            pos.x += dir * speed * Time.deltaTime;
            transform.position = pos;
            yield return null;
        }
        pos.x = targetX;
        transform.position = pos;
    }
}
