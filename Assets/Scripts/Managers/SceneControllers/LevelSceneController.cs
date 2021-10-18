using UnityEngine.Events;
using UnityEngine.Assertions;

public class LevelSceneController : SceneController
{
    public bool lastWaveFlag;
    public bool noEnemiesflag;
    public bool lossFlag;

    public UserMessage messanger;
    public PlayerHealth pHealth;

    public void Start()
    {
        Assert.IsNotNull(messanger);
        Assert.IsNotNull(pHealth);
    }

    public void Update()
    {
        if (lossFlag) SendLossMessage();
        if (lastWaveFlag && noEnemiesflag) SendWinMessage();
    }

    public void SendWinMessage()
    {
        noEnemiesflag = lastWaveFlag = false;
        
        if (gm != null)
        {
            UnityAction action = (() => gm.levelLoading.LoadLevel(gm.levelLoading.MainMenuSceneName));
            messanger.ShowMessageToUser("Level Complete", "You have completed the demo level.", action, "Main Menu");
        }
        else
        {
            messanger.ShowMessageToUser("Level Complete", "You have completed the demo level.", null, "Woo Hoo! =)");
        }
    }

    public void SendLossMessage()
    {
        lossFlag = false;
        if (gm != null)
        {
            UnityAction action = (() => gm.levelLoading.LoadLevel(gm.levelLoading.MainMenuSceneName));
            messanger.ShowMessageToUser("Defeat", "You lost.", action, "Main Menu");
        }
        else
        {
            UnityAction action = (() => pHealth.DebugRevive());
            messanger.ShowMessageToUser("Defeat", "You lost.", action, "k =/");
        }
    }
}
