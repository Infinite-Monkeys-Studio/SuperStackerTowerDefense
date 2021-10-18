using UnityEngine;
using UnityEngine.Assertions;

public class MinerBrain : BlockBrain
{
    public float EnergyRate;

    private PlayerEnergy playerEnergy;

    private bool seatedOnOre = false; //Used to ensure the miner isn't mining another block
    private Animator anim;

    
    private new void Start()
    {
        base.Start();
        foreach(GameObject gm in gameObject.scene.GetRootGameObjects()) //Finding the player energy gameobject.
        {                                                               //This has to be done in the script since there can't be a referance in the prefab.
            playerEnergy = gm.GetComponent<PlayerEnergy>();
            if(playerEnergy != null) //Was player energy found?
            {
                break; //Stop looking.
            }
        }

        anim = GetComponentInChildren<Animator>();
        Assert.IsNotNull(anim);
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        if(!seatedOnOre && actor.progressToNextLocation > .99f) //Check if we are done moving or if we are seated.
        {
            int below = actor.yLoc - 1;
            if (!grid.IsYInGrid(below)) return; //will there be a block under us?

            if (grid.GetAt(actor.xLoc, below).Exists(a => a.types.Contains(ActorType.Mineable))) // is the miner on ore?
            {
                seatedOnOre = true;  //Start mining.
                playerEnergy.addEnergyRate(EnergyRate); //Simpley add to the rate means there are no calculations being made.  Such Efficent, Much gooder, wow!
                anim.SetTrigger("Begin");
            }
        }
    }

    private new void Death()
    {
        playerEnergy.loseEnergyRate(EnergyRate);
        base.Death();
    }
}
