using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Actor : MonoBehaviour
{
    public List<ActorType> types;
    public int xLoc;
    public int yLoc;

    public RootMotion motion;
    public BlockGrid grid;

    public float progressToNextLocation;
    public float CurrentSpeed;
    public float CurrentDistance;
    
    //Points list specific
    public List<Vector2> CurrentPointList;
    public float CurrentSegmentDistance;
    public float CurrentSegmentProgress;
    public int nextX;
    public int nextY;
    public int pointIndex;
    public bool usingPointList;

    
    
    void Start()
    {
        init();
    }

    public void init() //Having this broken out like this means it can be called by other classes incase the actor needs to be initialized before the next frame.
    {
        if(motion == null)
        {
            motion = GetComponentInChildren<RootMotion>();
        }

        Assert.IsNotNull(motion);

        if(grid == null)
        {
            grid = GetComponentInParent<BlockGrid>();
        }
        
        Assert.IsNotNull(grid);

        if(types == null)
        {
            types = new List<ActorType>();
        }
    }

    public void FixedUpdate()
    {
        transform.SetPositionAndRotation(grid.GetWorldLocation(xLoc, yLoc), transform.rotation);

        if(usingPointList)
        {//Now doing a point list
            CurrentSegmentProgress += (Time.fixedDeltaTime * CurrentSpeed) / (CurrentSegmentDistance == 0 ? 1 : CurrentSegmentDistance);

            if (CurrentSegmentProgress >= 1) // if past this segment of the points list
            {
                pointIndex++;
                if(pointIndex < CurrentPointList.Count) // check for completion of the list
                {
                    CurrentSegmentProgress = 0;
                    CurrentSegmentDistance = motion.SinglePointLerp(CurrentPointList[pointIndex], CurrentSpeed);
                } else
                {
                    usingPointList = false;

                    // get the vector from the new location to the current offset
                    Vector2 offset = (grid.GetWorldLocation(xLoc, yLoc) + motion.transform.localPosition) - grid.GetWorldLocation(nextX, nextY);

                    updateLocation(nextX, nextY);
                    CurrentDistance = motion.StartLerp(offset, new Vector2(0, 0), CurrentSpeed);
                    progressToNextLocation = 0;
                }
            } else
            {
                motion.DoUpdate(CurrentSegmentProgress);
            }
        }
        else
        {
            progressToNextLocation += (Time.fixedDeltaTime * CurrentSpeed) / (CurrentDistance == 0 ? 1 : CurrentDistance);  //incrament progress by speed * deltaTime
            motion.DoUpdate(progressToNextLocation);
        }
    }

    private void updateLocation(int x, int y)
    {
        grid.Remove(this);
        xLoc = x;
        yLoc = y;
        grid.Insert(this);
        transform.SetPositionAndRotation(grid.GetWorldLocation(xLoc, yLoc), transform.rotation);
    }

    //move to x,y grid location.  If glide is true it will glide there, otherwise it will snap.
    public void MoveTo(int x, int y, float speed)
    {
        progressToNextLocation = 0f;
        CurrentSpeed = speed;
        Vector2 startingVector = grid.GetWorldLocation(xLoc, yLoc) - grid.GetWorldLocation(x, y);
        Vector2 endingVector = new Vector2(0, 0);

        updateLocation(x, y);

        CurrentDistance = motion.StartLerp(startingVector, endingVector, CurrentSpeed);
    }

    public void MoveAlongPointsTo(int x, int y, float speed, List<Vector2> points)
    {
        usingPointList = true;
        pointIndex = 0;
        CurrentPointList = points;

        nextX = x;
        nextY = y;

        Assert.IsTrue(points.Count >= 2);

        float dist = 0;

        for (int i = 1; i < points.Count; i++)
        {
            dist += Vector2.Distance(points[i - 1], points[i]);
        }

        CurrentDistance = dist;
        CurrentSpeed = speed;
        CurrentSegmentDistance = motion.SinglePointLerp(CurrentPointList[pointIndex], CurrentSpeed);
        CurrentSegmentProgress = 0;
    }

    //Cause the actor to get to where it is now.
    public void JumpTo(int x, int y)
    {
        progressToNextLocation = 1f;
        motion.SetToZero();
        updateLocation(x, y);
    }

    public void JumpToCurrent()
    {
        progressToNextLocation = 1f;
        motion.SetToZero();
    }

    public void FallFromPosition(Vector3 pos, int x, int y)
    {
        progressToNextLocation = 0f;
        Vector2 startingVector = pos - grid.GetWorldLocation(x, y);
        Vector2 endingVector = new Vector2(0, 0);
        updateLocation(x, y);
        CurrentSpeed = grid.FallingSpeed;
        CurrentDistance = motion.StartLerp(startingVector, endingVector, CurrentSpeed);
    }
}

[System.Serializable]
public enum ActorType
{
    Enemy,      // Enemy represents an attacker.  Use for turrets and preventing friendly fire
    Block,      // Block represents a physical object.  Can be climbed, walked on, but cannot be walked past!
    Passable,   // Passable is a block modifier.  It means that the block CAN be walked past (i.e. curled crawler, gate)
    Defender,   // Defender represents a player object.  Used for enemies to attack, and to prevent friendly fire.
    Goal,       // Goal is the reactor, or at least something aliens want to destroy.
    Ground,     // Ground defines a block that should not be attacked and cannot be placed.
    Buildable,  // Blocks can be placed on top of this block
    Mineable    // A miner can extract energy from this block
};
