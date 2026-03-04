using UnityEngine;

public class Flower : MonoBehaviour
{
    public enum Liquids { WATER, FERTILIZER, HERBICIDE }
    public int flowerHealth;
    public int maxFlowerHealth = 2;
    public GameObject flowerDead;
    public GameObject flowerAlive;
    public GameObject flowerBloom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //flowerHealth = 0;
        if (flowerHealth == 1)
        {
            flowerDead.SetActive(true);
            flowerAlive.SetActive(false);
            flowerBloom.SetActive(false);
        }
        //StartCoroutine(growShrink(0.5f, 1));
        flowerInteraction(Liquids.HERBICIDE);
    }



    public void flowerInteraction(Liquids liquid)
    {
        switch (liquid)
        {
            case Liquids.WATER://grow when water is added
                flowerHealth = Mathf.Clamp(flowerHealth + 1, 0, maxFlowerHealth);
                if (flowerHealth==1)
                {
                    flowerDead.SetActive(false);
                    flowerAlive.SetActive(true);
                    flowerBloom.SetActive(false);
                } 
                else if (flowerHealth==2)
                {
                    flowerDead.SetActive(false);
                    flowerAlive.SetActive(true);
                    flowerBloom.SetActive(true);
                }
                    //StartCoroutine(growShrink(0.5f, 1)); else StartCoroutine(growShrink(1, 1));
                break;
            case Liquids.FERTILIZER:
                flowerHealth = Mathf.Clamp(flowerHealth + 1, 0, maxFlowerHealth);
                if (flowerHealth == 1)
                {
                    flowerDead.SetActive(false);
                    flowerAlive.SetActive(true);
                    flowerBloom.SetActive(false);
                }
                else if (flowerHealth == 2)
                {
                    flowerDead.SetActive(false);
                    flowerAlive.SetActive(true);
                    flowerBloom.SetActive(true);
                }
                //StartCoroutine(growShrink(0.5f, 1)); else StartCoroutine(growShrink(1, 1));
                break;
            case Liquids.HERBICIDE://shrink when herbicide is added
                flowerHealth = Mathf.Clamp(flowerHealth - 1, 0, maxFlowerHealth);
                if (flowerHealth == 1)
                {
                    flowerDead.SetActive(false);
                    flowerAlive.SetActive(true);
                    flowerBloom.SetActive(false);
                }
                else if (flowerHealth == 0)
                {
                    flowerDead.SetActive(true);
                    flowerAlive.SetActive(false);
                    flowerBloom.SetActive(false);
                }
                //StartCoroutine(growShrink(0.5f, -1)); else StartCoroutine(growShrink(0, -1));
                break;

        }
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
