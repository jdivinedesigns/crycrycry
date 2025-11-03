using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class GlitchOverlay : MonoBehaviour
{
    [Header("Visibility bursts")]
    [Range(0f, 2f)] public float avgBurstsPerSecond = 0.6f;
    public Vector2 burstDurationRange = new Vector2(0.04f, 0.18f); // seconds

    [Header("Look")]
    [Range(0f, 1f)] public float minAlpha = 0.25f;
    [Range(0f, 1f)] public float maxAlpha = 0.7f;
    public Vector2 bandScale = new Vector2(1f, 12f);     // Tiling X,Y (Y = horizontal bars)
    public float jitterSpeed = 12f;                      // how fast the UVs jump during burst
    public float hueShiftPerBurst = 0.0f;                // set >0 for tint wiggle

    static readonly int MainTex_ST = Shader.PropertyToID("_MainTex_ST");
    static readonly int BaseColor = Shader.PropertyToID("_BaseColor"); // URP/Unlit

    Renderer rend;
    MaterialPropertyBlock mpb;
    float nextBurstAt;
    float burstUntil;
    Color baseColor;
    Vector2 tiling, offset;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
        // capture starting color to preserve your chosen tint/alpha
        baseColor = rend.sharedMaterial.HasProperty(BaseColor)
            ? rend.sharedMaterial.GetColor(BaseColor)
            : Color.white;
        tiling = new Vector2(bandScale.x, bandScale.y);
        offset = Vector2.zero;
        ScheduleNextBurst();
        rend.enabled = false;
    }

    void Update()
    {
        float t = Time.time;

        // start a burst?
        if (t >= nextBurstAt && t >= burstUntil)
        {
            float dur = Random.Range(burstDurationRange.x, burstDurationRange.y);
            burstUntil = t + dur;
            ScheduleNextBurst();

            if (hueShiftPerBurst > 0f)
            {
                // tiny hue nudge for variety
                Color.RGBToHSV(baseColor, out float H, out float S, out float V);
                H = Mathf.Repeat(H + hueShiftPerBurst, 1f);
                baseColor = Color.HSVToRGB(H, S, V);
            }
        }

        bool inBurst = t < burstUntil;
        rend.enabled = inBurst; // only show during bursts

        if (inBurst)
        {
            // randomize alpha + UV strips
            float a = Random.Range(minAlpha, maxAlpha);

            // fast horizontal band jitter (simulate scanline glitches)
            offset.y += Random.Range(-1f, 1f) * jitterSpeed * Time.deltaTime;

            // occasionally scale bands for chunkier breaks
            if (Random.value < 6f * Time.deltaTime)
                tiling.y = Random.Range(bandScale.y * 0.6f, bandScale.y * 1.8f);

            // apply to material (no instance cloning thanks to MPB)
            rend.GetPropertyBlock(mpb);
            mpb.SetColor(BaseColor, new Color(baseColor.r, baseColor.g, baseColor.b, a));
            mpb.SetVector(MainTex_ST, new Vector4(tiling.x, tiling.y, offset.x, offset.y));
            rend.SetPropertyBlock(mpb);
        }
    }

    void ScheduleNextBurst()
    {
        if (avgBurstsPerSecond <= 0f) { nextBurstAt = float.PositiveInfinity; return; }
        // exponential spacing around the given average rate feels organic
        float wait = Random.Range(0.1f, 1.5f) / Mathf.Max(0.01f, avgBurstsPerSecond);
        nextBurstAt = Time.time + wait;
    }
}
