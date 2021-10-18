using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    public void AnimationComplete() //Called by the animation event
    {
        Destroy(gameObject);

    }
}
