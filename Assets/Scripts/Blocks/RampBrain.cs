using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RampBrain : BlockBrain
{
    
    public List<Vector2> movementPoints;

    bool RampUp = true; //The prefab is designed with the ramp starting up.
    Animator anim;

    new void Start()
    {
        base.Start();

        anim = GetComponentInChildren<Animator>();
        Assert.IsNotNull(anim);

        SetRampState(true);
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();

        if(!RampUp) //is the ramp down/in ramp mode
        {
            List<Actor> enemies = grid.GetAt(actor.xLoc, actor.yLoc).FindAll(a => a.types.Contains(ActorType.Enemy) && !a.types.Contains(ActorType.Block));
            
            foreach (var enemy in enemies)
            {
                var brain = enemy.GetComponent<EnemyBrain>();

                if (brain != null)
                {
                    if(!enemy.usingPointList)
                    {
                        enemy.MoveAlongPointsTo(actor.xLoc - 1, actor.yLoc + 1, brain.movementSpeed, movementPoints);
                    }
                }
            }
        }
    }

    private void SetRampState(bool newState)
    {
        if (RampUp == newState) return;
        RampUp = newState;

        anim.SetBool("RampUp", RampUp);
        anim.SetTrigger("Update");

        if (RampUp)
        {
            actor.types.Remove(ActorType.Passable);
        }
        else
        {
            actor.types.Add(ActorType.Passable);
        }
    }

    public override void TriggeredAbility()
    {
        SetRampState(!RampUp);
    }
}
