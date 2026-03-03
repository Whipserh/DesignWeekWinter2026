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

        float p = Mathf.Clamp01(source.ColorProgress01);
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