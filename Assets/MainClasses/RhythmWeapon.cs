using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmWeapon : MonoBehaviour
{
    public QueuedRunesong runesongAtEndOfTurn = new QueuedRunesong();
    public Dictionary<Elements, SequenceRow> trackerRowList = new Dictionary<Elements, SequenceRow>();

    public void CheckElementalInColumn(List<Elements> elementsInColumn, BeatUpdateSource beatSource, string FMODParameter)
    {



        if (elementsInColumn.Count > 0)
        {
            foreach (Elements e in elementsInColumn)
            {
                if (elementsInColumn.Count == 1 || (elementsInColumn.Contains(Elements.Neutral) && elementsInColumn.Count == 2))
                {
                    beatSource.SetParameterToInstance("PlayElement", (int)e + 1);
                    //Play element sound 
                    //This means neutral should be some note that can be played anyy time with base track
                }
                DamageCalc(runesongAtEndOfTurn.target, e);
            }

            if (elementsInColumn.Count > 1)
            {
                {
                    if (elementsInColumn.Contains(Elements.Fire) && elementsInColumn.Contains(Elements.Ice))
                    {
                        beatSource.SetParameterToInstance(FMODParameter, 6);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.FireIce] = runesongAtEndOfTurn.chordsPresent[ChordTypes.FireIce] + 1;
                    }

                    if (elementsInColumn.Contains(Elements.Fire) && elementsInColumn.Contains(Elements.Earth))
                    {
                        beatSource.SetParameterToInstance(FMODParameter, 7);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.FireEarth] = runesongAtEndOfTurn.chordsPresent[ChordTypes.FireEarth] + 1;
                    }

                    if (elementsInColumn.Contains(Elements.Fire) && elementsInColumn.Contains(Elements.Air))
                    {
                        beatSource.SetParameterToInstance(FMODParameter, 8);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.FireAir] = runesongAtEndOfTurn.chordsPresent[ChordTypes.FireAir] + 1;
                    }
                    if (elementsInColumn.Contains(Elements.Ice) && elementsInColumn.Contains(Elements.Earth))
                    {
                        beatSource.SetParameterToInstance(FMODParameter, 9);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.IceEarth] = runesongAtEndOfTurn.chordsPresent[ChordTypes.IceEarth] + 1;
                    }
                    if (elementsInColumn.Contains(Elements.Ice) && elementsInColumn.Contains(Elements.Air))
                    {
                        beatSource.SetParameterToInstance(FMODParameter, 10);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.IceAir] = runesongAtEndOfTurn.chordsPresent[ChordTypes.IceAir] + 1;
                    }
                    if (elementsInColumn.Contains(Elements.Earth) && elementsInColumn.Contains(Elements.Air))
                    {
                        beatSource.SetParameterToInstance(FMODParameter, 11);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.EarthAir] = runesongAtEndOfTurn.chordsPresent[ChordTypes.EarthAir] + 1;
                    }

                    // Three elements

                    if (elementsInColumn.Contains(Elements.Fire) &&
                        elementsInColumn.Contains(Elements.Ice) &&
                        elementsInColumn.Contains(Elements.Earth))
                    {
                        //detectBeat.SetParameterToInstance(FMODParameter, 12);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.FireIceEarth] = runesongAtEndOfTurn.chordsPresent[ChordTypes.FireIceEarth] + 1;
                    }

                    if (elementsInColumn.Contains(Elements.Fire) &&
                        elementsInColumn.Contains(Elements.Ice) &&
                        elementsInColumn.Contains(Elements.Air))
                    {
                        //detectBeat.SetParameterToInstance(FMODParameter, 13);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.FireIceAir] = runesongAtEndOfTurn.chordsPresent[ChordTypes.FireIceAir] + 1;
                    }

                    if (elementsInColumn.Contains(Elements.Ice) &&
                        elementsInColumn.Contains(Elements.Earth) &&
                        elementsInColumn.Contains(Elements.Air))
                    {
                        //detectBeat.SetParameterToInstance(FMODParameter, 15);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.IceEarthAir] = runesongAtEndOfTurn.chordsPresent[ChordTypes.IceEarthAir] + 1;
                    }

                    if (elementsInColumn.Contains(Elements.Fire) &&
                        elementsInColumn.Contains(Elements.Ice) &&
                        elementsInColumn.Contains(Elements.Earth) &&
                        elementsInColumn.Contains(Elements.Air))
                    {
                        //detectBeat.SetParameterToInstance(FMODParameter, 16);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.FireIceEarthAir] = runesongAtEndOfTurn.chordsPresent[ChordTypes.FireIceEarthAir] + 1;
                    }
                }
            }
        }
    }
    public void DamageCalc(Character target, Elements element)
    {
        StartCoroutine(DamageAfterDelay(target, element));
        //print("damage "+element);
    }

    IEnumerator DamageAfterDelay(Character target, Elements element)
    {
        Character newctempCharacter = target;
        Elements e = element;

        yield return new WaitForSeconds(GamesMainEvents.audioLatency);
        //print("pool " + currentRunepool);
        int damage = 0;
        switch (e)
        {
            case Elements.Neutral:
                damage = trackerRowList[Elements.Neutral].rowBaseDamage *
                    trackerRowList[Elements.Neutral].rowLevel;
                newctempCharacter.DamageHealth(damage);
                break;
            case Elements.Fire:
                damage = trackerRowList[Elements.Fire].rowBaseDamage *
                trackerRowList[Elements.Fire].rowLevel;
                newctempCharacter.DamageHealth(damage);
                break;
            case Elements.Ice:
                damage = trackerRowList[Elements.Ice].rowBaseDamage *
                trackerRowList[Elements.Ice].rowLevel;
                newctempCharacter.DamageHealth(damage);
                break;
            case Elements.Earth:
                damage = trackerRowList[Elements.Earth].rowBaseDamage *
                    trackerRowList[Elements.Earth].rowLevel;
                newctempCharacter.DamageHealth(damage);
                break;
            case Elements.Air:
                damage = trackerRowList[Elements.Air].rowBaseDamage *
                trackerRowList[Elements.Air].rowLevel;
                newctempCharacter.DamageHealth(damage);
                break;
        }

        //UI changes
        //VFX
    }


    public IEnumerator OnFinishRuneSong(QueuedRunesong newsong)
    {

        int countForceOfCombo = 0;
        //Apply rune combos at the and of runesong
        QueuedRunesong q = new QueuedRunesong(newsong);

        if (q.occupiedRuneSlotsinRow[Elements.Fire] > 0)
        {
            int fireRand = Random.Range(0, 100);
            if (fireRand < q.occupiedRuneSlotsinRow[Elements.Fire] * 10)
            {
                print("BURN !!! " + q.target);
                q.target.SetState(CharacterStates.Burn, q.occupiedRuneSlotsinRow[Elements.Fire] * (int)(q.savedRunesongPattern[Elements.Fire].rowBaseDamage * 0.5f), 2);
                q.target.UpdateStatUI(CharacterStates.Burn, q.target.ReturnState(CharacterStates.Burn).turnTimes.ToString(),0);
                countForceOfCombo++;
            }
            fireRand = Random.Range(0, 100);
            if (fireRand < q.occupiedRuneSlotsinRow[Elements.Fire] * 10)
            {
                q.runesongStarter.Cleansing(CharacterStates.Burn);
                q.runesongStarter.Cleansing(CharacterStates.Frozen);
                q.runesongStarter.Cleansing(CharacterStates.Erosion);
                q.runesongStarter.Cleansing(CharacterStates.Imbalance);
                countForceOfCombo++;
            }
        }

        ///
        /// Ice States 
        /// 

        if (q.occupiedRuneSlotsinRow[Elements.Ice] > 0)
        {
            q.target.SetState(CharacterStates.Frozen, q.occupiedRuneSlotsinRow[Elements.Ice] * 2, 5);
            q.runesongStarter.SetState(CharacterStates.Regeneration, q.occupiedRuneSlotsinRow[Elements.Ice] * 5, 5); //one point for rune is small number
            countForceOfCombo++;
        }

        ///
        /// Earth states
        ///


        if (q.occupiedRuneSlotsinRow[Elements.Earth] > 0)
        {
            //print("Erosion " + q.target);
            q.target.SetState(CharacterStates.Erosion, q.occupiedRuneSlotsinRow[Elements.Earth], 3);
            q.target.DamageShield(q.occupiedRuneSlotsinRow[Elements.Earth]);
            countForceOfCombo++;
        }

        ///
        /// Air States
        ///

        if (q.occupiedRuneSlotsinRow[Elements.Air] > 0)
        {
            q.runesongStarter.SetState(CharacterStates.Galvinization, q.occupiedRuneSlotsinRow[Elements.Air], 1);
            q.target.SetState(CharacterStates.Imbalance, -q.occupiedRuneSlotsinRow[Elements.Air], 1);
            countForceOfCombo++;
        }

        ///
        /// Fire + Ice
        ///
        if (q.chordsPresent[ChordTypes.FireIce] > 0)
        {
            int fireIceRand = Random.Range(0, 100);
            if (fireIceRand < q.chordsPresent[ChordTypes.FireIce] * 10)
            {
                q.runesongStarter.Cleansing(CharacterStates.Frozen);
            }
            q.target.SetState(CharacterStates.Burn, (int)(q.chordsPresent[ChordTypes.FireIce] * q.savedRunesongPattern[Elements.Fire].rowBaseDamage * 0.5f), 1);
            q.target.SetState(CharacterStates.Frozen, q.chordsPresent[ChordTypes.FireIce], 5);
            countForceOfCombo++;
        }

        ///
        /// Fire + Earth
        ///
        if (q.chordsPresent[ChordTypes.FireEarth] > 0)
        {
            int fireEarthRand = Random.Range(0, 100);
            if (fireEarthRand < q.chordsPresent[ChordTypes.FireEarth] * 10)
            {
                q.runesongStarter.Cleansing(CharacterStates.Erosion);
            }
            countForceOfCombo++;
            //What it wll be Erosion points
        }

        ///
        /// Fire + Air
        ///
        if (q.chordsPresent[ChordTypes.FireAir] > 0)
        {
            int fireAirRand = Random.Range(0, 100);
            if (fireAirRand < q.chordsPresent[ChordTypes.FireAir] * 10)
            {
                q.runesongStarter.Cleansing(CharacterStates.Imbalance);
            }

            q.target.DamageHealth(q.chordsPresent[ChordTypes.FireAir] * q.savedRunesongPattern[Elements.Air].rowBaseDamage);
            countForceOfCombo++;
        }

        ///
        /// Ice + Earth
        ///
        if (q.chordsPresent[ChordTypes.IceEarth] > 0)
        {
            q.runesongStarter.SetState(CharacterStates.ShieldRegeneration, q.chordsPresent[ChordTypes.IceEarth], 5);
            print("ice + earth points " + q.chordsPresent[ChordTypes.IceEarth]);
            q.target.SetState(CharacterStates.Imbalance, -q.chordsPresent[ChordTypes.IceEarth], 1);
            countForceOfCombo++;
            // This inable weak enemy to deal damage at al, it will be good to randomize this 
        }


        ///
        /// Ice + Air 
        ///
        if (q.chordsPresent[ChordTypes.IceAir] > 0)
        {
            q.runesongStarter.SetState(CharacterStates.Galvinization, q.chordsPresent[ChordTypes.IceAir], 5);
            //print("frozen points " + q.target.ReturnStatesPoints(SequencerUserStates.Frozen));
            q.target.SetState(CharacterStates.Imbalance, -q.target.ReturnState(CharacterStates.Frozen).amount, 1);
            countForceOfCombo++;
        }


        ///
        /// Earth + Air
        ///
        if (q.chordsPresent[ChordTypes.EarthAir] > 0)
        {
            q.runesongStarter.SetState(CharacterStates.Galvinization, q.chordsPresent[ChordTypes.EarthAir], 1);
            q.target.DamageHealth(q.target.ReturnState(CharacterStates.Erosion).turnTimes);
            countForceOfCombo++;
        }
        // countForceOfCombo/allCombos gets float used as parameter for music/sound effect at the end of runesong


        yield return null;
    }

}





[System.Serializable]
public enum ChordTypes
{
    FireIce,
    FireEarth,
    FireAir,
    IceEarth,
    IceAir,
    EarthAir,
    FireIceEarth,
    FireIceAir,
    IceEarthAir,
    FireIceEarthAir,
    None //enum can't be null
}

[System.Serializable]
public enum Elements
{
    Neutral,
    Fire,
    Ice,
    Earth,
    Air
}



[System.Serializable]
public class SequenceRow
{
    Dictionary<int, bool> Row = new Dictionary<int, bool>();
    public Elements rowEffect;
    public int rowLevel = 1;
    public int rowBaseDamage = 10;

    public void SetStepInRow(int index, bool isOccupied)
    {
        if (!Row.ContainsKey(index))
        {
            if (Row.TryAdd(index, isOccupied))
            {

                //Debug.Log("added successfully " + GetStepFromRow(index));
            }
        }
        else
        {
            Row[index] = isOccupied;
            //Debug.Log("change successfully " + GetStepFromRow(index));
        }

    }

    public int GetDictionaryCount()
    {
        foreach (int i in Row.Keys)
        {
            //Debug.Log(i + " fff");
        }
        return Row.Count;
    }

    public bool GetRunePositionFromRow(int index)
    {
        if (Row.TryGetValue(index, out bool isOccupied))
        {
            //Debug.Log("check value " + isOccupied);
            return isOccupied;
        }
        return false;
    }

    public void RemoveStepFromRow(int index)
    {
        Row.Remove(index);
    }

}


public class QueuedRunesong
{
    public Dictionary<Elements, SequenceRow> savedRunesongPattern = new Dictionary<Elements, SequenceRow>();
    public Character target;
    public Character runesongStarter;
    public Dictionary<ChordTypes, int> chordsPresent = new Dictionary<ChordTypes, int>();
    public Dictionary<Elements, int> occupiedRuneSlotsinRow = new Dictionary<Elements, int>();

    public QueuedRunesong()
    {
        savedRunesongPattern.Add(Elements.Neutral, new SequenceRow());
        occupiedRuneSlotsinRow.Add(Elements.Neutral, 0);
        savedRunesongPattern.Add(Elements.Fire, new SequenceRow());
        occupiedRuneSlotsinRow.Add(Elements.Fire, 0);
        savedRunesongPattern.Add(Elements.Ice, new SequenceRow());
        occupiedRuneSlotsinRow.Add(Elements.Ice, 0);
        savedRunesongPattern.Add(Elements.Earth, new SequenceRow());
        occupiedRuneSlotsinRow.Add(Elements.Earth, 0);
        savedRunesongPattern.Add(Elements.Air, new SequenceRow());
        occupiedRuneSlotsinRow.Add(Elements.Air, 0);

        chordsPresent.Add(ChordTypes.FireIce, 0);
        chordsPresent.Add(ChordTypes.EarthAir, 0);
        chordsPresent.Add(ChordTypes.FireAir, 0);
        chordsPresent.Add(ChordTypes.FireEarth, 0);
        chordsPresent.Add(ChordTypes.IceAir, 0);
        chordsPresent.Add(ChordTypes.IceEarth, 0);
        chordsPresent.Add(ChordTypes.FireIceAir, 0);
        chordsPresent.Add(ChordTypes.FireIceEarth, 0);
        chordsPresent.Add(ChordTypes.IceEarthAir, 0);
        chordsPresent.Add(ChordTypes.FireIceEarthAir, 0);




    }

    public QueuedRunesong(Character target, Character player)
    {
        savedRunesongPattern.Add(Elements.Neutral, new SequenceRow());
        occupiedRuneSlotsinRow.Add(Elements.Neutral, 0);
        savedRunesongPattern.Add(Elements.Fire, new SequenceRow());
        occupiedRuneSlotsinRow.Add(Elements.Fire, 0);
        savedRunesongPattern.Add(Elements.Ice, new SequenceRow());
        occupiedRuneSlotsinRow.Add(Elements.Ice, 0);
        savedRunesongPattern.Add(Elements.Earth, new SequenceRow());
        occupiedRuneSlotsinRow.Add(Elements.Earth, 0);
        savedRunesongPattern.Add(Elements.Air, new SequenceRow());
        occupiedRuneSlotsinRow.Add(Elements.Air, 0);

        chordsPresent.Add(ChordTypes.FireIce, 0);
        chordsPresent.Add(ChordTypes.EarthAir, 0);
        chordsPresent.Add(ChordTypes.FireAir, 0);
        chordsPresent.Add(ChordTypes.FireEarth, 0);
        chordsPresent.Add(ChordTypes.IceAir, 0);
        chordsPresent.Add(ChordTypes.IceEarth, 0);
        chordsPresent.Add(ChordTypes.FireIceAir, 0);
        chordsPresent.Add(ChordTypes.FireIceEarth, 0);
        chordsPresent.Add(ChordTypes.IceEarthAir, 0);
        chordsPresent.Add(ChordTypes.FireIceEarthAir, 0);
        this.target = target;
        runesongStarter = player;

    }
    public QueuedRunesong(QueuedRunesong q)
    {
        savedRunesongPattern = q.savedRunesongPattern;
        target = q.target;
        runesongStarter = q.runesongStarter;
        chordsPresent = new Dictionary<ChordTypes, int>( q.chordsPresent);
        occupiedRuneSlotsinRow = new Dictionary<Elements, int>( q.occupiedRuneSlotsinRow);

    }




}