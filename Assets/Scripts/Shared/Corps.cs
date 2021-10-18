using UnityEngine;

public class Corps : MonoBehaviour
{
    public float deathDelay = 0.7f;  // The number of seconds to wait before deleting the game object.
    
    void Start()
    {
        GetComponentInChildren<Animator>().SetTrigger("Die"); //Tell the animatior to die
        Destroy(gameObject, deathDelay); // delete me in X seconds.
    }
}
