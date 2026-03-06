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

    // NEW: keep UI alive (avoid SetActive which resets animations)
    private CanvasGroup _cg;

    // NEW: small hysteresis to avoid flicker when isAbsorbing toggles briefly
    [Header("Stability")]
    [Range(0.90f, 0.999f)] public float absorbCompleteThreshold = 0.98f;
    private bool _lockAbsorbView = false;

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

        // CanvasGroup for hide/show without disabling object (keeps animations alive)
        _cg = GetComponent<CanvasGroup>();
        if (_cg == null) _cg = gameObject.AddComponent<CanvasGroup>();
        _cg.alpha = 1f;
        _cg.interactable = true;
        _cg.blocksRaycasts = true;
    }

    void Update()
    {
        if (slider == null || source == null) return;

        float absorbP = Mathf.Clamp01(source.ColorProgress01);
        float amountP = Mathf.Clamp01(source.SpongeAmount01);

        float p;

        if (useAbsorbingSwitch)
        {
            // If absorbing, show absorb progress (0->1)
            // Otherwise, show remaining amount (1->0)
            if (source.isAbsorbing)
            {
                _lockAbsorbView = true;
                p = absorbP;

                // When absorb completes, unlock so we can show amount next frame
                if (absorbP >= absorbCompleteThreshold)
                    _lockAbsorbView = false;
            }
            else
            {
                // If we were absorbing last frame and it's nearly complete, keep showing absorbP briefly
                // (prevents flicker when isAbsorbing drops for 1 frame)
                if (_lockAbsorbView)
                {
                    p = absorbP;
                    if (absorbP >= absorbCompleteThreshold)
                        _lockAbsorbView = false;
                }
                else
                {
                    p = amountP;
                }
            }
        }
        else
        {
            // Default: keep old behavior (fill progress)
            p = absorbP;
        }

        p = Mathf.Clamp01(p);
        slider.value = p;

        if (hideWhenIdle)
        {
            // Hide visually, but keep object active (no animation reset)
            bool show = p > idleEpsilon;
            _cg.alpha = show ? 1f : 0f;
            _cg.blocksRaycasts = show;
            _cg.interactable = show;
        }

        if (syncFillColorWithUIImage && fillImage != null && source.uiImage != null)
        {
            fillImage.color = source.uiImage.color;
        }
    }
}