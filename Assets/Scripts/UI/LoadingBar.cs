using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class LoadingBar : MonoBehaviour
{
    Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
    }

    public void SetLoading(bool state)
    {
        slider.gameObject.SetActive(state);
    }

    public void SetValue(float num)
    {
        slider.value = num;
    }
}
