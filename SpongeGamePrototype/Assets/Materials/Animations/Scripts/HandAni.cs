using UnityEngine;

public class HandAni : MonoBehaviour
{
    int i = 0;
    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) //when the a key is pressed
        {
            if (i == 0) {
                animator.SetBool("isAbsorbing", true); //abosrb animation plays
                animator.SetBool("isSqueezing", false);
                i++;
            }
            else
            {
                animator.SetBool("isSqueezing", true); //squeeazing animation plays
                animator.SetBool("isAbsorbing", false);
                i = 0;
            }
        }
        

    }
}
