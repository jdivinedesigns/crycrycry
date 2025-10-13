using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayAudioAction : MonoBehaviour
{
    public AudioSource source;
    void Reset() { source = GetComponent<AudioSource>(); }
    public void PlayClip() { if (source) source.Play(); }
    public void StopClip() { if (source) source.Stop(); }
}
