using System.Collections;
using UnityEngine;

public class Bush : MonoBehaviour
{
    public enum Liquids {WATER, FERTILIZER, HERBICIDE}
    private int bushHealth = 0;
    public int maxBushHealth = 2;
    public GameObject bushLeaves;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(bushHealth==0) bushLeaves.transform.localScale = Vector3.zero;
        //StartCoroutine(growShrink(0.5f, 1));
        bushInteraction(Liquids.WATER);
    }

    //getComponent<Bush>().bushInteraction(WATER);
    public void bushInteraction(Liquids liquid)
    {
        switch (liquid)
        {
            case Liquids.WATER://grow when water is added
                bushHealth = (bushHealth + 1) % maxBushHealth;
                if (bushHealth < 2) StartCoroutine(growShrink(0.5f, 1)); else StartCoroutine(growShrink(1, 1));
                    break;
            case Liquids.FERTILIZER:
                  bushHealth = (bushHealth + 1) % maxBushHealth;
                if (bushHealth < 2) StartCoroutine(growShrink(0.5f, 1)); else StartCoroutine(growShrink(1, 1));
                break;
            case Liquids.HERBICIDE://shrink when herbicide is added
                bushHealth = (bushHealth-1)%maxBushHealth;
                if (bushHealth > 0) StartCoroutine(growShrink(0.5f, -1)); else StartCoroutine(growShrink(0, -1));
                break;

        }
    }
    
    public float growShrinkRate;
    IEnumerator growShrink(float scale, int growShrink)
    {
        if (growShrink == 1)
        while (bushLeaves.transform.localScale.x < scale)//while it is below a scale size,
                                                         //grow/shrink with respect to growShrink integer (either 1 or -1)
        {
            //grow the bush by a certain amount
            bushLeaves.transform.localScale += growShrink * Vector3.one * growShrinkRate * Time.deltaTime;
            yield return null;//wait a frame
        }
        if (growShrink == -1)
            while (bushLeaves.transform.localScale.x > scale)//while it is below a scale size,
                                                             //grow/shrink with respect to growShrink integer (either 1 or -1)
            {
                //grow the bush by a certain amount
                bushLeaves.transform.localScale += growShrink * Vector3.one * growShrinkRate * Time.deltaTime;
                yield return null;//wait a frame
            }
    }

}
