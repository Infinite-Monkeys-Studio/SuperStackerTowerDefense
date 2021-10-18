using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

public class TextUpdate : MonoBehaviour
{
    private Text text;
    private TextMeshProUGUI tmpText;
    private bool usingTMP;

    void Start()
    {
        text = GetComponent<Text>();
        usingTMP = false;
        if (text == null)
        {
            tmpText = GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(tmpText);
            usingTMP = true;
        }
    }

    public void SetText(int value)
    {
        SetText(value.ToString());
    }

    public void SetText(float value)
    {
        SetText(value.ToString());
    }

    public void SetText(string value)
    {
        if (usingTMP)
        {
            tmpText.text = value.ToString();
        }
        else
        {
            text.text = value.ToString();
        }
    }
}
