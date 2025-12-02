using System.Collections;
using UnityEngine;

public class PhoneRingController : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource ringSource;      // looping ringing sound
    public AudioSource pickupSource;    // one-shot pickup/static sound

    [Header("Timing (seconds)")]
    public float minDelay = 60f;        // 1 minute
    public float maxDelay = 180f;       // 3 minutes

    bool pickedUp = false;

    void Start()
    {
        // schedule the phone to ring after a random time
        StartCoroutine(RingAfterDelay());
    }

    IEnumerator RingAfterDelay()
    {
        // random time between 1 and 3 minutes
        float delay = Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);

        // if player already picked it up, do nothing
        if (pickedUp) yield break;

        // wait until NOTHING else is going on (light, sitting, phone)
        while (GameInteractionManager.instance != null &&
               GameInteractionManager.instance.IsBusy())
        {
            yield return null; // wait a frame and check again
        }

        // now safe to ring
        StartRinging();
    }

    void StartRinging()
    {
        if (ringSource == null || pickedUp) return;

        ringSource.loop = true;
        ringSource.Play();
        Debug.Log("Phone started ringing");
    }

    void StopRinging()
    {
        if (ringSource != null)
        {
            ringSource.Stop();
            ringSource.loop = false;
        }
    }

    public void OnPhonePickedUp()
    {
        if (pickedUp) return;
        pickedUp = true;

        // mark this in the global manager so nothing else triggers on top
        if (GameInteractionManager.instance != null)
        {
            GameInteractionManager.instance.phonePickedUp = true;
        }

        // stop ringing
        StopRinging();

        // play pickup / static / voice sound
        if (pickupSource != null)
        {
            pickupSource.Play();
            Debug.Log("Pickup sound played");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // simplest: whole avatar triggers pickup
        if (other.CompareTag("Player"))
        {
            OnPhonePickedUp();
        }

        // If you want only the hand, use:
        // if (other.CompareTag("PlayerHand")) OnPhonePickedUp();
    }
}
