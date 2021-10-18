using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class BlockPlacer : MonoBehaviour
{
#pragma warning disable 0649
    public Camera SceneCamera;
    public float KeyRepeatDelay;
    public Color placeColor;
    public Color blockedColor;
    public GameObject AttackAreaPrefab;
    public List<GameObject> AttackAreaObjects;
    [SerializeField] private PlayerEnergy playerEnergy;

    private GameObject SelectedBlock;
    private GameObject SelectedBlockUI;
    [SerializeField] private BlockGrid grid;
    [SerializeField] private Transform blockGhost;
    public int xLoc;
    public int yLoc;

    [System.Serializable]
    public class BlockDict : SerializableDictionary<string, GameObject> { }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(BlockDict))]
    public class BlockDictionaryDrawer : DictionaryDrawer<string, GameObject> { } //This is a custom editor that allows editing the dictionary
#endif

    [SerializeField] public BlockDict NamedBlocks;

    private bool placeBlock = false;
    public bool ignoreCoolDown = false;
    
    private void Start()
    {
        Assert.IsNotNull(SceneCamera);
        Assert.IsNotNull(grid);
        Assert.IsNotNull(playerEnergy);
        Assert.IsNotNull(blockGhost);
        Assert.IsNotNull(NamedBlocks);
    }

    
    private void Update()
    {
        UpdateLocation(Input.mousePosition);

        transform.SetPositionAndRotation(grid.GetWorldLocation(xLoc, yLoc), transform.rotation); //Set the position

        updateBlockGhost();

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetButtonDown("Mouse0"))
            {
                if (SelectedBlock != null)
                {
                    placeBlock = true;
                }
            }

            if (Input.GetButtonDown("Mouse1"))
            {
                if (SelectedBlock != null)
                {
                    SelectedBlock = null;
                }
            }
        }
    }    

    private void FixedUpdate()
    {
        if(placeBlock) //Spawn a new block
        {
            var cooldown = SelectedBlockUI.GetComponent<CoolDownEffect>();

            bool cooling = ignoreCoolDown ? false : cooldown.CoolingDown;

            BlockBrain blockBase = SelectedBlock.GetComponent<BlockBrain>(); // get a reference to the brain of the block
            bool canPlace = false;                                           // This prevents placing until there has been another check
            int lowY = HighestBuildingSpaceInColumn(xLoc, ref canPlace);    // Find the grid space the block is going to
            if (canPlace && !cooling && playerEnergy.spendEnergy(blockBase.EnergyCost)) // player energy will return true if there is enough energy
            {
                // make the new block from the prefab. set it's position, rotation, and parent
                GameObject newBlock = Instantiate(SelectedBlock, transform.position, transform.rotation, grid.transform);
                newBlock.GetComponent<BlockBrain>().initBlock(); // called so the block gets it's references to it's actor and grid
                newBlock.GetComponent<BlockBrain>().actor.init();   // called so the actor get's isn't references
                newBlock.GetComponent<BlockBrain>().actor.FallFromPosition(transform.position, xLoc, lowY); //setup the actors "falling into place" animation
                cooldown.StartCoolDown();

                if(!Input.GetButton("ModKey0"))
                {
                    SelectedBlock = null;
                }
            }
            placeBlock = false; //the block has either been placed or not.  Don't try again.
        }
    }

    private void updateBlockGhost() //Updates the block ghost to show where and if the block can be placed.
    {
        if(SelectedBlock == null)
        {
            blockGhost.gameObject.SetActive(false);
            HideAttackArea();
        } else
        {
            blockGhost.gameObject.SetActive(true);
            Sprite newSprite = SelectedBlock.GetComponentInChildren<SpriteRenderer>().sprite; // get the sprite of the selected block
            Sprite oldSprite = blockGhost.GetComponent<SpriteRenderer>().sprite; // and the sprite the ghost is using
            if (newSprite != null && newSprite != oldSprite) // are the sprites the same?
            {
                blockGhost.GetComponent<SpriteRenderer>().sprite = newSprite; // nope, guess they need to update.
                HideAttackArea();
            }

            bool canPlace = false; //created to be passed by reference so we know if the block can be placed.
            int lowY = HighestBuildingSpaceInColumn(xLoc, ref canPlace); //find the valid building site for this column
            blockGhost.SetPositionAndRotation(grid.GetWorldLocation(xLoc, lowY), blockGhost.rotation); //Set the position and rotation of the ghost
            blockGhost.GetComponent<SpriteRenderer>().color = canPlace ? placeColor : blockedColor; // chage the color to show if the block can be placed.

            BaseTurret turret = SelectedBlock.GetComponent<BaseTurret>();
            if (turret != null)
            {
                if(AttackAreaObjects.Count == 0)
                {
                    ShowAttackArea(turret, xLoc, lowY);
                }
            } else
            {
                HideAttackArea();
            }
        }
    }

    private void HideAttackArea()
    {
        foreach (var item in AttackAreaObjects)
        {
            item.SetActive(false);
            Destroy(item, 0.2f);
        }

        AttackAreaObjects.Clear();
    }

    private void ShowAttackArea(BaseTurret turret, int x, int y)
    {
        foreach (var pos in turret.GetAttackArea())
        {
            GameObject obj = Instantiate(
                    AttackAreaPrefab,
                    grid.GetWorldLocation(pos.Item1 + x, pos.Item2 + y),
                    transform.rotation, blockGhost.transform
                    );

            AttackAreaObjects.Add(obj);
        }
    }

    private void UpdateLocation(Vector3 mousePos)
    {
        Vector2 worldPos = SceneCamera.ScreenToWorldPoint(mousePos);
        
        grid.GetGridLocation(worldPos, out int newX, out int y);
        if(newX >= 0 && newX < grid.width)
        {
            xLoc = newX;
        }
    }

    private void SetSelectedBlock(string name) // called by the UI to change what block the player selects.
    {
        GameObject newBlock;
        bool gottem = NamedBlocks.TryGetValue(name, out newBlock);
        if(gottem) //If the Name that was sent is valid set the new selected block
        {
            SelectedBlock = newBlock;
        } else
        {
            Debug.LogError(string.Format("Cannont Find a block named: \"{0}\"", name), this);
        }
    }

    public void SetSelectedBlock(string name, GameObject UIObject)
    {
        SelectedBlockUI = UIObject;
        SetSelectedBlock(name);
    }

    //Returns the Y value of the highest empty block with a block or ground below it.
    //CanPlace will be set depeding if the block below is valid to build on or if there are enemies in the building space.
    public int HighestBuildingSpaceInColumn(int columnX, ref bool CanPlace)
    {
        int highestY = grid.height - 1;
        CanPlace = false;

        for(int y = highestY; y >= 0; y--)
        {
            int below = y - 1;
            if(grid.IsYInGrid(below))
            {
                //check for solid below
                if (grid.GetAt(columnX, below).Exists(a => a.types.Contains(ActorType.Block))) //this is the ground
                {
                    highestY = y;//This Y is a valid spot
                    break; //End the for loop
                }
            } else
            {
                //Handle below outside of grid
                break;
            }
            
        }

        //check for a buildable block below
        if(grid.GetAt(columnX, highestY > 0 ? highestY - 1 : highestY).Exists(a => a.types.Contains(ActorType.Buildable)))
        {
            CanPlace = true;
        }
        
        //check for alien in square
        if (grid.GetAt(columnX, highestY).Exists(a => a.types.Contains(ActorType.Enemy)))
        {
            CanPlace = false;
        }

        //edge case.  Blocks are stacked to build height
        if (grid.GetAt(columnX, highestY).Exists(a => a.types.Contains(ActorType.Block)))
        {
            //If the selected block is a wrecking ball then ignore that it will be placed inside another block
            //this allows deleting blocks at the build limit
            WreckingBall wreckingBall = SelectedBlock.GetComponent<WreckingBall>();
            if(wreckingBall == null)
            {
                CanPlace = false;
            }
        }

        return highestY;
    }

    public void IgnoreCoolDown(bool newState)
    {
        ignoreCoolDown = newState;
    }
}
