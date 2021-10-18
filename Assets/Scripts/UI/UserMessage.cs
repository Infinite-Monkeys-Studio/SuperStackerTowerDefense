using UnityEngine;
using TMPro;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

public class UserMessage : MonoBehaviour
{
    public TextMeshProUGUI TitleArea;
    public TextMeshProUGUI MessageArea;
    public Button DismissButton;
    public Text ButtonText;

    private bool Visable = false;

    void Start()
    {
        Assert.IsNotNull(TitleArea);
        Assert.IsNotNull(MessageArea);
        Assert.IsNotNull(DismissButton);
        Assert.IsNotNull(ButtonText);
    }

    public void ShowMessageToUser(string Title, string Message, UnityAction callback, string ButtonMessage = "OK")
    {
        Hide();

        TitleArea.text = Title;
        MessageArea.text = Message;
        ButtonText.text = ButtonMessage;

        DismissButton.onClick.RemoveAllListeners();
        if(callback != null)
        {
            DismissButton.onClick.AddListener(callback);
        }

        DismissButton.onClick.AddListener(Hide);

        Show();
    }

    public void SendTestMessage()
    {
        ShowMessageToUser("Test", "This was a test of the Player Alert System.", null, "Great!");
    }

    public void Hide()
    {
        Visable = false;
        gameObject.SetActive(Visable);
    }

    public void Show()
    {
        Visable = true;
        gameObject.SetActive(Visable);
    }

    public void ToggleVisable()
    {
        Visable = !Visable;
        gameObject.SetActive(Visable);
    }
}
