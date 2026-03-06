using UnityEngine;
using UnityEngine.UI;

public class SyncFillColor : MonoBehaviour
{
    [Header("Source")]
    public CheckLiquid source;   // 玩家颜色来源

    private Image fillImage;

    void Awake()
    {
        fillImage = GetComponent<Image>();
    }

    void Update()
    {
        if (source == null) return;
        if (source.uiImage == null) return;

        fillImage.color = source.uiImage.color;
    }
}