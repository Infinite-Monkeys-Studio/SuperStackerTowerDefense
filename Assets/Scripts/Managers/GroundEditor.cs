#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Assertions;

[ExecuteInEditMode]
[RequireComponent(typeof(BlockGrid))]
public class GroundEditor : MonoBehaviour
{
    public float gridSpacing;
    public float SnapTime = 1f;
    public int maxX;
    public int maxY;

    float[] lastModified;
    BlockGrid grid;
    Actor[] childActors;

    private void OnValidate()
    {
        if (!Application.isPlaying) init();
    }    

    private void init()
    {
        if (!Application.isPlaying)
        {
            grid = GetComponent<BlockGrid>();
            Assert.IsNotNull(grid);

            childActors = grid.GetComponentsInChildren<Actor>();
            Assert.IsNotNull(childActors);

            lastModified = new float[childActors.Length];
            for (int i = 0; i < lastModified.Length; i++)
            {
                lastModified[i] = Time.time;
            }
        }
    }

    private void Update()
    {
        if(!Application.isPlaying)
        {
            for (int i = 0; i < childActors.Length; i++)
            {
                Actor actor = childActors[i];

                if (actor.transform.hasChanged) //HAs the user moved the transform since last frame?
                {
                    GetGridLocation(actor.transform.position, out int x, out int y);
                    actor.xLoc = x;
                    actor.yLoc = y;
                    actor.transform.hasChanged = false;
                    lastModified[i] = Time.time;
                }

                if (Time.time - lastModified[i] > SnapTime)
                {
                    actor.transform.SetPositionAndRotation(GetWorldLocation(actor.xLoc, actor.yLoc), transform.rotation);
                }

                Assert.IsTrue(actor.xLoc >= 0 && actor.xLoc <= maxX, "Actor.xLoc outside of BlockGrid!");
                Assert.IsTrue(actor.yLoc >= 0 && actor.yLoc <= maxY, "Actor.yLoc outside of BlockGrid!");
                UnityEditor.EditorUtility.SetDirty(actor);
            }
        }
    }

    public Vector3 GetWorldLocation(int x, int y) //Provides conversion from grid location to world transform
    {
        float fx = transform.position.x + (gridSpacing * x);
        float fy = transform.position.y + (gridSpacing * y);
        Vector3 loc = new Vector3(fx, fy, 0f);
        return loc;
    }

    public void GetGridLocation(Vector2 worldPos, out int x, out int y) //returns the x and y of the world pos sent.
    {
        worldPos -= (Vector2)transform.position;
        worldPos.x += gridSpacing / 2;

        x = Mathf.FloorToInt(worldPos.x / gridSpacing);
        y = Mathf.RoundToInt(worldPos.y / gridSpacing);
    }
}
#endif