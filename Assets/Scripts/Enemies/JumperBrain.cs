using UnityEngine;

[RequireComponent(typeof(Animator))]
public class JumperBrain : EnemyBrain
{
    public float jumpSpeed;

    private new void Start()
    {
        base.Start();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        
    }

    private void Update()
    {
        animator.SetBool("Falling", falling);
    }

    protected override void getNextGridLocation()
    {
        int forward = actor.xLoc - 1;
        if (forward < 0) return;

        //Find the blocks we need
        Actor front = grid.GetAt(actor.xLoc - 1, actor.yLoc).Find(a => a.types.Contains(ActorType.Block));
        Actor top = grid.GetAt(actor.xLoc - 1, actor.yLoc + 1).Find(a => a.types.Contains(ActorType.Block));

        //if there is a block ahead, and nothing or something passable on top
        if ((front != null && !front.types.Contains(ActorType.Passable)) && (top == null || top.types.Contains(ActorType.Passable)) )
        {
            //JUMP!
            actor.MoveTo(actor.xLoc - 1, actor.yLoc + 1, jumpSpeed);
            animator.applyRootMotion = false;
            animator.SetTrigger("Jump");
            return;
        }

        //if there is nothing in front or it's passable
        if (front == null || front.types.Contains(ActorType.Passable))
        {
            //Walking 500 miles
            actor.MoveTo(actor.xLoc - 1, actor.yLoc, movementSpeed);
            animator.applyRootMotion = true;
            return;
        }

        //Check for a curled crawler beside the jumper
        Actor beside = grid.GetAt(actor.xLoc, actor.yLoc).Find(a => a.types.Contains(ActorType.Block));
        Actor above = grid.GetAt(actor.xLoc, actor.yLoc + 1).Find(a => a.types.Contains(ActorType.Block));
        
        //if There is a block to jump right beside the jumper, and there is nothing or something passable above
        if (beside != null && (above == null || above.types.Contains(ActorType.Passable)) )
        {
            //might as well jump. *jump!*
            animator.SetTrigger("Jump Up");
            animator.applyRootMotion = false;
            actor.MoveTo(actor.xLoc, actor.yLoc + 1, movementSpeed);
            return;
        }

        //Have to go through it
        actor.progressToNextLocation = 1; //Jumper is right where it wants to be
        if (Time.fixedTime - lastAttackTime > 1 / attackRate)
        {
            front.GetComponent<Health>().damage(damage);
            lastAttackTime = Time.fixedTime;
            animator.applyRootMotion = true;
            animator.SetTrigger("Attack");
        }
    }
}
