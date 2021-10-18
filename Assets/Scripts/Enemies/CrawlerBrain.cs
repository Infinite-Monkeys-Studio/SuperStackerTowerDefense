using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CrawlerBrain : EnemyBrain
{
    public bool curled = false;

    private new void Start()
    {
        base.Start();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private new void Death()
    {
        grid.Remove(actor);                         //Remove the crawler from the grid
        GameObject corpse = Instantiate(GetComponent<Health>().CorpsePrefab);            //Spawn the corpse
        corpse.transform.SetPositionAndRotation(transform.position, transform.rotation); //Move the corpse to the right place
        corpse.GetComponentInChildren<Animator>().SetBool("Curled", curled);                       //Set the corpse to match the living curled state
        Destroy(gameObject);                                                             //Delete the living object
    }

    //This is the brain of the enemy.
    // This is called everytime a descision has to be made.  This determines all the special behaviour of this enemy.
    protected override void getNextGridLocation()
    {
        if (curled) return; //Once curled: always curled.

        int forwardX = actor.xLoc - 1;
        if (!grid.IsXInGrid(forwardX)) return; //Check that X isn't outside the grid.

        Actor block = grid.GetAt(forwardX, actor.yLoc).Find(a => a.types.Contains(ActorType.Block)); //Get the block ahead of the crawler

        // can we go forward?
        if (block != null && !block.types.Contains(ActorType.Passable))
        {
            int upY = actor.yLoc + 1;
            if (upY > grid.height) return; //Check that y isn't outside the grid.

            //No? ok check if we should make a step
            Actor upBlock = grid.GetAt(forwardX, upY).Find(a => a.types.Contains(ActorType.Block)); //What's 'upBlock'?  Not much what's up with you?

            //Find if another crawler is already curled here
            bool alreadyCurled = grid.GetAt(actor).Exists(a => a.types.Contains(ActorType.Block));

            if (upBlock != null && !alreadyCurled)
            {
                //Curl up
                animator.SetTrigger("Curl");
                curled = true;
                actor.progressToNextLocation = 1f;
                actor.types.Add(ActorType.Block);
                actor.types.Add(ActorType.Passable);
            }
            else
            {
                //don't curl; ATTACK!
                actor.progressToNextLocation = 1;  // When progress is set to one.  getNextGridLocation() will be called each update.  This way the enemy can see when the block is destroyed.
                if (Time.fixedTime - lastAttackTime > 1 / attackRate)
                {
                    block.GetComponent<Health>().damage(damage);
                    lastAttackTime = Time.fixedTime;
                }

                animator.SetBool("Walking", false);
                animator.SetBool("Attacking", true);
            }
        }
        else
        {
            //yes? great! keep moving
            actor.MoveTo(actor.xLoc - 1, actor.yLoc, movementSpeed);
            animator.SetBool("Walking", true);
            animator.SetBool("Attacking", false);
        }
    }
}
