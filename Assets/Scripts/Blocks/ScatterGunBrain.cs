using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ScatterGunBrain : BaseTurret
{
    public int NumberOfShots = 3;
    private Animator anim;
    private List<Actor> currentTargets;
    
    private new void Start()
    {
        base.Start();
        anim = GetComponentInChildren<Animator>();
        Assert.IsNotNull(anim);
    }
    
    private new void Update()
    {
        base.Update();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();

        currentTargets = FindNewTargets(NumberOfShots); // find at most as many targets as there are shots.

        if (currentTargets.Count > 0) //Check there are targets to shoot.
        {
            if (Time.fixedTime - lastShotTime > 1 / rateOfFire)
            {
                attack();
            }
        }
    }

    public void attack()
    {
        int shot = 0;
        foreach(Actor a in currentTargets)
        {
            if(shot < NumberOfShots)
            {
                //Shoot each of the currentTargets. as long as there are shots.
                currentTargets[shot].GetComponent<Health>().damage(damage);
                shot++;
            } else
            {
                break;
            }   
        }
        
        anim.SetTrigger("Fire");
        lastShotTime = Time.fixedTime;
    }
}
