using UnityEngine;

public class AnimationCompletePassUp : MonoBehaviour
{
    public void AnimationComplete()
    {
        transform.parent.SendMessage("AnimationComplete");
    }
}
