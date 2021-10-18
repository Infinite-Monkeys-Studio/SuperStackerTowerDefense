using UnityEngine;
using UnityEngine.Assertions;

public class GateBrain : BlockBrain
{
    private bool GateOpen;
    private Animator anim;

    private new void Start()
    {
        base.Start();

        anim = GetComponentInChildren<Animator>();
        Assert.IsNotNull(anim);

        SetOpen(true);
    }

    private void SetOpen(bool openState)
    {
        GateOpen = openState;
        anim.SetBool("IsOpen", GateOpen);
        anim.SetTrigger("Update");
        if (GateOpen)
        {
            actor.types.Add(ActorType.Passable);
        } else
        {
            actor.types.Remove(ActorType.Passable);
        }
    }

    public override void TriggeredAbility()
    {
        SetOpen(!GateOpen);
        base.TriggeredAbility();
    }
}
