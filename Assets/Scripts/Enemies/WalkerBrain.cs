using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WalkerBrain : EnemyBrain
{
    private new void Start()
    {
        base.Start();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void getNextGridLocation()
    {
        int forward = actor.xLoc - 1;
        if (!grid.IsXInGrid(forward)) return;
        List<Actor> list = grid.GetAt(forward, actor.yLoc); // find what's ahead of the walker

        // can we go forward?
        //if there is a block and it's not passable
        if (list.Exists(x => x.types.Contains(ActorType.Block) && !x.types.Contains(ActorType.Passable)))
        {
            //no? ATTACK!
            actor.progressToNextLocation = 1;  // set this to one to signal the alien is blocked until it can finish attacking.
            if (Time.fixedTime - lastAttackTime > 1 / attackRate)
            {
                Actor blockActor = list.Find(a => a.types.Contains(ActorType.Defender));
                blockActor.GetComponent<Health>().damage(damage);
                lastAttackTime = Time.fixedTime;
            }
            animator.SetBool("Walking", false);
            animator.SetBool("Attacking", true);
        }
        else
        {
            //yes? great! I would walk 500 miles!
            actor.MoveTo(actor.xLoc - 1, actor.yLoc, movementSpeed);
            animator.SetBool("Walking", true);
            animator.SetBool("Attacking", false);
        }
    }
}
