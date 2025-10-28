using UnityEngine;
public class ShowGhostOnTrigger : MonoBehaviour
{
    [SerializeField] GameObject ghostRoot;
    [SerializeField] bool oneShot = true;
    bool shown;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[SGOT] enter by {other.name} tag={other.tag}");
        if (!other.CompareTag("Player")) return;
        if (oneShot && shown) return;
        if (!ghostRoot) { Debug.LogWarning("[SGOT] ghostRoot not set"); return; }
        ghostRoot.SetActive(true);
        shown = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || oneShot || !ghostRoot) return;
        ghostRoot.SetActive(false);
    }
}
