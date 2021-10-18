using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(EnemyWaves))]
public class DebugSpawner : MonoBehaviour
{
#pragma warning disable 0649
    public EnemyWaves waveManager;

    private int FlyerSpawnHeight = -1;

    //Creates a list of named prefabs that the UI uses to say what prefab to spawn
    [System.Serializable]
    public class EnemyDict : SerializableDictionary<string, GameObject> { }

#if UNITY_EDITOR
    //Create a custom editor for this dictionary
    [UnityEditor.CustomPropertyDrawer(typeof(EnemyDict))]
    public class EnemyDictionaryDrawer : DictionaryDrawer<string, GameObject> { }
#endif
    [SerializeField] private EnemyDict NamedEnemies;

    private void Start()
    {
        waveManager = GetComponent<EnemyWaves>();
        Assert.IsNotNull(waveManager);
    }

    public void Spawn(string name)
    {
        GameObject Enemy;

        bool gottem = NamedEnemies.TryGetValue(name, out Enemy);
        Assert.IsTrue(gottem);
        if(Enemy.GetComponent<FlyerBrain>() != null)
        {
            waveManager.SpawnEnemy(Enemy, -1, FlyerSpawnHeight);
        } else
        {
            waveManager.SpawnEnemy(Enemy);
        }
    }

    public void SetFlyerSpawnHeight(float spawnHeight)
    {
        FlyerSpawnHeight = Mathf.RoundToInt(spawnHeight);
    }
}
