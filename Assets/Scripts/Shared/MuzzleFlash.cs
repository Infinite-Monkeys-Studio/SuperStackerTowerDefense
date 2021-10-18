using UnityEngine;

public class MuzzleFlash : StateMachineBehaviour
{
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Finds the animator in the first child of this gameobject and sets the trigger "Fire"
        animator.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Fire");
    }
}
