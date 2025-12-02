using UnityEngine;

public class AvatarGhostSwitcher : MonoBehaviour
{
    [Header("References")]
    public GameObject avatarRoot;      // normal player (readyplayme busstop)
    public GameObject ghostRoot;       // SF_ghost2 (1)
    public Transform matchFrom;        // optional: usually hips; can leave empty

    [Header("Options")]
    public bool once = true;

    bool hasSwitched = false;

    void Start()
    {
        // make sure we start in human form
        if (avatarRoot) avatarRoot.SetActive(true);
        if (ghostRoot) ghostRoot.SetActive(false);
    }

    [ContextMenu("Ghostify Now")]
    public void Ghostify()
    {
        if (once && hasSwitched) return;
        hasSwitched = true;

        // decide what transform we match from
        Transform src = matchFrom;
        if (!src && avatarRoot)
            src = avatarRoot.transform;

        if (ghostRoot && src)
        {
            Transform gt = ghostRoot.transform;
            gt.position = src.position;
            gt.rotation = src.rotation;
            gt.localScale = src.localScale;
        }

        // swap active roots
        if (avatarRoot) avatarRoot.SetActive(false);
        if (ghostRoot) ghostRoot.SetActive(true);

        Debug.Log("[AvatarGhostSwitcher] Switched to ghost + matched transform.");
    }
}
