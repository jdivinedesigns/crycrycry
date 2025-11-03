using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    [Header("Target (leave blank to use Light on this object)")]
    public Light target;

    [Header("Base glow")]
    public float minIntensity = 0.6f;
    public float maxIntensity = 1.4f;
    public float noiseSpeed = 6f;     // how fast the soft flicker moves

    [Header("Occasional hard glitches")]
    [Range(0f, 1f)] public float glitchChancePerSecond = 0.4f;
    public float glitchDuration = 0.06f;  // seconds
    public float glitchOffChance = 0.5f;  // 50% of glitches go fully dark

    float t, baseIntensity;
    float glitchUntil;

    void Awake()
    {
        if (!target) target = GetComponent<Light>() ?? GetComponentInChildren<Light>();
        if (target) baseIntensity = target.intensity;
    }

    void Update()
    {
        if (!target) return;

        // Soft Perlin-noise flicker
        t += Time.deltaTime * noiseSpeed;
        float n = Mathf.PerlinNoise(0f, t);                    // 0..1
        float soft = Mathf.Lerp(minIntensity, maxIntensity, n);

        // Random “hard” glitch bursts
        if (Time.time > glitchUntil && Random.value < glitchChancePerSecond * Time.deltaTime)
            glitchUntil = Time.time + glitchDuration;

        bool inGlitch = Time.time < glitchUntil;
        float hard = inGlitch
            ? (Random.value < glitchOffChance ? 0f : maxIntensity * 2.2f)   // pop or go dark
            : soft;

        target.intensity = baseIntensity * hard;
    }
}
