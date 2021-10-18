using UnityEngine;
using UnityEngine.Assertions;

public class LaserBrain : BaseTurret
{
#pragma warning disable 0649
    private Animator anim;
    [SerializeField] private GameObject LaserBeam;
    private LaserFlash flash;

    private int LastAttackX;

    private new void Start()
    {
        base.Start();
        anim = GetComponentInChildren<Animator>();
        flash = GetComponentInChildren<LaserFlash>();
        Assert.IsNotNull(flash);
        Assert.IsNotNull(anim);
        Assert.IsNotNull(LaserBeam);

        flash.grid = grid;
        flash.LaserBeam = LaserBeam;
        flash.actor = actor;
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();

        LastAttackX = GetLastAttackX();
        FieldOfFireWidth = LastAttackX - actor.xLoc;
        flash.LastAttackX = LastAttackX;

        if (Time.fixedTime - lastShotTime > 1 / rateOfFire) // check that the turret isn't reloading
        {
            for (int x = actor.xLoc; x <= LastAttackX; x++)
            {
                if(grid.GetAt(x, actor.yLoc).Find(a => a.types.Contains(ActorType.Enemy)))
                {
                    attack();   //shoot her!
                    break;
                }
            }
        }
    }

    private int GetLastAttackX()
    {
        int curX = actor.xLoc + 1;
        int curY = actor.yLoc;

        while(curX < grid.width - 1 && !grid.GetAt(curX, curY).Exists(a => a.types.Contains(ActorType.Block)))
        {
            curX += 1;
        }

        return curX;
    }

    public void attack()
    {
        anim.SetTrigger("Fire");
        for (int x = actor.xLoc + 1; x <= LastAttackX; x++)
        {
            foreach (var enemy in grid.GetAt(x, actor.yLoc).FindAll(a => a.types.Contains(ActorType.Enemy)))
            {
                enemy.GetComponent<Health>().damage(damage);
            }
        }

        lastShotTime = Time.fixedTime;
    }
}
