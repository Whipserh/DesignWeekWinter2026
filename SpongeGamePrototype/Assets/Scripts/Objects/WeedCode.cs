using System.Collections;
using UnityEngine;

public class WeedCode : MonoBehaviour
{
    public enum Liquids { WATER, FERTILIZER, HERBICIDE }

    [Header("Ref")]
    public LiquidReceiver LiquidReceiver;
    public GameObject Dead;
    public GameObject Alive;
    public GameObject CurrentWeedType;
    
    

    

    [Header("Bush Health")]
    [SerializeField] private int bushHealth = 0;
    public int maxBushHealth = 2;

    [Header("Grow/Shrink")]
    public float growShrinkRate = 2f;
    private Coroutine _scaleRoutine;

    // Edge-trigger memory (avoid calling every frame)
    private bool _lastWatered;
    private bool _lastFertilized;
    private bool _lastHerbicided;

    void Start()
    {
        if (LiquidReceiver == null)
            LiquidReceiver = GetComponentInChildren<LiquidReceiver>(true);

        if (CurrentWeedType == null)
            
            Debug.LogWarning($"Bush '{name}' has no bushLeaves assigned.", this);

        ApplyScaleImmediate();
    }

    void Update()
    {
        if (LiquidReceiver == null) return;

        // Read outputs from LiquidReceiver (the ONLY driver)
        bool wateredNow = LiquidReceiver.Watered;
        bool fertilizedNow = LiquidReceiver.Fertilized;
        bool herbicidedNow = LiquidReceiver.Herbicided;

        // Rising-edge trigger: only when false -> true
        bool waterPressed = wateredNow && !_lastWatered;
        bool fertPressed = fertilizedNow && !_lastFertilized;
        bool herbPressed = herbicidedNow && !_lastHerbicided;

        // Update last states
        _lastWatered = wateredNow;
        _lastFertilized = fertilizedNow;
        _lastHerbicided = herbicidedNow;

        // If multiple become true in same frame, pick a priority.
        // (Usually only one should be true anyway.)
        if (herbPressed)
        {
            bushInteraction(Liquids.HERBICIDE);
        }
        else if (fertPressed)
        {
            bushInteraction(Liquids.FERTILIZER);
        }
        else if (waterPressed)
        {
            bushInteraction(Liquids.WATER);
        }
    }

    // This is still useful internally, but it's only called by Update above.
    public void bushInteraction(Liquids liquid)
    {
        if (maxBushHealth < 1) maxBushHealth = 1;

        switch (liquid)
        {
            case Liquids.WATER:
            case Liquids.FERTILIZER:
                bushHealth = Mathf.Clamp(bushHealth + 1, 0, maxBushHealth);
                
                if(Dead.activeSelf == true)
                {
                    Dead.SetActive(false);
                    Alive.SetActive(true);
                }
                



                break;

            case Liquids.HERBICIDE:
                bushHealth = Mathf.Clamp(bushHealth - 1, 0, maxBushHealth);

                if(Alive.activeSelf == true)
                {
                    Dead.SetActive(true);
                    Alive.SetActive(false);
                }
               

                break;
        }

        float targetScale = GetScaleFromHealth();

        if (_scaleRoutine != null) StopCoroutine(_scaleRoutine);
        _scaleRoutine = StartCoroutine(GrowShrinkTo(targetScale));
    }

    private float GetScaleFromHealth()
    {
        // 0 -> 0, max -> 1
        return (float)bushHealth / maxBushHealth;
    }

    private void ApplyScaleImmediate()
    {
        if (CurrentWeedType == null) return;

        float s = GetScaleFromHealth();
        CurrentWeedType.transform.localScale = Vector3.one * s;
    }

    private IEnumerator GrowShrinkTo(float targetScale)
    {
        if (CurrentWeedType == null) yield break;

        Vector3 target = Vector3.one * Mathf.Clamp01(targetScale);

        while ((CurrentWeedType.transform.localScale - target).sqrMagnitude > 0.0001f)
        {
            CurrentWeedType.transform.localScale =
                Vector3.MoveTowards(CurrentWeedType.transform.localScale, target, growShrinkRate * Time.deltaTime);
            yield return null;
        }

        CurrentWeedType.transform.localScale = target;
        _scaleRoutine = null;
    }
}