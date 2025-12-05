using System.Collections;
using UnityEngine;

// Simple delay script that just enables the Mocopi receiver component
public class MocopiStartDelay : MonoBehaviour
{
    // Drag the MocopiSimpleReceiver component into this slot in the Inspector
    public Behaviour receiverComponent;
    public float delaySeconds = 3f;

    IEnumerator Start()
    {
        // Make sure receiver is disabled at first
        if (receiverComponent != null)
            receiverComponent.enabled = false;

        // Wait for Mocopi app to start streaming and stabilize
        yield return new WaitForSeconds(delaySeconds);

        if (receiverComponent != null)
        {
            Debug.Log("MocopiStartDelay: enabling Mocopi receiver after delay");
            receiverComponent.enabled = true;   // when this turns on, it will start receiving
        }
    }
}
