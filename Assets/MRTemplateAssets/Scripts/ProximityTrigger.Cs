using UnityEngine;
using UnityEngine.Events;

public class ProximityTrigger : MonoBehaviour
{
    public string playerTag = "Player";   // Tag your XR rig or MainCamera root as "Player"
    public UnityEvent onEnter;
    public UnityEvent onExit;

    void OnTriggerEnter(Collider other) { if (other.CompareTag(playerTag)) onEnter?.Invoke(); }
    void OnTriggerExit(Collider other) { if (other.CompareTag(playerTag)) onExit?.Invoke(); }
}
