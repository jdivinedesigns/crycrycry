using UnityEngine;

public class GhostTrigger : MonoBehaviour
{
    public GameObject ghost;        // drag your ghost object here in Inspector
    public string playerTag = "Player";  // tag for your mocopi avatar

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            // turn ghost on when player touches bench
            ghost.SetActive(true);
            Debug.Log("Ghost appeared!");
        }
        else
        {
            Debug.Log("Something else entered");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            // hide ghost again when player leaves
            ghost.SetActive(false);
            Debug.Log("Ghost disappeared!");
        }
    }
}
