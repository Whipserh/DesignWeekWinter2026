using UnityEngine;

public class PaintableObject : MonoBehaviour
{
    public LiquidReceiver LiquidReceiver;
    private Renderer render;
    public enum Liquids { WATER, FERTILIZER, HERBICIDE, PAINT }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (LiquidReceiver == null)
            LiquidReceiver = GetComponentInChildren<LiquidReceiver>(true);

        render = GetComponent<Renderer>();
        //interact(Liquids.PAINT, new Color(255, 0, 0));
    }

    // Edge-trigger memory (avoid calling every frame)
    private bool _lastWatered;
    private bool _lastFertilized;
    private bool _lastHerbicided;
    private bool _lastPainted;

    private void Update()
    {
        if (LiquidReceiver == null) return;

        // Read outputs from LiquidReceiver (the ONLY driver)
        bool wateredNow = LiquidReceiver.Watered;
        bool fertilizedNow = LiquidReceiver.Fertilized;
        bool herbicidedNow = LiquidReceiver.Herbicided;
        bool paintedNow = LiquidReceiver.Painted;

        // Rising-edge trigger: only when false -> true
        bool waterPressed = wateredNow && !_lastWatered;
        bool fertPressed = fertilizedNow && !_lastFertilized;
        bool herbPressed = herbicidedNow && !_lastHerbicided;
        bool paintPressed = paintedNow && !_lastPainted;

        // Update last states
        _lastWatered = wateredNow;
        _lastFertilized = fertilizedNow;
        _lastHerbicided = herbicidedNow;
        _lastPainted = paintedNow; // (FIX) you must track painted too

        // If multiple become true in same frame, pick a priority.
        // (Usually only one should be true anyway.)
        if (paintPressed) // (FIX) paint was never handled before
        {
            interact(Liquids.PAINT, LiquidReceiver.color);
        }
        else if (herbPressed)
        {
            interact(Liquids.HERBICIDE, LiquidReceiver.color);
        }
        else if (fertPressed)
        {
            interact(Liquids.FERTILIZER, LiquidReceiver.color);
        }
        else if (waterPressed)
        {
            interact(Liquids.WATER, LiquidReceiver.color);
        }
    }

    public void interact(Liquids liquid, Color color)
    {
        switch (liquid)
        {
            case Liquids.HERBICIDE: return;
            case Liquids.FERTILIZER: return;

            case Liquids.PAINT:
                paint(color);
                break;

            // Unity Color is 0..1, so use Color.white instead of (255,255,255)
            case Liquids.WATER:
                paint(Color.white);
                break;
        }
    }

    public void paint(Color color)
    {
        foreach (Transform childTransform in this.transform)
        {
            Renderer w = childTransform.GetComponent<Renderer>();
            if (w != null) w.material.color = color;
        }
        if (render != null) render.material.color = color;
    }
}