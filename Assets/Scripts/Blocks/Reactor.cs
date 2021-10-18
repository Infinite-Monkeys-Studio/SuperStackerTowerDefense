using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Actor))]
public class Reactor : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private PlayerHealth playerHealth;
    private Actor actor;

    bool firstFrame = true;

    private void Start()
    {
        actor = GetComponent<Actor>();

        Assert.IsNotNull(playerHealth);
        Assert.IsNotNull(actor);

        actor.init();
        actor.grid.Insert(actor);
        actor.JumpToCurrent();
    }

    private void FixedUpdate()
    {
        if(firstFrame)
        {
            if(actor.grid.sc.gm != null)
            {
                GetComponentInChildren<SpriteRenderer>().color = actor.grid.sc.gm.DropShipColor;
                firstFrame = false;
            }
        }

        List<Actor> list = actor.grid.GetAt(actor).FindAll(a => a.types.Contains(ActorType.Enemy));
        if(list.Count > 0)
        {
            foreach(Actor a in list)
            {
                Health enemyHealth = a.GetComponent<Health>();
                enemyHealth.die();
                playerHealth.damage(Mathf.CeilToInt(enemyHealth.getCurrentHealth()));
            }
        }
    }
}
