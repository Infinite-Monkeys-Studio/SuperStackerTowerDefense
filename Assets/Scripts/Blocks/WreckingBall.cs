using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WreckingBall : BlockBrain
{
    private Animator anim;

    private bool firstDrop = true;

    new void Start()
    {
        base.Start();

        anim = GetComponentInChildren<Animator>();
        Assert.IsNotNull(anim);
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
        if(actor.progressToNextLocation > .99f) //Crack once the ball gets to where it's going.
        {
            if(firstDrop)
            {
                firstDrop = false;

                //Only go lower if there isn't already a block to delete
                if(!grid.GetAt(actor).Exists(a => a.types.Contains(ActorType.Block)))
                {
                    int newY = actor.yLoc - 1;
                    if (!grid.GetAt(actor.xLoc, newY).Exists(a => a.types.Contains(ActorType.Ground)))  //Make sure this isn't the ground
                    {
                        actor.MoveTo(actor.xLoc, newY, grid.FallingSpeed);
                    }
                }
            } else
            {
                anim.SetTrigger("Crack");

                // find everything in the square the wrecking ball is landing in and delete it.
                foreach (var item in grid.GetAt(actor.xLoc, actor.yLoc).FindAll(a => a.types.Contains(ActorType.Defender)))
                {
                    grid.Remove(item);
                    item.SendMessage("Death");
                }
            }
        }
    }

    private void AnimationComplete() //The animation will call this once it's finished Crack-a-lacking.
    {
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        Death();
    }
}
