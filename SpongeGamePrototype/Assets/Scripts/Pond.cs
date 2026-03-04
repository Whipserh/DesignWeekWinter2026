using UnityEngine;
using System.Collections;
public class Pond : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pondWater.transform.localScale = new Vector3(1, waterFill/maxWaterFill,1);
        //interaction(Liquids.FERTILIZER);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum Liquids { WATER, FERTILIZER, HERBICIDE, PAINT }
    private float waterFill = 0;
    public float maxWaterFill = 2;
    public Color Water;
    public Color FerTILIZER;
    public Color HERBICIDE;
    public Color Paint;


    public void interaction(Liquids liquid)
    {
        switch (liquid)
        {
            case Liquids.WATER://grow when water is added
                pondWater.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = Water;
                break;
            case Liquids.FERTILIZER:
                pondWater.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = FerTILIZER;
                break;
            case Liquids.HERBICIDE://shrink when herbicide is added
                pondWater.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = HERBICIDE;
                break;
            case Liquids.PAINT: 
                pondWater.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = Paint;
                break;
        }
        waterFill = (waterFill+1)% maxWaterFill;
        //Debug.Log(waterFill / maxWaterFill);
        StartCoroutine(growShrink(waterFill/maxWaterFill, 1));
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
