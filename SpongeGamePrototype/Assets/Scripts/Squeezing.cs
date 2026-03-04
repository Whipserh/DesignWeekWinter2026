using UnityEngine;

public class Squeezing : MonoBehaviour
{
    [Header("Refs")]
    public CheckLiquid checkLiquid; // reads nearBucket + uiImage.color + idleColor
    public StarterAssets.FirstPersonController controller; // reads isSpongeHeld()/isSpongePress()

    [Header("Rules")]
    public bool requireSqueezePress = false; // true = isSpongePress(), false = isSpongeHeld()
    [Range(0f, 0.2f)] public float idleColorTolerance = 0.02f; // how close counts as "idleColor"

    [Header("Particle")]
    public ParticleSystem ps;              // the particle system to start/stop
    public bool syncParticleAlpha = true;  // keep alpha from UI color

    [Header("isSqueeze")]
    public bool isSqueeze;

    void Awake()
    {
        if (ps == null) ps = GetComponent<ParticleSystem>();
        if (ps != null) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void Update()
    {


        if (ps == null) return;

        bool nearBucket = (checkLiquid != null) && checkLiquid.nearBucket;

        bool squeeze = false;
        if (controller != null)
            squeeze = requireSqueezePress ? controller.isSpongePress() : controller.isSpongeHeld();

        // If we cannot read color state, treat as idle (more conservative).
        bool isIdleColor = true;
        Color currentColor = Color.white;

        if (checkLiquid != null && checkLiquid.uiImage != null)
        {
            currentColor = checkLiquid.uiImage.color;

            // Compare current UI color vs idleColor with tolerance.
            Color idle = checkLiquid.idleColor;

            float diff =
                Mathf.Abs(currentColor.r - idle.r) +
                Mathf.Abs(currentColor.g - idle.g) +
                Mathf.Abs(currentColor.b - idle.b) +
                Mathf.Abs(currentColor.a - idle.a);

            isIdleColor = diff <= idleColorTolerance;
        }

        // Show condition => start particle; otherwise stop it.
        bool show = (!nearBucket) && squeeze && (!isIdleColor);
        isSqueeze = show;
        if (show)
        {
            // Sync particle color to current UI color
            if (checkLiquid != null && checkLiquid.uiImage != null)
            {
                Color c = currentColor;
                if (!syncParticleAlpha) c.a = 1f;

                var main = ps.main;
                main.startColor = c;
            }

            if (!ps.isPlaying)
                ps.Play(true);
        }
        else
        {
            if (ps.isPlaying)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
    
}