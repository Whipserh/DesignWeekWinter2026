

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
    public GameObject Player;
    public float range = 2f;
    public Squeezing squeezing;
    public CheckLiquid checkliquid;

    [Header("Output (Read Only)")]
    [SerializeField] private bool watered;
    [SerializeField] private bool fertilized;
    [SerializeField] private bool herbicided;

    [Header("Coroutine")]
    public float pollInterval = 0.05f;

    private Coroutine signalRoutine;

    private void Start()
    {
        // Auto-find if not assigned (optional)
        if (Player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) Player = p;
        }

        if (squeezing == null)
        {
            // 如果 squeezing 在 Player 上
            if (Player != null) squeezing = Player.GetComponent<Squeezing>();
        }

        if (checkliquid == null)
        {
            // 如果 checkliquid 在 Player 上
            if (Player != null) checkliquid = Player.GetComponent<CheckLiquid>();
        }

        // 启动协程
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

    private void GetSignalsOnce()
    {
        // 基础容错
        if (Player == null || squeezing == null || checkliquid == null)
        {
            SetOutputs(false, false, false);
            return;
        }

        float distance = Vector3.Distance(Player.transform.position, transform.position);

        if (distance < range && IsSqueezing())
        {
            // 读取 checkliquid 的状态
            // 这里按你给的字段名写：waterReady / fertReady / herbReady
            watered = checkliquid.waterReady;
            fertilized = checkliquid.fertReady;
            herbicided = checkliquid.herbReady;
        }
        else
        {
            // 不在范围 / 没有 squeeze => 输出 false（如果你想保持上一次，就删掉这行）
            SetOutputs(false, false, false);
        }
    }

    // 这里封装一下，避免你 squeezing 的字段名不同
    private bool IsSqueezing()
    {
        // 你把这里改成你 Squeezing 里真正的 bool/方法
        // 例：return squeezing.isSqueezing;
        return squeezing != null && squeezing.isSqueeze;
    }

    private void SetOutputs(bool w, bool f, bool h)
    {
        watered = w;
        fertilized = f;
        herbicided = h;
    }

    // 如果你需要外部读取输出，用只读 getter
    public bool Watered => watered;
    public bool Fertilized => fertilized;
    public bool Herbicided => herbicided;
}