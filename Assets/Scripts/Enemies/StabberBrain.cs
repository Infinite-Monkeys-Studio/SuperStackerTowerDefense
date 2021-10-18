using UnityEngine;

[RequireComponent(typeof(Animator))]
public class StabberBrain : EnemyBrain
{

    private new void Start()
    {
        base.Start();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // called everytime a descision needs to be made.  This is the brain.
    protected override void getNextGridLocation()
    {
        Actor forwardBlock = grid.GetAt(actor.xLoc - 1, actor.yLoc).Find(a => a.types.Contains(ActorType.Block));
        
        // can we go forward?
        if (forwardBlock != null && !forwardBlock.types.Contains(ActorType.Passable))
        {
            //no? ATTACK!
            Actor highblock = grid.GetAt(actor.xLoc - 1, actor.yLoc + 1).Find(a => a.types.Contains(ActorType.Block));
            if (highblock == null || highblock.types.Contains(ActorType.Passable)) highblock = null; //Don't attack passable blocks.

            actor.progressToNextLocation = 1;  // right where the stabber want's to be.  Just stabbing away!
            if (Time.fixedTime - lastAttackTime > 1 / attackRate)
            {

                // Attack high?
                if(highblock != null)
                {
                    highblock.GetComponent<Health>().damage(damage);
                    animator.SetBool("AttackingHigh", true);
                    animator.SetBool("AttackingLow", false);
                } else
                {
                    //Attack low
                    forwardBlock.GetComponent<Health>().damage(damage);
                    animator.SetBool("AttackingLow", true);
                    animator.SetBool("AttackingHigh", false);
                }

                lastAttackTime = Time.fixedTime;
            }
            animator.SetBool("Walking", false);
        }
        else
        {
            //yes? great! moving on!
            actor.MoveTo(actor.xLoc - 1, actor.yLoc, movementSpeed);
            animator.SetBool("Walking", true);
            animator.SetBool("AttackingHigh", false);
            animator.SetBool("AttackingLow", false);
        }
    }
}
