using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Actor))]
public class BlockBrain : MonoBehaviour
{
    public string DisplayName;
    public int EnergyCost;
    public float PlacementCoolDown;
    public BlockGrid grid;
    public Actor actor;

    public float ShakeTime;
    public float ShakeForce;

    protected bool falling;

    private float ShakeEndTime;

    public void Start()
    {
        if(grid == null || actor == null)
        {
            initBlock();
        }
    }

    public void initBlock() // This is broken out of start so it can be called outside the class.
    {                       // There is an issue when creating a new instance the "start()" isn't called until the next frame.
                            // This way init() can get the grid and actor references for use before then.
        grid = GetComponentInParent<BlockGrid>();
        actor = GetComponent<Actor>();

        Assert.IsNotNull(grid);
        Assert.IsNotNull(actor);

        grid.Insert(actor);
    }

    public void Update()
    {
        if(ShakeEndTime > Time.time)
        {
            //Shake it baby!
            Vector3 shake = new Vector3(1, 1, 1);
            shake = shake * ShakeForce * (Time.time / ShakeEndTime);
            transform.localScale = shake;
        } else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    protected void FixedUpdate()
    {
        int down = actor.yLoc - 1;
        if (grid.IsYInGrid(down)) { //check this block isn't on the bottom
            if(grid.GetAt(actor.xLoc, down).Find(a => a.types.Contains(ActorType.Block)) == null) // check if a block is below this one
            {
                //Oh No! falling!
                actor.MoveTo(actor.xLoc, down, grid.FallingSpeed);
                falling = true;

                //Find all the enemies that the block fell on.  Move them back one block
                // This will keep enemies from sneaking(glitching) under falling blocks.
                List<Actor> list = grid.GetAt(actor.xLoc, actor.yLoc).FindAll(a => a.types.Contains(ActorType.Enemy));
                foreach (var enemy in list)
                {
                    enemy.MoveTo(actor.xLoc + 1, actor.yLoc, grid.FallingSpeed);
                }
            } else
            {
                falling = false;
            }
        }
    }

    private void takeDamage()
    {
        if(ShakeEndTime < Time.time) // already shaking.  Don't do it more.
        {
            ShakeEndTime = Time.time + ShakeTime;
        }
    }

    protected void Death()
    {
        grid.Remove(actor);
        //TODO spawn wreck
        Destroy(gameObject, 0.1f);
    }

    public virtual void TriggeredAbility() { }

}
