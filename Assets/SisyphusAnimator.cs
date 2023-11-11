using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SisyphusAnimator : MonoBehaviour
{
    Animator animator;
    public float speedPercent = 0f;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("speedPercent", speedPercent);
    }
}
