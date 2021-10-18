using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(LevelLoading))]
public class GameManager : MonoBehaviour
{
    public LevelLoading levelLoading;

    public Color DropShipColor;

    private void Start()
    {
        levelLoading = GetComponent<LevelLoading>();
        Assert.IsNotNull(levelLoading);
    }

    private void Update()
    {
        
    }
}
