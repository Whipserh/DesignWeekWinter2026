using UnityEngine;

public class PaintableObject : MonoBehaviour
{

    private Renderer render;
    public enum Liquids { WATER, FERTILIZER, HERBICIDE, PAINT }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        render = GetComponent<Renderer>();
        //interact(Liquids.PAINT, new Color(255, 0, 0));
    }

    public void interact(Liquids liquid, Color color)
    {
        switch(liquid) {
            case Liquids.PAINT:
                paint(color);
                break;
            case Liquids.WATER: paint(new Color(0,0,0)); break;
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
