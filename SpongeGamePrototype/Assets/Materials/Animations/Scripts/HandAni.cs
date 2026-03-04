using UnityEngine;

public class HandAni : MonoBehaviour
{

    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) //when the a key is pressed, the hand will squeeze
        {
            animator.SetBool("isSqueezing", true);
            animator.SetBool("isAbsorbing", false);
        }

        if (Input.GetKeyDown(KeyCode.A)) //when the s key is pressed, the hand will release
        {
            animator.SetBool("isAbsorbing", true);
            animator.SetBool("isSqueezing", false);
        }
        

    }
}
