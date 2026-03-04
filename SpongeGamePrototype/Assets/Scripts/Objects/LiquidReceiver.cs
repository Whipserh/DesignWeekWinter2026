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
            // Find player action squeeze From Squeezing
            if (Player != null) squeezing = Player.GetComponent<Squeezing>();
        }

        if (checkliquid == null)
        {
            // Find liquid type From CheckLiquid
            if (Player != null) checkliquid = Player.GetComponent<CheckLiquid>();
        }

        // Call Coroutine
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
        // Basic fault tolerance
        if (Player == null || squeezing == null || checkliquid == null)
        {
            SetOutputs(false, false, false);
            return;
        }

        float distance = Vector3.Distance(Player.transform.position, transform.position);

        if (distance < range && IsSqueezing())
        {
            // Read liquid status
            // waterReady / fertReady / herbReady
            watered = checkliquid.waterReady;
            fertilized = checkliquid.fertReady;
            herbicided = checkliquid.herbReady;
        }
        else
        {
            // if not in range / No squeeze => Output false
            SetOutputs(false, false, false); // delate this if you what to keep the memory
        }
    }

    // make sure the name is correct
    private bool IsSqueezing()
    {
        //
        return squeezing != null && squeezing.isSqueeze;
    }

    private void SetOutputs(bool w, bool f, bool h)
    {
        watered = w;
        fertilized = f;
        herbicided = h;
    }

    // If you need to read the output externally, use a read-only getter.
    public bool Watered => watered;
    public bool Fertilized => fertilized;
    public bool Herbicided => herbicided;
}