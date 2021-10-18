using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class BaseTurret : BlockBrain
{
    public GameObject AttackAreaPrefab;
    public TargetPriority targetPriority = TargetPriority.Nearest;
    [Tooltip("Damage per shot")]
    [SerializeField] protected int damage;  // damage per shot
    [Tooltip("Rate in shots per sec")]
    [SerializeField] protected float rateOfFire; //Shots per sec
    [SerializeField] public int FieldOfFireWidth;
    [SerializeField] public int FieldOfFireHeight;
    [Tooltip("Offset Specifys the bottom left corner of the FOF")]
    [SerializeField] public int FieldOfFireOffsetX;
    [Tooltip("Offset Specifys the bottom left corner of the FOF")]
    [SerializeField] public int FieldOfFireOffsetY;

    protected Actor currentTarget;

    protected float lastShotTime;
    private List<GameObject> AttackAreaObjects;

    protected new void Start()
    {
        base.Start();

        Assert.IsNotNull(AttackAreaPrefab);

        lastShotTime = Time.fixedTime;
        currentTarget = null;
        AttackAreaObjects = new List<GameObject>();
    }

    protected new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected List<Actor> FindNewTargets(int numberOfShots)
    {
        List<Actor> possibleTargets = FindPossibleTargets(); //Get a list of all targets in the FOF

        //TODO make sure sort works.
        possibleTargets.Sort((a, b) => a.xLoc - b.xLoc); //Sort be nearest

        if(numberOfShots > possibleTargets.Count) //Return the whole list if that are fewer than the number of shots.
        {
            return possibleTargets;
        }

        return possibleTargets.GetRange(0, numberOfShots); // return the first X number.
    }

    public void ShowAttackArea()
    {
        if(AttackAreaPrefab != null)
        {
            HideAttackArea();
        }

        foreach (var pos in GetAttackArea())
        {
            GameObject obj = Instantiate(
                    AttackAreaPrefab,
                    grid.GetWorldLocation(pos.Item1 + actor.xLoc, pos.Item2 + actor.yLoc),
                    transform.rotation, grid.transform
                    );

            AttackAreaObjects.Add(obj);
        }
    }

    public List<System.Tuple<int, int>> GetAttackArea()
    {
        List<System.Tuple<int, int>> area = new List<System.Tuple<int, int>>();

        for (int x = 0; x < FieldOfFireWidth; x++)
        {
            for (int y = 0; y < FieldOfFireHeight; y++)
            {
                int offx = x + FieldOfFireOffsetX;
                int offy = y + FieldOfFireOffsetY;
                area.Add(new System.Tuple<int, int>(offx, offy));
            }
        }
        
        return area;
    }

    public void HideAttackArea()
    {
        foreach (var item in AttackAreaObjects)
        {
            item.SetActive(false);
            Destroy(item, 0.2f);
        }

        AttackAreaObjects.Clear();
    }

    private Actor findNearest(List<Actor> possibleTargets)
    {
        if (possibleTargets.Count > 0) //Is the list empty
        {
            Actor nearestActor = null;
            foreach (Actor actor in possibleTargets)  // loop through targets and find the one with the least x
            {
                if (nearestActor == null)
                {
                    nearestActor = actor;
                }

                if (actor.xLoc < nearestActor.xLoc)
                {
                    nearestActor = actor;
                }
            }

            return nearestActor;
        }

        return null;
    }

    protected Actor FindNewTarget() 
    {
        List<Actor> possibleTargets = FindPossibleTargets();

        //TODO implement Target priority
        switch (targetPriority)
        {
            case TargetPriority.Nearest:
                return findNearest(possibleTargets);
            default:
                return findNearest(possibleTargets);
        }
    }

    protected List<Actor> FindPossibleTargets()
    {
        List<Actor> possibleTargets = new List<Actor>();
        for (int x = 0; x < FieldOfFireWidth; x++)          //loop through the squares in the FOF
        {
            int offX = x + FieldOfFireOffsetX + actor.xLoc; // offX is the absolute X to be checked.

            if (offX > grid.width - 1 || offX < 0) continue; //Check x is inside the block grid

            for (int y = 0; y < FieldOfFireHeight; y++)
            {
                int offY = y + FieldOfFireOffsetY + actor.yLoc; // offY is the absolute Y to be checked.

                if (offY > grid.height - 1 || offY < 0) continue; //Check y is inside the block grid

                List<Actor> enemies = grid.GetAt(offX, offY).FindAll(a => a.types.Contains(ActorType.Enemy));  // find enemies and put them in possible targets

                if (enemies.Count > 0) //Don't add an empty list
                {
                    possibleTargets.AddRange(enemies);
                }
            }
        }

        return possibleTargets;
    }

    protected new void Death()
    {
        HideAttackArea();
        base.Death();
    }
}


public enum TargetPriority { Nearest, Strongest, Healthiest, Furthest, Weakest, Unhealthyest }