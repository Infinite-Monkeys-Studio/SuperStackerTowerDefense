using UnityEngine;
using UnityEngine.Assertions;

public class PlayerWorldMouse : MonoBehaviour
{
    public BlockGrid grid;
    public Actor SelectedBlock { get; private set; }
    public GameObject SelectionCursorPrefab;

    private Transform instaCursor;
    private bool FirstFrame = true;
    public Camera sceneCamera;

    void Start()
    {
        Assert.IsNotNull(grid);
        Assert.IsNotNull(SelectionCursorPrefab);
    }

    void Update()
    {
        if(FirstFrame)
        {
            instaCursor = Instantiate(SelectionCursorPrefab, grid.transform).transform;
            instaCursor.gameObject.SetActive(false);
            FirstFrame = false;
        }

        //Left click selects a block
        if(Input.GetButtonDown("Mouse0"))
        {
            Vector3 pos = sceneCamera.ScreenToWorldPoint(Input.mousePosition);

            int xpos, ypos;
            bool inside = grid.GetGridLocation(pos, out xpos, out ypos);

            if (inside)
            {
                SelectBlockAt(xpos, ypos);
            }
        }

        //Right click activates a function unique to each block type
        if (Input.GetButtonDown("Mouse1"))
        {
            Vector3 pos = sceneCamera.ScreenToWorldPoint(Input.mousePosition);

            int xpos, ypos;
            bool inside = grid.GetGridLocation(pos, out xpos, out ypos);

            if (inside)
            {
                ActivateBlockAt(xpos, ypos);
            }
        }

        if (SelectedBlock != null) //There is no cursor. but there is a block selected
        {
            instaCursor.SetPositionAndRotation(grid.GetWorldLocation(SelectedBlock.xLoc, SelectedBlock.yLoc), instaCursor.rotation);
            instaCursor.gameObject.SetActive(true);
        } else
        {
            instaCursor.gameObject.SetActive(false);
        }
    }

    bool SelectBlockAt(int x, int y) // Find and select the block at the grid location x,y.  return true if found.
    {
        Actor block = grid.GetAt(x, y).Find(a => a.types.Contains(ActorType.Defender));

        HideAttackArea();

        if (block != null)
        {
            SelectedBlock = block;
            ShowAttackArea();
            return true;
        }

        return false;
    }

    void ActivateBlockAt(int x, int y)
    {
        Actor block = grid.GetAt(x, y).Find(a => a.types.Contains(ActorType.Defender));
        if (block != null){
            BlockBrain activeBlock = block.GetComponent<BlockBrain>();
            activeBlock.TriggeredAbility();
        }
    }

    void ShowAttackArea()
    {
        BaseTurret turret = SelectedBlock.GetComponent<BaseTurret>();
        if (turret != null)
        {
            turret.ShowAttackArea();
        }
    }

    void HideAttackArea()
    {
        if(SelectedBlock != null)
        {
            BaseTurret turret = SelectedBlock.GetComponent<BaseTurret>();
            if (turret != null)
            {
                turret.HideAttackArea();
            }

            SelectedBlock = null;
        }   
    }
}
