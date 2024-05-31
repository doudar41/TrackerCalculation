using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.Events;

public class Player : Character
{
    [SerializeField] Tracker tracker;

    public UnityEvent<Character> sendPlayer;
    int countTurns = 0;

    private void Start()
    {
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
        if(isOn) sendPlayer.Invoke(this);
    }


    public override void OnLoseBattle()
    {
        //Start game from save point 
    }

    public override void OnWinBattle()
    {
        //give experience points which can be used on element upgrade or runepool, health or speed shield
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
        //in case something needs to be changed
    }

    public void PlayerUpgrade(CharacterStats statToUpgrade, int amount) 
    {
        switch (statToUpgrade)
        {
            case CharacterStats.health:
                UpgradeStats(CharacterStats.health , amount);
                break;
            case CharacterStats.shield:
                UpgradeStats(CharacterStats.shield, amount);
                break;
            case CharacterStats.speed:
                UpgradeStats(CharacterStats.speed, amount);
                break;
            case CharacterStats.runepool:
                tracker.SetMaxRunepool(tracker.ReturnMaxRunepool()+amount);
                break;
            case CharacterStats.neutral:
                tracker.TrackerRowsUpgrade(Elements.Neutral, amount);
                break;
            case CharacterStats.fire:
                tracker.TrackerRowsUpgrade(Elements.Fire, amount);
                break;
            case CharacterStats.ice:
                tracker.TrackerRowsUpgrade(Elements.Ice, amount);
                break;
            case CharacterStats.earth:
                tracker.TrackerRowsUpgrade(Elements.Earth, amount);
                break;
            case CharacterStats.air:
                tracker.TrackerRowsUpgrade(Elements.Air, amount);
                break;
        }
    }

    public override void OnDeath()
    {
        GamesMainEvents.LoseBattle();
    }
}


[System.Serializable]
public enum CharacterStats
{
    health,
    shield,
    speed,
    runepool,
    neutral,
    fire,
    ice,
    earth,
    air,
    none
}

