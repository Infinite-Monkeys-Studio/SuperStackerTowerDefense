using System.Collections.Generic;
using UnityEngine;

public class SawBrain : BaseTurret
{
    private List<Actor> currentTargets;
    
    private new void Start()
    {
        base.Start();
        currentTargets = new List<Actor>();
    }
    
    private new void Update()
    {
        base.Update();
    }
    
    private new void FixedUpdate()
    {
        base.FixedUpdate();

        currentTargets = FindNewTargets(200);

        if (currentTargets.Count > 0) //Check that there are targets
        {
            if (Time.fixedTime - lastShotTime > 1 / rateOfFire)
            {
                attack();
            }

        }
    }

    public void attack()
    {
        foreach(Actor e in currentTargets) //Attack each target
        {
            if (e != null) //Final sanity check that it's not null
            {
                e.GetComponent<Health>().damage(damage);
            }
        }
 
        lastShotTime = Time.fixedTime;
    }

}
