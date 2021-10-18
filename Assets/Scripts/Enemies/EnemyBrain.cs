using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Actor))]
public abstract class EnemyBrain : MonoBehaviour
{
    public int damage;
    public float attackRate;
    public float movementSpeed;
    public GameObject DamageEffect;
    protected float lastAttackTime;
    public bool IsFlying;

    protected Actor actor;
    protected BlockGrid grid;

    protected AudioSource audioSrc;
    protected Animator animator;

    protected bool falling;

    public void Start()
    {
        grid = GetComponentInParent<BlockGrid>();
        Assert.IsNotNull(grid);

        actor = GetComponent<Actor>();
        Assert.IsNotNull(actor);

        audioSrc = GetComponent<AudioSource>();
        Assert.IsNotNull(audioSrc);

        animator = GetComponentInChildren<Animator>();
        Assert.IsNotNull(animator);

        lastAttackTime = Time.fixedTime;

        actor.init();

        grid.Insert(actor);

        actor.JumpToCurrent();
    }

    public void FixedUpdate()
    {
        //Check if next place needs to be gotten
        if (actor.progressToNextLocation >= 1f)
        {
            if (!IsFlying)
            {
                // Check for falling
                int down = actor.yLoc - 1;
                if (grid.IsYInGrid(down)) // check if on ground
                {
                    //Get the block below.
                    Actor blockBelow = grid.GetAt(actor.xLoc, down).Find(a => a.types.Contains(ActorType.Block));

                    if (blockBelow == null)
                    {
                        //Fall
                        actor.MoveTo(actor.xLoc, down, grid.FallingSpeed);
                        falling = true;
                    } else
                    {
                        falling = false;
                    }
                }
            }
            

            if(!falling)
            {
                getNextGridLocation();
            }
            
        }

    }

    //Called by Health when the enmey takes damage.  Used to spawn the Damage Effect.
    protected void takeDamage()
    {
        float xOffset = Random.Range(-0.09f, 0.09f);
        float yOffset = Random.Range(-0.1f, 0.1f);

        GameObject effect = Instantiate(DamageEffect, animator.transform.position, animator.transform.rotation, animator.transform); //Spawn the effect on the enemy
        effect.transform.Translate(new Vector3(xOffset, 0.15f + yOffset)); // move to a point in a random circle around the enemy.

        audioSrc.PlayDelayed(0.01f);
    }

    protected abstract void getNextGridLocation(); //Must be implemented by the Class brain

    protected void Death()
    {
        grid.Remove(actor);
        GameObject corpse = Instantiate(GetComponent<Health>().CorpsePrefab);
        corpse.transform.SetPositionAndRotation(transform.position, transform.rotation);
        corpse.GetComponentInChildren<RootMotion>().transform.SetPositionAndRotation(actor.motion.transform.position, actor.motion.transform.rotation);
        Destroy(gameObject);
    }
}
