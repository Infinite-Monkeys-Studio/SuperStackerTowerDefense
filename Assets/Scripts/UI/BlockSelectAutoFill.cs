using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Assertions;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class BlockSelectAutoFill : MonoBehaviour
{
    public BlockBrain prefab;

    public TextMeshProUGUI Name;
    public TextMeshProUGUI Cost;
    public CoolDownEffect CoolDown;

    private BlockPlacer blockPlacer;

    private void OnValidate()
    {
        AutoFill();
    }

    private void Start()
    {
        if(Application.isPlaying)
        {
            AutoFill();
        }
    }

    void AutoFill()
    {
        if (prefab == null) return;
        Assert.IsNotNull(Name);
        Assert.IsNotNull(Cost);
        Assert.IsNotNull(CoolDown);

        blockPlacer = FindObjectOfType<BlockPlacer>();
        var namedBlocks = blockPlacer.NamedBlocks;
        var button = GetComponent<Button>();

        Assert.IsNotNull(blockPlacer);
        Assert.IsNotNull(namedBlocks);
        Assert.IsNotNull(button);

        Name.text = prefab.DisplayName;
        Cost.text = prefab.GetComponent<BlockBrain>().EnergyCost.ToString();
        GetComponent<Image>().sprite = prefab.GetComponentInChildren<SpriteRenderer>().sprite;
        CoolDown.SetCoolDownTime(prefab.PlacementCoolDown);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => blockPlacer.SetSelectedBlock(prefab.name, gameObject));

        if (!namedBlocks.ContainsKey(prefab.name))
        {
            namedBlocks.Add(prefab.name, prefab.gameObject);
        }
    }
}
