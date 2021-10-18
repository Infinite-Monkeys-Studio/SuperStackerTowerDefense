using System.Collections.Generic;
using UnityEngine;

public enum GridSize
{
    Width = 20, Height = 8
}

public class BlockGrid : MonoBehaviour
{

#pragma warning disable 0649
    public float gridSpacing;
    public float FallingSpeed = 4;
    public LevelSceneController sc;
    

    [HideInInspector] public int width { get; private set; } //This will not change the size of the array.  They are just used to know the size of it.
    [HideInInspector] public int height { get; private set; }

    private List<Actor>[,] Array; //Contains a 2D array of lists.  Each list contains 0 to N actors.

#if UNITY_EDITOR
    private List<GameObject> gridLines;
#endif

    void Awake()
    {
        width = (int)GridSize.Width;
        height = (int)GridSize.Height;
        initGridContents();
    }

    void initGridContents() // Fill the grid array with empty lists.
    {
        Array = new List<Actor>[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Array[x, y] = new List<Actor>();
            }
        }
    }

    private void Update()
    {
        foreach (var list in Array)
        {
            if(list.Exists(a => a.types.Contains(ActorType.Enemy)))
            {
                sc.noEnemiesflag = false;
                return;
            }
        }

        sc.noEnemiesflag = true;
    }

    public Vector3 GetWorldLocation(int x, int y) //Provides conversion from grid location to world transform
    {
        float fx = transform.position.x + (gridSpacing * x);
        float fy = transform.position.y + (gridSpacing * y);
        Vector3 loc = new Vector3(fx, fy, 0f);
        return loc;
    }

    //Returns true if the grid location is inside the bounds of the grid.
    public bool GetGridLocation(Vector2 worldPos, out int x, out int y) //returns the x and y of the world pos sent.
    {
        worldPos -= (Vector2) transform.position;
        worldPos.x += gridSpacing / 2;

        x = Mathf.FloorToInt(worldPos.x / gridSpacing);
        y = Mathf.FloorToInt(worldPos.y / gridSpacing);

        return (x >= 0 && x < width) && (y >= 0 && y < height);
    }

    public List<Actor> GetAt(int x, int y)
    {
        if(IsLocationInGrid(x,y))
        {
            return Array[x, y];
        } else
        {
            Debug.LogErrorFormat("Cannont get. Location ({0}, {1}) is outside of the grid.", x, y);
            return new List<Actor>();
        }
    }

    public List<Actor> GetAt(Actor actor)
    {
        return GetAt(actor.xLoc, actor.yLoc);
    }

    public void Insert(Actor actor)
    {
        Insert(actor.xLoc, actor.yLoc, actor);
    }

    public void Insert(int x, int y, Actor actor)
    {
        if(IsLocationInGrid(x, y))
        {
            Array[x, y].Add(actor);
        } else
        {
            Debug.LogErrorFormat(actor, "Cannont insert actor. Location ({0}, {1}) is outside of the grid.", x, y);
        }
    }

    public void Remove(int x, int y, Actor actor)
    {
        if(IsLocationInGrid(x, y))
        {
            Array[x, y].RemoveAll(a => a == actor);
        } else
        {
            Debug.LogErrorFormat(actor, "Cannont Delete actor. Location ({0}, {1}) is outside of the grid.", x, y);
        }
    }

    public void Remove(Actor actor)
    {
        Remove(actor.xLoc, actor.yLoc, actor);
    }

    public bool IsLocationInGrid(int x, int y)
    {
        return IsXInGrid(x) && IsYInGrid(y);
    }

    public bool IsYInGrid(int y)
    {
        return y >= 0 && y < height;
    }

    public bool IsXInGrid(int x)
    {
        return x >= 0 && x < width;
    }

#if UNITY_EDITOR
    public void showGridLines(bool show)
    {
        if (gridLines == null) gridLines = new List<GameObject>();

        if (show)
        {
            float z = transform.position.z;
            float XOffset = transform.position.x - gridSpacing / 2;
            float YOffset = transform.position.y;

            float gridWidth = width * gridSpacing;
            float gridHeight = height * gridSpacing;

            for (int x = 0; x < width; x++)
            {
                Vector3 start = new Vector3((x * gridSpacing) + XOffset, YOffset, z);
                Vector3 end = new Vector3((x * gridSpacing) + XOffset, YOffset + gridHeight, z);

                gridLines.Add(DrawLine(start, end));
            }

            for (int y = 0; y < height; y++)
            {
                Vector3 start = new Vector3(XOffset, (y * gridSpacing) + YOffset, z);
                Vector3 end = new Vector3(XOffset + gridWidth, (y * gridSpacing) + YOffset, z);

                gridLines.Add(DrawLine(start, end));
            }
        } else
        {
            foreach (var item in gridLines)
            {
                Destroy(item, .2f);
            }
        }
    }

    GameObject DrawLine(Vector3 start, Vector3 end)
    {
        GameObject myLine = new GameObject();

        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.black;
        lr.endColor = Color.black;
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        return myLine;
    }
#endif
}
