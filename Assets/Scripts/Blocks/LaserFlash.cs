using UnityEngine;

public class LaserFlash : MonoBehaviour
{
    [HideInInspector] public BlockGrid grid;
    [HideInInspector] public GameObject LaserBeam;
    public GameObject LaserSplat;
    [HideInInspector] public Actor actor;
    [HideInInspector] public int LastAttackX;

    public void SpawnLaserBeam()
    {
        for (int x = actor.xLoc + 1; x < LastAttackX - 1; x++)
        {
            Instantiate(LaserBeam, grid.GetWorldLocation(x, actor.yLoc), transform.rotation, transform);
        }
        Instantiate(LaserSplat, grid.GetWorldLocation(LastAttackX - 1, actor.yLoc), transform.rotation, transform);
    }
}
