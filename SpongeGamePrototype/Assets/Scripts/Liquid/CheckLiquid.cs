using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheckLiquid : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam;
    public LayerMask targetLayers;   // Now is everything, but we can shrink to certain object types
    public TMP_Text label;
    public Image uiImage;              // The coloring UI Image
    public StarterAssets.FirstPersonController controller; // 用来读 isSpongeHeld()

    [Header("Rules")]
    public float maxAimDistance = 8f;        // the maxium ray detecting distance
    public float requireNearDistance = 3f;   // the character need to be close enough
    [Range(-1f, 1f)] public float facingDot = 0.85f; // A facting clamp

    [Header("Colors")]
    public Color waterColor = new Color(0.2f, 0.5f, 1f, 1f);      // blue
    public Color fertColor = new Color(1f, 0.65f, 0.15f, 1f);    // orange
    public Color herbColor = new Color(0.55f, 0.05f, 0.05f, 1f); // dark red
    public Color paint;
    public Color idleColor = new Color(1f, 1f, 1f, 0.15f);       // defult

    public float colorLerpTime = 0.6f;

    private Coroutine _colorRoutine;

    // Remember the last "completed" color. If player releases before completion, we revert back to this color.
    private Color _lastCommittedColor;

    // Track sponge state to detect release.
    private bool _spongeHeldLastFrame = false;

    // Track what we are currently transitioning towards.
    private EnemyKind? _currentTargetKind = null;

    // Completed state lock:
    // When completed, progress stays at 1 and will not reverse unless a NEW kind starts.
    private EnemyKind? _completedKind = null;

    // Exposed progress (0..1) for UI slider: progress towards fully changed color.
    public float ColorProgress01 { get; private set; } = 0f;

    // Track whether we are currently transitioning forward or reversing.
    private enum ColorTransitionMode { None, ForwardToTarget, ReverseToCommitted }
    private ColorTransitionMode _transitionMode = ColorTransitionMode.None;

    [Header("Absorb Mod")]
    public bool nearBucket;
    public bool isAbsorbing;

    [Header("Aimming Liquid")]
    public bool atWater;
    public bool atFert;
    public bool atHerb;
    public bool atPaint;

    [Header("Ready When Fully Colored")]
    public bool waterReady;
    public bool fertReady;
    public bool herbReady;
    public bool paintReady;

    // ---------------- NEW: Sponge Capacity ----------------
    // 玩家吸满一个 liquid 时：SpongeAmount01 = 1
    // 玩家 squeeze 喷出：SpongeAmount01 逐渐减少（在 Squeezing 脚本里做）
    // ready 在吸满时才 true，在喷到 0 时才取消（在 Squeezing 脚本里做）
    [Header("Sponge Capacity")]
    [Range(0f, 1f)]
    public float SpongeAmount01 = 0f;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        SetLabel(false, "");
        SetAllAtFalse();
        SetImageColor(idleColor);

        // Initial UI color is also the initial committed color.
        _lastCommittedColor = idleColor;

        // Initial progress is 0.
        ColorProgress01 = 0f;
        _completedKind = null;

        // NEW: initial sponge amount is 0
        SpongeAmount01 = 0f;
        isAbsorbing = false;
    }

    void Update()
    {
        if (cam == null || label == null)
        {
            SetLabel(false, "");
            SetAllAtFalse();

            // If we lose refs mid-transition, treat it like a release.
            _spongeHeldLastFrame = false;
            return;
        }

        // 1) Aiming Detection Ray(ViewPort)
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, maxAimDistance, targetLayers, QueryTriggerInteraction.Ignore))
        {
            SetLabel(false, "");
            SetAllAtFalse();

            _spongeHeldLastFrame = false;
            return;
        }

        // 2) Get Enemy type including the parent
        EnemyType enemy = null;
        if (!hit.collider.TryGetComponent(out enemy))
        {
            enemy = hit.collider.GetComponentInParent<EnemyType>();
        }

        if (enemy == null)
        {
            SetLabel(false, "");
            SetAllAtFalse();

            _spongeHeldLastFrame = false;
            return;
        }

        // 3) Distance + facing
        Vector3 playerPos =
            (controller != null) ? controller.transform.position :
            (cam != null) ? cam.transform.position :
            transform.position;

        // 用 collider 最近点，不用 enemy.transform.position（pivot 可能偏）
        Vector3 closest = hit.collider.ClosestPoint(playerPos);
        float dist = Vector3.Distance(playerPos, closest);

        if (dist > requireNearDistance)
        {
            SetLabel(false, "");
            SetAllAtFalse();
            //HandleReleaseOrInvalidState();
            _spongeHeldLastFrame = false;
            return;
        }

        Vector3 toTarget = (enemy.transform.position - cam.transform.position).normalized;
        float dot = Vector3.Dot(cam.transform.forward, toTarget);
        if (dot < facingDot)
        {
            SetLabel(false, "");
            SetAllAtFalse();

            _spongeHeldLastFrame = false;
            return;
        }

        // 4) Satify conditions：Show words + Show bool
        SetAllAtFalse(); // Clear first (IMPORTANT: do NOT clear after setting nearBucket)
        nearBucket = true;

        SetLabel(true, enemy.kind.ToString());
        switch (enemy.kind)
        {
            case EnemyKind.Water: atWater = true; break;
            case EnemyKind.Fertilizer: atFert = true; break;
            case EnemyKind.Herbicide: atHerb = true; break;
            case EnemyKind.Paint: atPaint = true; break;
        }

        // 5) Changing color when the player squeeze
        if ((controller != null) && controller.isSpongeReleased())
        {
            // FIX: 只有海绵空（或你希望允许“覆盖吸水”时改掉这个条件）才允许开始吸水
            if (SpongeAmount01 <= 0.0001f)
            {
                isAbsorbing = true;

                Color target = GetTargetColor(enemy);
                StartSmoothColorTo(enemy.kind, target);
                _spongeHeldLastFrame = true;
            }
        }
        else _spongeHeldLastFrame = false;
    }

    void SetAllAtFalse()
    {
        atWater = false;
        atFert = false;
        atHerb = false;
        atPaint = false;
        //
        nearBucket = false;
    }

    void SetLabel(bool on, string text)
    {
        if (label.enabled != on) label.enabled = on;
        label.text = on ? text : "";
    }

    void SetImageColor(Color c)
    {
        if (uiImage == null) return;
        uiImage.color = c;
    }

    //--------Changing the color------------/
    //</Summary> I want to change the color smoothly/
    private void ResetReadyBools()
    {
        waterReady = false;
        fertReady = false;
        herbReady = false;
        paintReady = false;
    }

    private void SetReadyBool(EnemyKind kind, bool value)
    {
        //Only allow 1 true
        waterReady = false;
        fertReady = false;
        herbReady = false;
        paintReady = false;

        if (!value) return;

        switch (kind)
        {
            case EnemyKind.Water: waterReady = true; break;
            case EnemyKind.Fertilizer: fertReady = true; break;
            case EnemyKind.Herbicide: herbReady = true; break;
            case EnemyKind.Paint: paintReady = true; break;
        }
    }

    private Color GetKindColor(EnemyKind kind)
    {
        switch (kind)
        {
            case EnemyKind.Water: return waterColor;
            case EnemyKind.Fertilizer: return fertColor;
            case EnemyKind.Herbicide: return herbColor;
            case EnemyKind.Paint: return paint;
            default: return uiImage != null ? uiImage.color : Color.white;
        }
    }

    private void StartSmoothColorTo(EnemyKind kind, Color targetColor)
    {
        if (uiImage == null) return;

        // FIX: only lock while sponge still has liquid. If empty, allow absorb again.
        if (_completedKind.HasValue && _completedKind.Value == kind && SpongeAmount01 > 0.0001f)
        {
            ColorProgress01 = 1f;
            isAbsorbing = false; // safety
            return;
        }

        // If we are already going forward to the same target, do not restart.
        if (_transitionMode == ColorTransitionMode.ForwardToTarget &&
            _currentTargetKind.HasValue &&
            _currentTargetKind.Value == kind &&
            _colorRoutine != null)
        {
            return;
        }

        _currentTargetKind = kind;

        // Not completed yet => ready flags must be false.
        ResetReadyBools();

        // Any new forward attempt invalidates completed lock.
        _completedKind = null;

        // Interrupt any existing transition (forward or reverse) and start forward.
        if (_colorRoutine != null)
        {
            StopCoroutine(_colorRoutine);
            _colorRoutine = null;
        }

        _transitionMode = ColorTransitionMode.ForwardToTarget;
        ColorProgress01 = 0f;

        _colorRoutine = StartCoroutine(CoLerpColorForward(kind, targetColor, colorLerpTime));
    }

    private System.Collections.IEnumerator CoLerpColorForward(EnemyKind kind, Color target, float duration)
    {
        Color start = uiImage.color;

        float t = 0f;
        ColorProgress01 = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, duration);
            float u = Mathf.Clamp01(t);

            // Progress towards fully changed color (0..1).
            ColorProgress01 = u;

            uiImage.color = Color.Lerp(start, target, u);
            yield return null;
        }

        // Be the target color, avoid float diff issues
        uiImage.color = target;

        // Commit the result: from now on, this is the "previous color" we can revert to.
        _lastCommittedColor = target;

        // Only becomes true when fully colored
        SetReadyBool(kind, true);

        // Fill complete => Sponge is FULL
        SpongeAmount01 = 1f;

        // Lock completed state to prevent restarting/looping.
        _completedKind = kind;

        ColorProgress01 = 1f;

        isAbsorbing = false;//absorbing stopped

        _transitionMode = ColorTransitionMode.None;
        _colorRoutine = null;
        _currentTargetKind = null;
    }

    private Color GetTargetColor(EnemyType enemy)
    {
        if (enemy == null) return uiImage != null ? uiImage.color : Color.white;

        Color c = (enemy.kind == EnemyKind.Paint)
            ? enemy.paintColor
            : GetKindColor(enemy.kind);

        // 强制不透明，避免 alpha=0 导致“消失”
        c.a = 1f;
        return c;
    }

    public void OnSpongeEmptied()
    {
        // If absorb coroutine is running, stop it
        if (_colorRoutine != null)
        {
            StopCoroutine(_colorRoutine);
            _colorRoutine = null;
        }

        // Unlock the "completed" lock so we can absorb again
        _completedKind = null;
        _currentTargetKind = null;
        _transitionMode = ColorTransitionMode.None;

        // Reset fill progress for next absorb cycle
        ColorProgress01 = 0f;

        // Return to idle visuals
        _lastCommittedColor = idleColor;
        SetImageColor(idleColor);

        // Absorb state ends
        isAbsorbing = false;
    }
}