using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowMove : AnimalMove
{
    public Animator anim;

    public override void Awake()
    {
        base.Awake();
        base.SetMoveVector(2.5f);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        MoveAnimation();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Fear") || anim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            isMove = false;
        }
    }

    private void MoveAnimation()
    {
        if (isMove)
        {
            anim.SetBool("isWalk", true);
        }
        else
        {
            anim.SetBool("isWalk", false);
        }
        
    }
}
