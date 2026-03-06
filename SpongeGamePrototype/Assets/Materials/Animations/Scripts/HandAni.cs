using UnityEngine;

public class HandAni : MonoBehaviour
{
    public Animator animator;

    [Header("Refs")]
    public CheckLiquid checkLiquid; // use CheckLiquid.isAbsorbing
    public Squeezing squeezing;     // still used for isSqueeze (releasing)

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator == null) return;

        // Absorb animation uses CheckLiquid.cs public bool isAbsorbing
        bool absorbing = (checkLiquid != null) && checkLiquid.isAbsorbing;
        animator.SetBool("isAbsorbing", absorbing);

        // Squeeze animation (releasing water) still uses Squeezing.isSqueeze
        bool squeezingNow = (squeezing != null) && squeezing.isSqueeze;
        animator.SetBool("isSqueezing", squeezingNow);
    }
}