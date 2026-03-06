using UnityEngine;
using UnityEngine.UI;

public class ColorProgressSlider : MonoBehaviour
{
    [Header("Refs")]
    public Slider slider;
    public CheckLiquid source;         // Drag your CheckLiquid here
    public Image fillImage;            // Drag Slider -> Fill Area -> Fill (Image) here

    [Header("Behavior")]
    public bool hideWhenIdle = true;
    [Range(0f, 0.05f)] public float idleEpsilon = 0.001f;

    [Header("Sync Color")]
    public bool syncFillColorWithUIImage = true;

    // NEW: use CheckLiquid.isAbsorbing to decide which progress to show
    [Header("Mode")]
    public bool useAbsorbingSwitch = true;

    void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();

        if (slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0f;
        }

        // If not assigned, try to auto-find the Fill Image from Slider hierarchy.
        if (fillImage == null && slider != null && slider.fillRect != null)
        {
            fillImage = slider.fillRect.GetComponent<Image>();
        }
    }

    void Update()
    {
        if (slider == null || source == null) return;

        float p;

        if (useAbsorbingSwitch)
        {
            // Absorbing: show fill progress (0->1)
            // Releasing: show remaining sponge amount (1->0)
            p = source.isAbsorbing ? source.ColorProgress01 : source.SpongeAmount01;
        }
        else
        {
            // Default: keep old behavior (fill progress)
            p = source.ColorProgress01;
        }

        p = Mathf.Clamp01(p);
        slider.value = p;

        if (hideWhenIdle)
        {
            slider.gameObject.SetActive(p > idleEpsilon);
        }

        if (syncFillColorWithUIImage && fillImage != null && source.uiImage != null)
        {
            fillImage.color = source.uiImage.color;
        }
    }
}