using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.Events;

public class Enemy : Character
{
    int countTurns = 0;

    private void Start()
    {
        GamesMainEvents.UpdateOnBeat += UpdateOnRhythm;
        currentHealhShow.Invoke(ReturnCurrentHealth().ToString());
        currentShieldShow.Invoke(ReturnCurrentShield().ToString());
        currentSpeedShow.Invoke(ReturnCurrentSpeed().ToString());
        burnStateTimes.Invoke("0");
        frozenStateTimes.Invoke("0");
        imbalanceStateTimes.Invoke("0");
        erosionStateTimes.Invoke("0");
        regenerationStateTimes.Invoke("0");
        galvanizationStateTimes.Invoke("0");
    }

    public override void CharacterOnToggleClick(bool isOn)
    {
       
    }

    public override void OnLoseBattle()
    {
        throw new System.NotImplementedException();
    }

    public override void OnWinBattle()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateOnEndOfTurn()
    {
        UpdateState(characterStates);
    }

    public override void UpdateOnRhythm(string marker)
    {
        int.TryParse(marker, out int countParse);
        if (countParse % 2 == 0) return;
        countTurns++;

        if (countTurns >= GamesMainEvents.turnLength)
        {
            //Apply state effects 
            UpdateOnEndOfTurn();
            countTurns = 0;
        }
    }


    public override int DamageHealth(int amount) 
    { 
        
        print("enemy damage " + amount);
        return base.DamageHealth(amount);
    }


    public override int DamageShield(int amount)
    {
        
        return base.DamageShield(amount);
    }

    public override void OnDeath()
    {
       //Send info that enemy is dead to check if all enemies dead
    }
}
