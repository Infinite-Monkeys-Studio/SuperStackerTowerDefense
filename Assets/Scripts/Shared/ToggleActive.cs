using UnityEngine;

public class ToggleActive : MonoBehaviour
{
    public void ToggleActiveState()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
