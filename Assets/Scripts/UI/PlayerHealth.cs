using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Assertions;

public class PlayerHealth : MonoBehaviour
{
#pragma warning disable 0649
    public int MaxHealth;
    [HideInInspector] private int CurrentHealth;
    [SerializeField] private Slider HealthBar;
    [SerializeField] private UserMessage Messanger;
    [SerializeField] private LevelSceneController sc;

    private TextMeshProUGUI HealthText;
    
    void Start()
    {
        CurrentHealth = MaxHealth;

        HealthText = HealthBar.GetComponentInChildren<TextMeshProUGUI>();
        Assert.IsNotNull(HealthText);
        Assert.IsNotNull(Messanger);
        Assert.IsNotNull(sc);
        HealthBar.maxValue = MaxHealth;
        setHealthPercent();
        setHealthText();
    }

    private void Update() //UPdate the bare and the text readout
    {
        setHealthPercent();
        setHealthText();
    }

    
    void FixedUpdate() //Check if the player is dead
    {
        if(CurrentHealth <= 0)
        {
            die();
        }
    }

    void die()
    {
        sc.lossFlag = true;       
    }

    public void damage(int damage)
    {
        CurrentHealth -= damage;
    }

    public int getCurrentHealth()
    {
        return CurrentHealth;
    }

    void setHealthText()
    {
        HealthText.text = string.Format("{0} / {1}", CurrentHealth, MaxHealth);
    }

    void setHealthPercent()
    {
        HealthBar.value = CurrentHealth;
    }

    public void DebugRevive()
    {
        CurrentHealth = MaxHealth;
    }
}
