using UnityEngine;
using UnityEngine.Assertions;

public class MenuManager : MonoBehaviour
{
    public GameObject PlayMenu;
    public GameObject SettingsMenu;
    public GameObject LevelSelect;
    public SceneController sceneController;

    public SpriteRenderer dropship;
    private int colorI = 0;

    void Start()
    {
        Assert.IsNotNull(dropship);
        PlayMenu.SetActive(true);
        SettingsMenu.SetActive(false);
        LevelSelect.SetActive(false);
    }

    public void ButtonPressed(string ButtonName)
    {
        PlayMenu.SetActive(false);
        SettingsMenu.SetActive(false);
        LevelSelect.SetActive(false);

        switch (ButtonName)
        {
            case "Play":
                LevelSelect.SetActive(true);
                break;
            case "Settings":
                SettingsMenu.SetActive(true);
                break;
            case "Quit":
                Application.Quit();
                break;
            case "Main Menu":
                PlayMenu.SetActive(true);
                break;
        }
    }

    public void LevelSelected(string levelName)
    {
        sceneController.gm.levelLoading.LoadLevel(levelName);
    }

    public void NextDropshipColor()
    {
        sceneController.gm.DropShipColor = getColor(colorI++);
        dropship.color = sceneController.gm.DropShipColor;
    }

    private Color getColor(int i)
    {
        i = i % 8;
        switch(i)
        {
            case 0: return Color.red;
            case 1: return Color.gray;
            case 2: return Color.white;
            case 3: return Color.yellow;
            case 4: return Color.magenta;
            case 5: return Color.green;
            case 6: return Color.cyan;
            case 7: return Color.blue;
            case 8: return Color.black;
            default: return Color.clear;
        }
    }
}
