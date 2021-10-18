using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

[RequireComponent(typeof(GameManager))]
public class LevelLoading : MonoBehaviour
{
    public string MainMenuSceneName;
    public string LoadingSceneName;
    //public List<string> LevelNames;

    GameManager gm;

    Scene MainMenuScene;
    Scene LoadingScene;

    LoadingBar loadingBar;

    AsyncOperation LevelLoadingOperation;
    Scene LoadedLevel;

    void Start()
    {
        gm = GetComponent<GameManager>();

        Assert.IsNotNull(gm);

        // load the loading scene
        SceneManager.LoadScene(LoadingSceneName, LoadSceneMode.Additive);
        LoadingScene = SceneManager.GetSceneByName(LoadingSceneName);
        SetGamemanagerInScene(LoadingScene);

        StartCoroutine(loadMainMenu()); // this has to wait until things have loaded to do some stuff.
    }

    private void Update()
    {
        if (LevelLoadingOperation != null) // is something currently loading?
        {
            loadingBar.SetValue(LevelLoadingOperation.progress); //update the loading bar

            if(LevelLoadingOperation.isDone) // is it done loading?
            {
                loadingBar.SetLoading(false); 
                LevelLoadingOperation = null;
                SetGamemanagerInScene(LoadedLevel);     // grabs the scene controller and gives it a reference to the gamemanager
                SceneManager.SetActiveScene(LoadedLevel); //activate the scene that was just loaded
            }
        }


    }

    //Loads the given level name.
    public void LoadLevel(string levelName)
    {
        SceneManager.SetActiveScene(LoadingScene);

        LevelLoadingOperation = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

        loadingBar.SetLoading(true);

        if(LoadedLevel.IsValid())
        {
            SceneManager.UnloadSceneAsync(LoadedLevel);
        }
        
        LoadedLevel = SceneManager.GetSceneByName(levelName);
    }

    //Find the loading bar in the loading scene and set it to loadingBar
    private void GetLoadingBar()
    {
        GameObject[] objs = LoadingScene.GetRootGameObjects();
        for (int i = 0; i < objs.Length; i++)
        {
            LoadingBar bar = objs[i].GetComponentInChildren<LoadingBar>();
            if (bar != null)
            {
                loadingBar = bar;

                return;
            }
        }
    }

    //Find the sceneController in the other scene and set it's reference to gamemanager
    private void SetGamemanagerInScene(Scene scene)
    {
        GameObject[] objs = scene.GetRootGameObjects();
        for (int i = 0; i < objs.Length; i++)
        {
            SceneController sm = objs[i].GetComponent<SceneController>();
            if (sm != null)
            {
                sm.setGameManager(gm);
                return;
            }
        }
    }

    //Wait a bit to let stuff load then load the main menu.
    IEnumerator loadMainMenu()
    {
        yield return new WaitForSeconds(1);
        GetLoadingBar();
        yield return new WaitForSeconds(1);
        LoadLevel(MainMenuSceneName);
        yield return new WaitForSeconds(1);
        MainMenuScene = SceneManager.GetSceneByName(MainMenuSceneName);
    }
}
