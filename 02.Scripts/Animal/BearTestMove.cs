using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTestMove : AnimalMove
{
    public Animator anim;

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        MoveAnimation();
    }
    private void MoveAnimation() {
        if (isMove)
        {
            anim.SetBool("WalkForward", true);
        }
        else {
            anim.SetBool("WalkForward", false);
        }
    
    }
}
