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

    [Header("Liquid Drain")]
    public float drainSpeed = 0.5f; // SpongeAmount01 drains per second when squeezing

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

        // ---------------- FIX: Drain condition MUST NOT depend on isIdleColor ----------------
        bool canDrain = (!checkLiquid.nearBucket) && squeeze && (checkLiquid.SpongeAmount01 > 0f);

        // 粒子是否显示，你可以继续用 isIdleColor 作为“视觉门槛”
        bool showParticles = canDrain && (!isIdleColor);

        // 这是对外显示的 isSqueeze（你原来用来 debug/看状态的）
        isSqueeze = canDrain;

        if (canDrain)
        {
            // Releasing stage => slider should show SpongeAmount01
            checkLiquid.isAbsorbing = false;

            // Cache the color at the moment we START draining (so we fade from THIS color back to idle)
            if (!_hasDrainStartColor && checkLiquid.uiImage != null)
            {
                _drainStartColor = checkLiquid.uiImage.color;
                _hasDrainStartColor = true;
            }

            // Fade UI color back to idle according to remaining amount
            if (checkLiquid.uiImage != null)
            {
                float a = Mathf.Clamp01(checkLiquid.SpongeAmount01);
                Color faded = Color.Lerp(checkLiquid.idleColor, _drainStartColor, a);

                checkLiquid.uiImage.color = faded;

                // Particles follow UI color
                if (showParticles)
                {
                    Color c = faded;
                    if (!syncParticleAlpha) c.a = 1f;

                    var main = particlesSystem.main;
                    main.startColor = c;
                }
            }

            // Start/Stop particle system based on showParticles
            if (showParticles)
            {
                if (!particlesSystem.isPlaying)
                    particlesSystem.Play(true);
            }
            else
            {
                if (particlesSystem.isPlaying)
                    particlesSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            // Drain sponge amount while squeezing (ALWAYS drains while canDrain)
            checkLiquid.SpongeAmount01 -= drainSpeed * Time.deltaTime;
            checkLiquid.SpongeAmount01 = Mathf.Clamp01(checkLiquid.SpongeAmount01);

            // Only cancel when reaches 0
            if (checkLiquid.SpongeAmount01 <= 0f)
            {
                checkLiquid.SpongeAmount01 = 0f;

                // Cancel ready ONLY when fully drained to 0
                checkLiquid.waterReady = false;
                checkLiquid.fertReady = false;
                checkLiquid.herbReady = false;
                checkLiquid.paintReady = false;

                // Ensure exact idle
                if (checkLiquid.uiImage != null)
                    checkLiquid.uiImage.color = checkLiquid.idleColor;

                _hasDrainStartColor = false;

                if (particlesSystem.isPlaying)
                    particlesSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
        else
        {
            // Not draining => stop particles
            if (particlesSystem.isPlaying)
                particlesSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}