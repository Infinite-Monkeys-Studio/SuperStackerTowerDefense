using UnityEngine;
using UnityEngine.Assertions;

public class HelpScreen : MonoBehaviour
{
    public TimeManager timeManager;
    public bool ShowAtStart;
    private bool Showing;

    void Start()
    {
        Assert.IsNotNull(timeManager);

        Hide(); //Do this to make sure everything is in the right state
        if (ShowAtStart) Show();
    }

    void Update()
    {
        if (Showing && Input.anyKey)
        {
            Hide();
        }
    }

    public void SetShowing(bool newState)
    {
        Showing = newState;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(Showing);
        }

        timeManager.SetPause(Showing);
    }

    public void Show()
    {
        SetShowing(true);
    }

    public void Hide()
    {
        SetShowing(false);
    }
}
