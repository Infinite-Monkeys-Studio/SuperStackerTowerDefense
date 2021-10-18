using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int MaxHealth = 1;
    private float CurrentHealth;

    [Tooltip("In Damage per second")]
    [SerializeField] private float RegenRate = 0;
    [SerializeField] public GameObject CorpsePrefab; // This is used by brains to know what to spawn on death.  NOt all things with health have a corpse
    
    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    private void FixedUpdate()
    {
        if (CurrentHealth <= 0) die(); // are you dead?

        CurrentHealth += RegenRate * Time.fixedDeltaTime; //Add regen

        if(CurrentHealth > MaxHealth) //Make sure it's not super charging
        {
            CurrentHealth = MaxHealth;
        }
    }

    public void die() 
    {
        SendMessage("Death"); //Call "Death()" in any/all other components in this game object
    }

    public void damage(int damageAmount)
    {
        SendMessage("takeDamage"); //Call "takeDamage()" in any/all other components in this game object
        CurrentHealth -= damageAmount;
        if(CurrentHealth <= 0) //Did it die?
        {
            die();
        }
    }

    public void heal(int healAmount) 
    {
        CurrentHealth += healAmount;
        if(CurrentHealth > MaxHealth) //Check that it didn't over charge
        {
            CurrentHealth = MaxHealth;
        }
    }

    public float getCurrentHealth() //Using this rather than the field prevents classes from doing silly things like adding too much health.
    {
        return CurrentHealth;
    }
}
