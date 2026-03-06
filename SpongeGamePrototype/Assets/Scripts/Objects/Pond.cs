using UnityEngine;
using System.Collections;
public class Pond : MonoBehaviour
{
    public enum Liquids { WATER, FERTILIZER, HERBICIDE, PAINT }
    private float waterFill = 0;
    public float maxWaterFill = 2;
    public Color Water;
    public Color FerTILIZER;
    public Color HERBICIDE;
    public Color Paint;

    public LiquidReceiver liquidReciever;

    // Edge-trigger memory (avoid calling every frame)
    private bool _lastWatered;
    private bool _lastFertilized;
    private bool _lastHerbicided;
    private bool _lastPainted;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (liquidReciever == null)
            liquidReciever = GetComponent<LiquidReceiver>(); // Find from itself
        pondWater.transform.localScale = new Vector3(1, waterFill/maxWaterFill,1);
        //interaction(Liquids.FERTILIZER);
    }

    // Update is called once per frame
    void Update()
    {
        reciveLiquid();
    }

    public void reciveLiquid()
    {
        if (liquidReciever == null) return;

        // Read outputs from LiquidReceiver (the ONLY driver)
        bool wateredNow = liquidReciever.Watered;
        bool fertilizedNow = liquidReciever.Fertilized;
        bool herbicidedNow = liquidReciever.Herbicided;
        bool paintedNow = liquidReciever.Painted; // (ADD)

        // Rising-edge trigger: only when false -> true
        bool waterPressed = wateredNow && !_lastWatered;
        bool fertPressed = fertilizedNow && !_lastFertilized;
        bool herbPressed = herbicidedNow && !_lastHerbicided;
        bool paintPressed = paintedNow && !_lastPainted; // (ADD)

        // Update last states
        _lastWatered = wateredNow;
        _lastFertilized = fertilizedNow;
        _lastHerbicided = herbicidedNow;
        _lastPainted = paintedNow; // (ADD)

        // If multiple become true in same frame, pick a priority.
        // (Usually only one should be true anyway.)
        if (paintPressed)
        {
            poundInteraction(Liquids.PAINT);
        }
        else if (herbPressed)
        {
            poundInteraction(Liquids.HERBICIDE);
        }
        else if (fertPressed)
        {
            poundInteraction(Liquids.FERTILIZER);
        }
        else if (waterPressed)
        {
            poundInteraction(Liquids.WATER);
        }
    }



    public void poundInteraction(Liquids liquid)
    {
        if (pondWater == null) return;

        // no hardcode color: always follow player's current color
        var r = pondWater.transform.GetChild(0).gameObject.GetComponent<Renderer>();
        if (r != null) r.material.color = liquidReciever.color;

        // all liquids can fill the pond (no modulo, so it can reach full)
        waterFill = Mathf.Clamp(waterFill + 1f, 0f, maxWaterFill);

        float targetY = (maxWaterFill <= 0f) ? 0f : (waterFill / maxWaterFill);

        // if you spam liquids, multiple coroutines can fight; if you want it stable, stop previous one (optional)
        StartCoroutine(growShrink(targetY, 1));
    }


    public GameObject pondWater;
    public float growShrinkRate;
    IEnumerator growShrink(float scale, int growShrink)
    {
        //Debug.Log(pondWater.transform.localScale.y);
        while (pondWater.transform.localScale.y < scale)//while it is below a scale size,
                                                         //grow/shrink with respect to growShrink integer (either 1 or -1)
        {
            //Debug.Log("growing");
            //grow the bush by a certain amount
            pondWater.transform.localScale += growShrink * new Vector3(0,1,0) * growShrinkRate * Time.deltaTime;
            yield return null;//wait a frame
        }
    }
}
