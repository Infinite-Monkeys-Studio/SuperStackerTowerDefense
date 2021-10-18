using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerEnergy : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private int StartingEnergy;
    [SerializeField] private float BaseEnergyRate;

    [Header("UI elements")]
    [SerializeField] private TextMeshProUGUI EnergyStoreValue;
    [SerializeField] private TextMeshProUGUI EnergyRateValue;

    private float CurrentEnergyRate;
    private int CurrentEnergy;
    private float LeftOverEnergy;

    public bool ignoreEneryCosts = false;


    void Start()
    {
        CurrentEnergy = StartingEnergy;
        CurrentEnergyRate = BaseEnergyRate;
    }

    private void Update() //Update the UI to show both the current energy and rate
    {
        EnergyStoreValue.text = string.Format("{0} E", CurrentEnergy);
        EnergyRateValue.text = string.Format("{0} (E/sec)", CurrentEnergyRate);
    }

    
    void FixedUpdate() // add to the energy by the rate
    {
        float newEnergy = CurrentEnergyRate * Time.fixedDeltaTime;  //Calculate the energy generated this frame

        newEnergy += LeftOverEnergy;                                //Add in any energy from last frame

        int energyToAdd = Mathf.FloorToInt(newEnergy);              //Take all the whole energy out.

        LeftOverEnergy = newEnergy - energyToAdd;                   //Save the deciml that's left for next frame

        CurrentEnergy += energyToAdd;                               //add this frame's energy to the total
    }

    public void IgnoreEneryCosts(bool newState) //Allow the UI to toggle spawnwaves for debug stuff.
    {
        ignoreEneryCosts = newState;
    }

    public void addEnergyRate(float rate) // called by things like miners to increase the production rate
    {
        CurrentEnergyRate += rate;
    }

    public bool loseEnergyRate(float rate) // returns true if there was rate to be lost.  False if there wasn't enough energy.
    {
        if(CurrentEnergyRate - rate > 0)
        {
            CurrentEnergyRate -= rate;
            return true;
        }
        return false;
    }

    public void addEnergy(int energy) // called to add a one time energy amount
    {
        CurrentEnergy += energy;
    }

    public bool spendEnergy(int energy) // returns true if the energy was spent.  False if there wasn't enough energy.
    {
        if(CurrentEnergy - energy >= 0 )
        {
            CurrentEnergy -= energy;
            return true;
        }

        return ignoreEneryCosts;
    }
}
