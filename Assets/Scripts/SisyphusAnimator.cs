using Assets.Scripts.BoulderStuff;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SisyphusAnimator : MonoBehaviour
{
    Animator animator;
    MovementStateController movementStateController;
    public float speedPercent = 0f;
    public bool pushing = false;
    public bool jumping = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        movementStateController = GetComponent<MovementStateController>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("speedPercent", speedPercent);

        //grounded checker
        animator.SetBool("isGrounded", movementStateController.isGrounded); 

    }

    public void SetPushing(bool pushing)
    {
        if (this.pushing != pushing)
        {
            animator.SetBool("pushing", pushing);
            this.pushing = pushing;
        }
    }

    public void SetJumping(bool jumping)
    {
        if(this.jumping != jumping)
        {
            animator.SetBool("isJumping", jumping); 
            this.jumping = jumping;
        }
    }

}
