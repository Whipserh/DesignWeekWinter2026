using UnityEngine;

public class Flower : MonoBehaviour
{
    public enum Liquids { WATER, FERTILIZER, HERBICIDE }
    public int flowerHealth;
    public int maxFlowerHealth = 2;
    public GameObject flowerDead;
    public GameObject flowerAlive;
    public GameObject flowerBloom;

    // Edge-trigger memory (avoid calling every frame)
    private bool _lastWatered;
    private bool _lastFertilized;
    private bool _lastHerbicided;

    [Header("Reciever")]
    public LiquidReceiver liquidReciever;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (liquidReciever == null)
            liquidReciever = GetComponent<LiquidReceiver>(); // Find from itself

        // Initiallize the flower
        ApplyVisualByHealth();
    }


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
           
            flowerInteraction(Liquids.HERBICIDE);
        }
        else if (fertPressed)
        {
           // Debug.Log()
            flowerInteraction(Liquids.FERTILIZER);
        }
        else if (waterPressed)
        {
            Debug.Log("Watering Flowers");
            flowerInteraction(Liquids.WATER);
        }
    }

    private void ApplyVisualByHealth()
    {
        if (flowerHealth <= 0)
        {
            flowerDead.SetActive(true);
            flowerAlive.SetActive(false);
            flowerBloom.SetActive(false);
        }
        else if (flowerHealth == 1)
        {
            flowerDead.SetActive(false);
            flowerAlive.SetActive(true);
            flowerBloom.SetActive(false);
        }
        else // >=2
        {
            flowerDead.SetActive(false);
            flowerAlive.SetActive(true);
            flowerBloom.SetActive(true);
        }
    }


    public void flowerInteraction(Liquids liquid)
    {
        switch (liquid)
        {
            case Liquids.WATER:
                flowerHealth = Mathf.Clamp(flowerHealth + 1, 0, maxFlowerHealth);
                break;

            case Liquids.FERTILIZER:
                flowerHealth = Mathf.Clamp(flowerHealth + 1, 0, maxFlowerHealth);
                break;

            case Liquids.HERBICIDE:
                flowerHealth = 0; // herbicide kills instantly
                break;
        }

        // 关键：状态变化后刷新外观（只在需要时调用）
        ApplyVisualByHealth();
    }

    //public float growShrinkRate;
    //IEnumerator growShrink(float scale, int growShrink)
    //{



    //    //while (bushLeaves.transform.localScale.x < scale)//while it is below a scale size,
    //    //                                                 //grow/shrink with respect to growShrink integer (either 1 or -1)
    //    //{
    //    //    //grow the bush by a certain amount
    //    //    bushLeaves.transform.localScale += growShrink * Vector3.one * growShrinkRate * Time.deltaTime;
    //    //    yield return null;//wait a frame
    //    //}
    //}
}
