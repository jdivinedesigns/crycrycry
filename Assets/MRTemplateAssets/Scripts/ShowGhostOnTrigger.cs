using UnityEngine;

public class ShowGhostOnTrigger : MonoBehaviour
{
    public GameObject ghost;
    public string playerTag = "Player";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag)) ghost.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag)) ghost.SetActive(false);
    }
}
