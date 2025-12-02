using System.Collections;
using UnityEngine;

public class LightFlickerAndGhost : MonoBehaviour
{
    [Header("Who can trigger")]
    public string playerTag = "Player";
    public Transform player;              // readyplayme busstop
    public Collider playerCollider;       // main capsule on the player

    [Header("Lights to control")]
    public Light[] lights;
    public Renderer[] emissiveRenderers;

    [Header("Effect")]
    public bool flicker = true;
    public float blackoutSeconds = 3f;
    public Vector2 flickerInterval = new Vector2(0.05f, 0.2f);

    [Header("After Effect")]
    public bool leaveLightsOff = false;

    [Header("Ghost Switch")]
    public AvatarGhostSwitcher ghostSwitcher;

    [Header("Other")]
    public bool oneShot = true;

    bool fired;

    void Reset()
    {
        var col = GetComponent<BoxCollider>();
        if (!col) col = gameObject.AddComponent<BoxCollider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[LightTrigger] OnTriggerEnter by {other.name} (root: {other.transform.root.name})");

        if (fired && oneShot) return;

        // If a specific collider is assigned, ONLY that one can trigger
        if (playerCollider != null)
        {
            if (other != playerCollider)
            {
                // ignore hands, phone sockets, mouse, etc.
                return;
            }
        }
        else
        {
            // fallback behaviour if you forget to assign it
            bool matchesTransform = player && other.transform.root == player;
            bool matchesTag = !string.IsNullOrEmpty(playerTag) && other.CompareTag(playerTag);

            if (!(matchesTransform || matchesTag))
                return;
        }

        StartCoroutine(DoBlackout());
    }

    [ContextMenu("Trigger Now")]
    public void TriggerNow()
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(DoBlackout());
    }

    IEnumerator DoBlackout()
    {
        if (fired && oneShot) yield break;
        fired = true;

        Debug.Log("[LightTrigger] Starting blackout/flicker...");

        // local helper to toggle all lights + emission
        void SetOn(bool on)
        {
            if (lights != null)
            {
                foreach (var L in lights)
                    if (L) L.enabled = on;
            }

            if (emissiveRenderers != null)
            {
                foreach (var r in emissiveRenderers)
                {
                    if (!r) continue;
                    foreach (var m in r.materials)
                    {
                        if (on) m.EnableKeyword("_EMISSION");
                        else m.DisableKeyword("_EMISSION");
                    }
                }
            }
        }

        if (!flicker)
        {
            SetOn(false);
            yield return new WaitForSeconds(blackoutSeconds);
        }
        else
        {
            float t = 0f;
            while (t < blackoutSeconds)
            {
                float a = Random.Range(flickerInterval.x, flickerInterval.y);
                float b = Random.Range(flickerInterval.x, flickerInterval.y);

                SetOn(false);
                yield return new WaitForSeconds(a);
                t += a;

                SetOn(true);
                yield return new WaitForSeconds(b);
                t += b;
            }

            SetOn(false);
        }

        // swap avatar → ghost
        if (ghostSwitcher) ghostSwitcher.Ghostify();
        else Debug.LogWarning("[LightTrigger] No GhostSwitcher assigned.");

        // decide if light comes back on
        if (!leaveLightsOff)
            SetOn(true);
    }
}
