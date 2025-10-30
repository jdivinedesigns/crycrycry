using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityByDistance : MonoBehaviour
{
    [Header("Who/what to affect")]
    [SerializeField] GameObject ghostRoot;

    [Header("Where to measure from (usually hips)")]
    [SerializeField] Transform playerBone;

    [Header("Settings")]
    [SerializeField] float radius = 1.5f;
    [SerializeField] bool oneShot = true;

    bool fired;

    void Update()
    {
        if (!playerBone || !ghostRoot) return;
        if (oneShot && fired) return;

        float distSqr = (playerBone.position - transform.position).sqrMagnitude;
        if (distSqr <= radius * radius)
        {
            ghostRoot.SetActive(true);
            fired = true;
            Debug.Log("[ProximityByDistance] ghost shown");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
