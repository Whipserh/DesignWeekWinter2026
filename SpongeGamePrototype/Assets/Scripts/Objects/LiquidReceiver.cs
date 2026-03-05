

using System.Collections;
using UnityEngine;

/// <summary>
/// It is on the bush object. When player is close enough and squeeze their sponge,
/// they can release their liquid. This receiver reads what kind of liquid they are releasing,
/// and outputs bools.
/// </summary>
public class LiquidReceiver : MonoBehaviour
{
    [Header("Ref")]
    public GameObject Sponge;
    public GameObject LiquidController;
    public float range = 2f;
    public Squeezing squeezing;
    public CheckLiquid checkliquid;

    [Header("Output (Read Only)")]
    [SerializeField] private bool watered;
    [SerializeField] private bool fertilized;
    [SerializeField] private bool herbicided;
    [SerializeField] private bool painted;

    [Header("Coroutine")]
    public float pollInterval = 0.05f;

    private Coroutine signalRoutine;

    private void Start()
    {

        if (Sponge == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) Sponge = p;
        }

        if (LiquidController == null)
        {
            var l = GameObject.FindGameObjectWithTag("GameController");
            if (l != null) LiquidController = l ;
        }

        if (LiquidController != null)
        {
            if (squeezing == null) squeezing = LiquidController.GetComponentInChildren<Squeezing>(true);
            if (checkliquid == null) checkliquid = LiquidController.GetComponentInChildren<CheckLiquid>(true);
        }

      //  Debug.Log($"[LiquidReceiver] Player={Player?.name} squeezing={squeezing?.name} checkliquid={checkliquid?.name}");

        signalRoutine = StartCoroutine(GetSignalsLoop());
    }

    private void OnDisable()
    {
        if (signalRoutine != null)
        {
            StopCoroutine(signalRoutine);
            signalRoutine = null;
        }
    }

    private IEnumerator GetSignalsLoop()
    {
        var wait = new WaitForSeconds(pollInterval);

        while (true)
        {
            GetSignalsOnce();
            yield return wait;
        }
    }

    public Color color;

    private void GetSignalsOnce()
    {

       

        if (LiquidController == null || squeezing == null || checkliquid == null)
        {
            SetOutputs(false, false, false, false);
            return;
        }

        float distance = Vector3.Distance(Sponge.transform.position, transform.position);
        if (distance >= range) return;
        if (distance < range && IsSqueezing())
        {
            // read color status
            watered = checkliquid.waterReady;
            fertilized = checkliquid.fertReady;
            herbicided = checkliquid.herbReady;
            painted = checkliquid.paintReady;
            color = checkliquid.uiImage != null ? checkliquid.uiImage.color : checkliquid.paint;
        }
        else
        {
            // only clear outputs when THIS receiver object is a "pound"
            if (CompareTag("pound"))
            {
                SetOutputs(false, false, false, false);
            }
            // otherwise: keep last outputs (do nothing)
        }

        // 颜色：用 UI 当前色最稳（避免 paint 字段没赋值）
        if (checkliquid.uiImage != null) color = checkliquid.uiImage.color;
    }

    // 这里封装一下，避免你 squeezing 的字段名不同
    private bool IsSqueezing()
    {
        // 你把这里改成你 Squeezing 里真正的 bool/方法
        // 例：return squeezing.isSqueezing;
        return squeezing != null && squeezing.isSqueeze;
    }

    private void SetOutputs(bool w, bool f, bool h, bool p)
    {
        watered = w;
        fertilized = f;
        herbicided = h;
        painted = p;
    }

    // 如果你需要外部读取输出，用只读 getter
    public bool Watered => watered;
    public bool Fertilized => fertilized;
    public bool Herbicided => herbicided;
    public bool Painted => painted;
}