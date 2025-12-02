using System.Collections;
using UnityEngine;

public class PhoneAutoPickup : MonoBehaviour
{
    [Header("References")]
    public GameObject phoneOnHook;        // original phone on hook mesh
    public Transform phoneInHandRoot;     // phone that moves into the hand
    public Transform rightHandSocket;     // RightPhoneHandSocket
    public Transform leftHandSocket;      // LeftPhoneHandSocket

    [Header("Options")]
    public float moveTime = 0.25f;
    public bool oneShot = true;

    [Header("Pickup Settings")]
    public float pickupRadius = 0.25f;    // how close the hand must be to this object

    bool hasPickedUp = false;
    Coroutine moveRoutine;

    void Start()
    {
        // phone starts on hook, in-hand phone hidden
        if (phoneOnHook != null)
            phoneOnHook.SetActive(true);

        if (phoneInHandRoot != null)
            phoneInHandRoot.gameObject.SetActive(false);
    }

    void Update()
    {
        // If something else is already happening, do nothing
        if (GameInteractionManager.instance != null &&
            GameInteractionManager.instance.IsBusy())
            return;

        if (oneShot && hasPickedUp)
            return;

        // We measure distance from THIS object (Phone Trigger) to the hands
        Vector3 center = transform.position;

        bool rightInRange = false;
        bool leftInRange = false;

        if (rightHandSocket != null)
        {
            float dR = Vector3.Distance(center, rightHandSocket.position);
            rightInRange = dR <= pickupRadius;
        }

        if (leftHandSocket != null)
        {
            float dL = Vector3.Distance(center, leftHandSocket.position);
            leftInRange = dL <= pickupRadius;
        }

        // If neither hand is close enough, do nothing this frame
        if (!rightInRange && !leftInRange)
            return;

        // Decide which hand to snap to
        Transform targetSocket = null;

        if (rightInRange && !leftInRange)
        {
            targetSocket = rightHandSocket;
        }
        else if (leftInRange && !rightInRange)
        {
            targetSocket = leftHandSocket;
        }
        else
        {
            // both in range: choose the closer one
            float dR = Vector3.Distance(center, rightHandSocket.position);
            float dL = Vector3.Distance(center, leftHandSocket.position);
            targetSocket = (dR < dL) ? rightHandSocket : leftHandSocket;
        }

        // Start pickup motion
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(PickupToSocket(targetSocket));
    }

    IEnumerator PickupToSocket(Transform handSocket)
    {
        hasPickedUp = true;

        if (GameInteractionManager.instance != null)
            GameInteractionManager.instance.phonePickedUp = true;

        // Hide phone on hook, show phone in hand
        if (phoneOnHook != null)
            phoneOnHook.SetActive(false);

        if (phoneInHandRoot != null)
            phoneInHandRoot.gameObject.SetActive(true);

        // Start from current in-hand phone position/rotation
        Vector3 startPos = phoneInHandRoot.position;
        Quaternion startRot = phoneInHandRoot.rotation;

        Vector3 endPos = handSocket.position;
        Quaternion endRot = handSocket.rotation;

        float t = 0f;

        while (t < moveTime)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / moveTime);

            phoneInHandRoot.position = Vector3.Lerp(startPos, endPos, u);
            phoneInHandRoot.rotation = Quaternion.Slerp(startRot, endRot, u);

            yield return null;
        }

        phoneInHandRoot.position = endPos;
        phoneInHandRoot.rotation = endRot;

        // TODO: trigger creepy whisper here if it's not already hooked up
        // e.g. GetComponent<AudioSource>()?.Play();
    }
}

