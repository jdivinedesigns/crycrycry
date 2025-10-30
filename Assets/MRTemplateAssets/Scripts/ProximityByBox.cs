using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityByBox : MonoBehaviour
{
    [Header("Who/what to affect")]
    [SerializeField] GameObject ghostRoot;

    [Header("Where to measure from (usually hips)")]
    [SerializeField] Transform playerBone;

    [Header("Box size (half extents, in meters)")]
    [SerializeField] Vector3 halfExtents = new Vector3(1.5f, 1.0f, 0.5f);

    [SerializeField] bool oneShot = true;
    bool fired;

    void Update()
    {
        // skip if missing refs or already triggered
        if (!playerBone || !ghostRoot) return;
        if (oneShot && fired) return;

        // convert player position into the BenchTrigger's local space
        Vector3 local = transform.InverseTransformPoint(playerBone.position);

        // check if within box bounds
        if (Mathf.Abs(local.x) <= halfExtents.x &&
            Mathf.Abs(local.y) <= halfExtents.y &&
            Mathf.Abs(local.z) <= halfExtents.z)
        {
            ghostRoot.SetActive(true);
            fired = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);
    }
}
