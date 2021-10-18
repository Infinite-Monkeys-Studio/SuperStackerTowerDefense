using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Actor))]
public class GroundBlock : MonoBehaviour
{
    public BlockGrid grid;
    public Actor actor;

    void Start()
    {
        if (grid == null || actor == null)
        {
            InitBlock();
        }

        actor.init();
        grid.Insert(actor);
        actor.JumpToCurrent();
    }

    public void InitBlock() // This is broken out of start so it can be called outside the class.
    {                       // There is an issue when creating a new instance the "start()" isn't called until the next frame.
                            // This way init() can get the grid and actor references for use before then.
        grid = GetComponentInParent<BlockGrid>();
        actor = GetComponent<Actor>();

        Assert.IsNotNull(grid);
        Assert.IsNotNull(actor);
    }
}
