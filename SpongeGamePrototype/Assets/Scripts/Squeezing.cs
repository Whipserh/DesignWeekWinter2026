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




        // Show condition => start particle; otherwise stop it.
        isSqueeze = (!checkLiquid.nearBucket) && squeeze && (!isIdleColor);
        if (isSqueeze)
        {
            // Sync particle color to current UI color
            if (checkLiquid.uiImage != null)
            {
                Color c = currentColor;
                if (!syncParticleAlpha) c.a = 1f;

                var main = particlesSystem.main;
                main.startColor = c;
            }

            if (!particlesSystem.isPlaying)
                particlesSystem.Play(true);
        }
        else
        {
            if (particlesSystem.isPlaying)
                particlesSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
    
}