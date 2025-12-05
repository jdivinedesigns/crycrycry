using UnityEngine;

public class AvatarGhostSwitcher : MonoBehaviour
{
    [Header("References")]
    public GameObject avatarRoot;   // normal player (readyplayme busstop)
    public GameObject ghostRoot;    // SF_ghost2 (1)
    public Transform matchFrom;     // usually Hips

    [Header("Options")]
    public bool once = true;

    bool hasSwitched = false;

    void Start()
    {
        // start in human form
        if (avatarRoot) avatarRoot.SetActive(true);
        if (ghostRoot) ghostRoot.SetActive(false);
    }

    [ContextMenu("Ghostify Now")]
    public void Ghostify()
    {
        if (once && hasSwitched) return;
        hasSwitched = true;

        // choose source transform: Hips if set, else avatar root
        Transform src = matchFrom != null ? matchFrom : avatarRoot.transform;

        if (ghostRoot && src)
        {
            Transform gt = ghostRoot.transform;

            // put ghost where the player's hips currently are
            gt.position = src.position;
            gt.rotation = src.rotation;
            gt.localScale = src.localScale;
        }

        // swap active objects
        if (avatarRoot) avatarRoot.SetActive(false);
        if (ghostRoot) ghostRoot.SetActive(true);

        Debug.Log("[AvatarGhostSwitcher] Switched to ghost + matched hips transform.");
    }
}

