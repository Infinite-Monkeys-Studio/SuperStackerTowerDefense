using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public float DeathTime = 1;

    private void Start()
    {
        Destroy(gameObject, DeathTime);
    }
}
