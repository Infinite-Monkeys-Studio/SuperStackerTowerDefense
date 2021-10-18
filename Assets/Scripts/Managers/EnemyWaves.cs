using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class EnemyWaves : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private TextMeshProUGUI WaveCounter;
    [SerializeField] private BlockGrid grid;
    [SerializeField] private Transform SpawnPoint;
    private int DefaultSpawnX = (int)GridSize.Width - 1;
    private int DefaultSpawnY = 1;

    public LevelSceneController sc;
    public UserMessage messanger;

    public int StartingWave;

    public List<Wave> waves;

    public bool spawnWaves = true; //This is a debug value. Only use it to disable the waves.
    
    void Start()
    {
        Assert.IsNotNull(messanger);
        Assert.IsNotNull(sc);
        Assert.IsNotNull(WaveCounter);
        Assert.IsNotNull(grid);
        Assert.IsNotNull(SpawnPoint);
        //All waves are spawned in coroutines.  This make timing very simple and doesn't slow down the main thread.
        //Because coroutines loose their reference to the manager a reference must be passed in
        StartCoroutine(Spawnwaves(this));
    }

    public void SpawnWaves(bool newState) //Allow the UI to toggle spawnwaves for debug stuff.
    {
        spawnWaves = newState;
    }

    IEnumerator SetLastWaveFlag(float delay)
    {
        yield return new WaitForSeconds(delay);
        sc.lastWaveFlag = true;
    }

    //Loops through the waves. and starts each one at the right time
    IEnumerator Spawnwaves(EnemyWaves manager)
    {
        //int WaveNumber = StartingWave + 1;
        for (int i = StartingWave; i<waves.Count; i++)
        {
            var wave = waves[i];
            wave.init(); // allow the wave to calculate it's duration
            StartCoroutine(SpawnWave(wave, manager)); // Start the wave
            WaveCounter.text = (i+1).ToString();
            //WaveNumber++;
            yield return new WaitForSeconds(wave.Duration); //Wait for the duration of the wave
        }
    }

    //controls the spawning of enemies in a wave.
    IEnumerator SpawnWave(Wave wave, EnemyWaves manager)
    {
        yield return new WaitForSeconds(wave.PreDelay); //Wait for the wave to start

        foreach (var enemy in wave.Enemies) //start all the enemy coroutines.
        {
            StartCoroutine(wave.SpawnType(enemy, manager)); // each of the enemy types controls it's own timing.
        }

        StartCoroutine(SetLastWaveFlag(wave.Duration));
    }

    public void SpawnEnemy(GameObject prefab)
    {
        SpawnEnemy(prefab, -1, -1);
    }

    // This must be in the manager since it has a reference to the spawnpoint and the grid.
    public void SpawnEnemy(GameObject prefab, int x, int y)
    {
        GameObject newEnemy = Instantiate(prefab, SpawnPoint.position, SpawnPoint.rotation, grid.transform); //Set the posistion, rotation, and parent of the new enemy
        newEnemy.GetComponent<Actor>().xLoc = x == -1 ? DefaultSpawnX : x; //Tell it's actor where to go.
        newEnemy.GetComponent<Actor>().yLoc = y == -1 ? DefaultSpawnY : y;
    }
}

[System.Serializable]
public class Wave
{
    public string Name;
    public float PreDelay;
    public float Duration { get; private set; }
    public bool FinalWave;

    //This is like a class but is just a way to keep data together.
    [System.Serializable]
    public struct EnemyType
    {
        public GameObject Prefab;
        public float PreDelay;
        public float SpawnDelay;
        public int SpawnCount;
        public int SpawnX;
        public int SpawnY;
    }

    public List<EnemyType> Enemies;

    public void init() //Calcualtes this waves duration
    {
        float longestType = 0; //Find which enemy type takes the most time to spawn
        foreach (var type in Enemies)
        {
            float thisType = type.PreDelay + type.SpawnCount * (type.SpawnDelay); // calculate the duration of this type
            if(thisType > longestType)
            {
                longestType = thisType;
            }
        }
        Duration = PreDelay + longestType; // set the duration to the longest enemytype plus the predelay
    }

    // Spawns an individual enemy type. One to several of these run for each wave.
    public IEnumerator SpawnType(EnemyType type, EnemyWaves waveManager)
    {
        yield return new WaitForSeconds(type.PreDelay); //Wait to start this type

        for (int i = 0; i < type.SpawnCount; i++) // spawn the right number
        {
            if (waveManager.spawnWaves) // check that spawning hasn't been disabled.
            {
                // spawn one of this type
                waveManager.SpawnEnemy(type.Prefab, type.SpawnX, type.SpawnY);
            }

            yield return new WaitForSeconds(type.SpawnDelay); // wait to spawn the next one
        }
    }
}