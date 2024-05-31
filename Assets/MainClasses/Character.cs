using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.Events;

public abstract class Character : MonoBehaviour
{
    [SerializeField] int healthMax = 100;
    [SerializeField] int shieldMax = 0;
    [SerializeField] int enemySpeedMax = 2;

    int elementDamageModifier = 0; // this should be on tracker properties
    int shieldModifier = 0;

    int currentHealth, currentShield, currentSpeed;

    public Dictionary<CharacterStates, StateProperties> characterStates = new Dictionary<CharacterStates, StateProperties>();

    public UnityEvent<string>   currentHealhShow, currentShieldShow, currentSpeedShow, burnStateTimes, frozenStateTimes,
                                imbalanceStateTimes, erosionStateTimes, regenerationStateTimes, galvanizationStateTimes;


    public UnityEvent<int> elementalModifierChange, shieldModifierChange;
    private void Awake()
    {
        foreach(CharacterStates c in System.Enum.GetValues(typeof(CharacterStates)))
        {
            characterStates.TryAdd(c, new StateProperties(c));
        }
        currentHealth = healthMax;
        currentShield = shieldMax;
        currentSpeed = enemySpeedMax;

    }

    public abstract void UpdateOnRhythm(string marker);
    public abstract void UpdateOnEndOfTurn();
    public abstract void CharacterOnToggleClick(bool isOn);
    public abstract void OnWinBattle();
    public abstract void OnLoseBattle();

    public abstract void OnDeath();

    public int ReturnCurrentHealth() { return currentHealth; }
    public int ReturnCurrentShield() { return currentShield + shieldModifier; }
    public int ReturnCurrentSpeed() { return currentSpeed; }


    private void Start()
    {
        GamesMainEvents.LoseBattle += OnLoseBattle;
        GamesMainEvents.WinBattle += OnWinBattle;
    }

    private void OnDestroy()

    {
        GamesMainEvents.WinBattle -= OnWinBattle;
        GamesMainEvents.LoseBattle -= OnLoseBattle;
        currentHealhShow.RemoveAllListeners();
        currentShieldShow.RemoveAllListeners();
        currentSpeedShow.RemoveAllListeners();
        burnStateTimes.RemoveAllListeners();
        frozenStateTimes.RemoveAllListeners();
        imbalanceStateTimes.RemoveAllListeners();
        erosionStateTimes.RemoveAllListeners();
        regenerationStateTimes.RemoveAllListeners();
        galvanizationStateTimes.RemoveAllListeners();
    }


    public StateProperties ReturnState( CharacterStates state) { return characterStates[state]; }

    public virtual int DamageHealth(int amount) 
    {
        int damage = amount - (currentShield+shieldModifier);
        if (damage > 0)
        {
            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, healthMax);
            currentHealhShow.Invoke(ReturnCurrentHealth().ToString());
        }

        if (currentHealth <= 0) { OnDeath(); }
        return currentHealth; 
    }
    public virtual int DamageShield(int amount) 
    { 
        currentShield -= amount;
        currentShield = Mathf.Clamp(currentShield, 0, shieldMax); 
        currentShieldShow.Invoke(ReturnCurrentShield().ToString());  
        return currentShield; 
    }

    public virtual int DamageSpeed(int amount) 
    { 
        currentSpeed -= 1;
        currentSpeedShow.Invoke(currentSpeed.ToString());
        return currentSpeed; 
    }

    public virtual void SetState(CharacterStates state, int amount, int times)
    {
        characterStates[state].turnTimes += times;
        characterStates[state].amount += amount;
        UpdateStatUI(state, characterStates[state].turnTimes.ToString(), 0);
    }

    public virtual void UpdateState(Dictionary<CharacterStates, StateProperties> allStates)
    {
        foreach (CharacterStates c in System.Enum.GetValues(typeof(CharacterStates)))
        {

        if (allStates[c].turnTimes> 0)
            {
                allStates[c].turnTimes--;
                StateEffectApplyUpdate(characterStates);
                UpdateStatUI(c, allStates[c].turnTimes.ToString(), 0);

                if (allStates[c].turnTimes <= 0)
                {
                    allStates[c].amount = 0;
                    StateEffectApplyUpdate(characterStates);
                    UpdateStatUI(c, allStates[c].turnTimes.ToString(), 0);
                }
            }
        }
    }

    public virtual void StateEffectApplyUpdate(Dictionary<CharacterStates, StateProperties> allStates)
    {

         foreach(CharacterStates c in System.Enum.GetValues(typeof(CharacterStates)))
        {
            switch (c)
            {
                case CharacterStates.Burn:
                    DamageHealth(allStates[CharacterStates.Burn].amount);
                    break;
                case CharacterStates.Frozen:
                    DamageSpeed(allStates[CharacterStates.Frozen].amount);
                    //Damage speed
                    break;
                case CharacterStates.Regeneration:
                    DamageHealth(-allStates[CharacterStates.Regeneration].amount);
                    break;
                case CharacterStates.Erosion:
                    //Matter of gameplay after erosion shield is restored to normal state or stays deminished

                    DamageShield(allStates[CharacterStates.Erosion].amount); 
/*                  shieldModifier = -allStates[CharacterStates.Erosion].amount;
                    shieldModifierChange.Invoke(shieldModifier);*/
                    break;
                case CharacterStates.Imbalance:
                    elementDamageModifier = allStates[CharacterStates.Imbalance].amount;
                    elementalModifierChange.Invoke(elementDamageModifier);
                    break;
                case CharacterStates.Galvinization:
                    elementDamageModifier = allStates[CharacterStates.Galvinization].amount;
                    elementalModifierChange.Invoke(elementDamageModifier);
                    //Add tracker elements modifier
                    break;
                case CharacterStates.ShieldRegeneration:
                    DamageShield(-allStates[CharacterStates.ShieldRegeneration].amount);
                    break;
                case CharacterStates.NoActiveState:
                    break;
            }
        }
    }

    public virtual void Cleansing(CharacterStates state)
    {
        switch (state)
        {
            case CharacterStates.Burn:
                characterStates[CharacterStates.Burn].amount = 0;
                characterStates[CharacterStates.Burn].turnTimes = 0;
                break;
            case CharacterStates.Frozen:
                characterStates[CharacterStates.Frozen].amount = 0;
                characterStates[CharacterStates.Frozen].turnTimes = 0;
                break;
            case CharacterStates.Erosion:
                currentShield = shieldMax;
                break;
            case CharacterStates.Imbalance:
                elementDamageModifier = 0;
                break;
        }

        UpdateState(characterStates);
    }


    public void UpdateStatUI( CharacterStates stat, string text, int number)
    {
        switch (stat)
        {
            case CharacterStates.Burn:

                burnStateTimes.Invoke(text);
                break;
            case CharacterStates.Frozen:
                frozenStateTimes.Invoke(text);
                break;

            case CharacterStates.Erosion:
                //print("does state erosion " + text );
                erosionStateTimes.Invoke(text);
                break;
            case CharacterStates.Imbalance:
                imbalanceStateTimes.Invoke(text);
                break;
            case CharacterStates.Galvinization:
                galvanizationStateTimes.Invoke(text);
                break;
            case CharacterStates.Regeneration:
                regenerationStateTimes.Invoke(text);
                break;

        }
    }
    public void UpgradeStats(CharacterStats stat, int amount)
    {
        switch (stat)
        {
            case CharacterStats.health:
                // need to a formula some amount for one/ten points of health 
                // (healthMax += ((int)(amount/someAmount))*someAmount2) rounded division on someamount
                if (amount > 0) healthMax += ((int)(amount / 10)) * 10;
                break;
            case CharacterStats.shield:
                if (amount > 0) shieldMax += ((int)(amount / 10)) * 10;
                break;
            case CharacterStats.speed:
                enemySpeedMax += ((int)(amount / 10));
                break;

        }
    }


}


[System.Serializable]
public enum CharacterStates
{
    Burn,
    Frozen,
    Regeneration,
    Erosion,
    Imbalance,
    Galvinization,
    ShieldRegeneration,
    NoActiveState
}

[System.Serializable] 
public class StateProperties
{
    public CharacterStates state;
    public int amount;
    public int turnTimes;

    public StateProperties() { }
    public StateProperties(CharacterStates stateEnter)
    {
        state = stateEnter;
        amount = 0;
        turnTimes = 0;
    }

    public StateProperties(CharacterStates stateEnter, int amountEnter, int turnTimesEnter)
    {
        state = stateEnter;
        amount = amountEnter;
        turnTimes = turnTimesEnter;
    }

    public int ReturnTurnTimes() { return turnTimes; }

}