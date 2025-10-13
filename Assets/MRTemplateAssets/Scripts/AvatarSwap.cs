using UnityEngine;
using System.Collections;

public class AvatarSwap : MonoBehaviour
{
    public GameObject human;
    public GameObject skeleton;
    public float dissolveTime = 0.6f; // optional delay, feels nicer

    public void Swap()
    {
        StartCoroutine(SwapCo());
    }

    IEnumerator SwapCo()
    {
        // quick delay for SFX hit / splash timing
        yield return new WaitForSeconds(0.15f);
        if (human) human.SetActive(false);
        if (skeleton) skeleton.SetActive(true);
    }
}
