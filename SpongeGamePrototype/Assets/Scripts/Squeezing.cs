using UnityEngine;

/*
 * This class exclusively hands the particles and UI when the player squeezes the sponge
 */

public class Squeezing : MonoBehaviour
{
    [Header("Refs")]
    public CheckLiquid checkLiquid; // reads nearBucket + uiImage.color + idleColor
    public StarterAssets.FirstPersonController controller; // reads isSpongeHeld()/isSpongePress()

    [Header("Rules")]
    public bool requireSqueezePress = false; // true = isSpongePress(), false = isSpongeHeld()
    [Range(0f, 0.2f)] public float idleColorTolerance = 0.02f; // how close counts as "idleColor"

    [Header("Particle")]
    public ParticleSystem particlesSystem;              // the particle system to start/stop
    public bool syncParticleAlpha = true;  // keep alpha from UI color

    [Header("isSqueeze")]
    public bool isSqueeze;

    // ---------------- NEW: Drain ----------------
    [Header("Liquid Drain")]
    public float drainSpeed = 0.5f; // SpongeAmount01 drains per second when squeezing

    // ---------------- NEW: Color Fade Back To Idle ----------------
    // Cache the "full liquid" color at the moment we START squeezing,
    // then fade back to idle as SpongeAmount01 decreases.
    private Color _drainStartColor = Color.white;
    private bool _hasDrainStartColor = false;

    void Awake()
    {
        if (particlesSystem == null) particlesSystem = GetComponent<ParticleSystem>();
        if (particlesSystem != null) particlesSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void Start()
    {
        checkLiquid = GetComponent<CheckLiquid>();
    }


    void Update()
    {
        if (particlesSystem == null) return;

        // If we are near bucket, we are in absorb mode; reset drain cache so next squeeze re-captures fresh color.
        if (checkLiquid != null && checkLiquid.nearBucket)
        {
            _hasDrainStartColor = false;
        }

        // If we cannot read color state, treat as idle (more conservative).
        bool isIdleColor = true;
        Color currentColor = Color.white;

        if (checkLiquid.uiImage != null)
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

        //this references the squeeze
        bool squeeze = false;
        if (controller != null)
            squeeze = requireSqueezePress ? controller.isSpongePress() : controller.isSpongeHeld();
        else Debug.LogError("There was no controller attached.");

        // Show condition => start particle; otherwise stop it.
        // must have SpongeAmount01 > 0 to squeeze output
        isSqueeze = (!checkLiquid.nearBucket) && squeeze && (!isIdleColor) && (checkLiquid.SpongeAmount01 > 0f);
        if (isSqueeze)
        {
            // Cache the color at the moment we START draining (so we fade from THIS color back to idle)
            if (!_hasDrainStartColor && checkLiquid.uiImage != null)
            {
                _drainStartColor = checkLiquid.uiImage.color;
                _hasDrainStartColor = true;
            }

            // Sync particle color to current UI color
            if (checkLiquid.uiImage != null)
            {
                // NEW: Fade UI color back to idle according to remaining amount
                // amount=1 => drainStartColor; amount=0 => idleColor
                float a = Mathf.Clamp01(checkLiquid.SpongeAmount01);
                Color faded = Color.Lerp(checkLiquid.idleColor, _drainStartColor, a);

                // Apply to UI immediately (gradual, per frame)
                checkLiquid.uiImage.color = faded;

                // Particles follow UI color
                Color c = faded;
                if (!syncParticleAlpha) c.a = 1f;

                var main = particlesSystem.main;
                main.startColor = c;
            }

            if (!particlesSystem.isPlaying)
                particlesSystem.Play(true);

            // Drain sponge amount while squeezing
            if (checkLiquid.SpongeAmount01 > 0f)
            {
                checkLiquid.SpongeAmount01 -= drainSpeed * Time.deltaTime;
                checkLiquid.SpongeAmount01 = Mathf.Clamp01(checkLiquid.SpongeAmount01);
            }

            // Only cancel when reaches 0
            if (checkLiquid.SpongeAmount01 <= 0f)
            {
                checkLiquid.SpongeAmount01 = 0f;

                // Cancel ready ONLY when fully drained to 0
                checkLiquid.waterReady = false;
                checkLiquid.fertReady = false;
                checkLiquid.herbReady = false;
                checkLiquid.paintReady = false;

                // At 0, UI will already be at idle due to lerp, but keep it exact
                if (checkLiquid.uiImage != null)
                    checkLiquid.uiImage.color = checkLiquid.idleColor;

                // Reset cache for next fill/squeeze cycle
                _hasDrainStartColor = false;

                // Stop particles when empty
                if (particlesSystem.isPlaying)
                    particlesSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
        else
        {
            if (particlesSystem.isPlaying)
                particlesSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}