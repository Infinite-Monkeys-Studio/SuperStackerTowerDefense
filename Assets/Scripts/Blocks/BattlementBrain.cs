using UnityEngine;
using UnityEngine.Assertions;

public class BattlementBrain : BlockBrain
{
    private bool BattlementOpen;
    private Animator anim;

    private int LeftArmX;
    private int LeftArmY;

    private int RightArmX;
    private int RightArmY;

    private bool HurryUp;

    private new void Start()
    {
        base.Start();

        anim = GetComponentInChildren<Animator>();
        Assert.IsNotNull(anim);

        SetOpen(false);
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();

        if(falling)
        {
            SetOpen(false, true);
        }
    }

    private void SetOpen(bool newState)
    {
        SetOpen(newState, false);
    }

    private void SetOpen(bool newState, bool hurry)
    {
        HurryUp = hurry;

        if (BattlementOpen != newState)
        {
            if (!BattlementOpen)
            {
                LeftArmX = actor.xLoc - 1;
                LeftArmY = actor.yLoc;

                RightArmX = actor.xLoc + 1;
                RightArmY = actor.yLoc;

                if (LeftArmX > 0 && RightArmX < grid.width)
                {
                    if (grid.GetAt(LeftArmX, LeftArmY).Count == 0)
                    {
                        if (grid.GetAt(RightArmX, RightArmY).Count == 0)
                        {
                            BattlementOpen = newState;
                            anim.SetTrigger("Update");
                            anim.SetBool("Open", BattlementOpen);

                            if(HurryUp)
                            {
                                grid.Insert(RightArmX, RightArmY, actor);
                                grid.Insert(LeftArmX, LeftArmY, actor);
                            }
                        }
                    }
                }
            }
            else
            {
                BattlementOpen = newState;
                anim.SetTrigger("Update");
                anim.SetBool("Open", BattlementOpen);

                if (HurryUp)
                {
                    grid.Remove(LeftArmX, LeftArmY, actor);
                    grid.Remove(RightArmX, RightArmY, actor);
                }
            }
        }
    }

    public void AnimationComplete()
    {
        if(!HurryUp)
        {
            if (BattlementOpen)
            {
                grid.Insert(RightArmX, RightArmY, actor);
                grid.Insert(LeftArmX, LeftArmY, actor);
            }
            else
            {
                grid.Remove(LeftArmX, LeftArmY, actor);
                grid.Remove(RightArmX, RightArmY, actor);
            }
        }
    }

    public override void TriggeredAbility()
    {
        SetOpen(!BattlementOpen);
        base.TriggeredAbility();
    }

    protected new void Death()
    {
        grid.Remove(LeftArmX, LeftArmY, actor);
        grid.Remove(RightArmX, RightArmY, actor);
        base.Death();
    }
}
