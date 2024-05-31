using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;


public class Tracker : RhythmWeapon
{

    [SerializeField] List<SequenceRow> rows = new List<SequenceRow>();
    
    List<Elements> elementsInColumn = new List<Elements>();

    [SerializeField] Character target, player;
    [SerializeField] int runePoolMax; 

    int currentRunepool; 
    int currentColumn = -1;
    int elementalDamageModifier = 0;
    bool doesRuneSongPlaying = false;

    public UnityEvent<int> updateUIColumn; //for Tracker UI
    public UnityEvent<string> updateRunepoolUI;
    public UnityEvent stopPlayingRunesong;

    public UnityEvent<string> damageNeutralUI, damageFireUI, damageIceUI, damageEarthUI, damageAirUI;

    [SerializeField] Button playButton;

    BeatUpdateSource beatSource;
    [SerializeField] string FMODParameter = "PlayElement";

    private void Start()
    {
        beatSource = FindAnyObjectByType<BeatUpdateSource>();
        GamesMainEvents.UpdateOnBeat += UpdateOnRhythm;

        foreach (SequenceRow s in rows)
        {
            trackerRowList.Add(s.rowEffect, s);
        }

        playButton.onClick.AddListener(PlayRunesong);
        currentRunepool = runePoolMax;

        updateRunepoolUI.Invoke(currentRunepool.ToString());
        damageNeutralUI.Invoke((trackerRowList[Elements.Neutral].rowBaseDamage + elementalDamageModifier).ToString());
        damageFireUI.Invoke((trackerRowList[Elements.Fire].rowBaseDamage + elementalDamageModifier).ToString());
        damageIceUI.Invoke((trackerRowList[Elements.Ice].rowBaseDamage + elementalDamageModifier).ToString());
        damageEarthUI.Invoke((trackerRowList[Elements.Earth].rowBaseDamage + elementalDamageModifier).ToString());
        damageAirUI.Invoke((trackerRowList[Elements.Air].rowBaseDamage + elementalDamageModifier).ToString());
    }

    public void TrackerRowsUpgrade(Elements element, int amount)
    {
        trackerRowList[element].rowLevel = trackerRowList[element].rowLevel + amount;
    }


    public void GetToggleFromTrackerUI(Elements element, int rowPosition, bool isOccupied)
    {
        trackerRowList[element].SetStepInRow(rowPosition, isOccupied);
    }

    public void GetElementalModifierFromPlayer(int amount)
    {
        elementalDamageModifier = amount;
        damageNeutralUI.Invoke((trackerRowList[Elements.Neutral].rowBaseDamage + elementalDamageModifier).ToString());
        damageFireUI.Invoke((trackerRowList[Elements.Fire].rowBaseDamage + elementalDamageModifier).ToString());
        damageIceUI.Invoke((trackerRowList[Elements.Ice].rowBaseDamage + elementalDamageModifier).ToString());
        damageEarthUI.Invoke((trackerRowList[Elements.Earth].rowBaseDamage + elementalDamageModifier).ToString());
        damageAirUI.Invoke((trackerRowList[Elements.Air].rowBaseDamage + elementalDamageModifier).ToString());
    }

    public void SetMaxRunepool(int amount)
    {
        runePoolMax = amount;
    }

    public int ReturnMaxRunepool() { return runePoolMax; }


    public void PlayRunesong() //On Play button click 
    {
        if (target == null) { print("no target"); return; }
        if (player.characterStates[CharacterStates.Frozen].turnTimes > 0)
        {
            playButton.onClick.RemoveListener(PlayRunesong);
            playButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Frozen";
            StartCoroutine(DelayBeforeRunesong());
        }
        else
        {
            doesRuneSongPlaying = true;
            runesongAtEndOfTurn = new QueuedRunesong(target, player);
            playButton.onClick.RemoveListener(PlayRunesong);
        }
    }

    IEnumerator DelayBeforeRunesong()
    {
        yield return new WaitForSeconds(player.characterStates[CharacterStates.Frozen].turnTimes);
        doesRuneSongPlaying = true;
        runesongAtEndOfTurn = new QueuedRunesong(target, player);
        playButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Play";
    }

    public void UpdateOnRhythm(string intFromFMODMarker) //fires at Every signal from beat detection script
    {

        if (!doesRuneSongPlaying) return;
        elementsInColumn.Clear();

        int.TryParse(intFromFMODMarker, out int countParse);
        if (countParse % 2 == 0 && currentColumn == -1) return;

        if (countParse % 2 != 0)
        {
            foreach (Elements e in System.Enum.GetValues(typeof(Elements)))
            {
                if (trackerRowList[e].GetStepFromRow(currentColumn+1))
                {
                    elementsInColumn.Add(e);
                    if (!runesongAtEndOfTurn.savedRunesongPattern.TryAdd(e, trackerRowList[e]))
                    {

                        runesongAtEndOfTurn.savedRunesongPattern[e] = trackerRowList[e];
                        runesongAtEndOfTurn.occupiedRuneSlotsinRow[e] = runesongAtEndOfTurn.occupiedRuneSlotsinRow[e] + 1;
                        
                    }
                }
            }
            currentColumn++;
        }
        else 
        {
            beatSource.SetParameterToInstance("PlayElement", 0);
            currentColumn++;
        }

        // Registering and play chords 
        updateUIColumn.Invoke((currentColumn / 2));
        //CheckRunesInColumn();
        CheckElementalInColumn(elementsInColumn, beatSource, FMODParameter);
        //currentColumn++;
        if (currentColumn > 15)
        {
            StartCoroutine(OnFinishRuneSong(runesongAtEndOfTurn));
            stopPlayingRunesong.Invoke();
            currentColumn = -1;
            doesRuneSongPlaying = false;
            playButton.onClick.AddListener(PlayRunesong);
            //detectBeat.SetParameterToInstance("PlayElement", 0);
        }
    }


    void CheckRunesInColumn()
    {
        // Registering and play chords  
        //print("elements " + elementsInColumn.Count);

        if (elementsInColumn.Count > 0)
        {
            foreach (Elements e in elementsInColumn)
            {
                if (elementsInColumn.Count == 1 || (elementsInColumn.Contains(Elements.Neutral)&& elementsInColumn.Count == 2))
                {
                    beatSource.SetParameterToInstance("PlayElement", (int)e + 1);
                    //Play element sound 
                    //This means neutral should be some note that can be played anyy time with base track
                }
                DamageCalc(target, e);
            }

            if (elementsInColumn.Count > 1)
            {
                {
                    if (elementsInColumn.Contains(Elements.Fire) && elementsInColumn.Contains(Elements.Ice))
                    {
                        beatSource.SetParameterToInstance("PlayElement", 6);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.FireIce] = runesongAtEndOfTurn.chordsPresent[ChordTypes.FireIce] + 1;
                    }

                    if (elementsInColumn.Contains(Elements.Fire) && elementsInColumn.Contains(Elements.Earth))
                    {
                        beatSource.SetParameterToInstance("PlayElement", 7);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.FireEarth] = runesongAtEndOfTurn.chordsPresent[ChordTypes.FireEarth] + 1;
                    }

                    if (elementsInColumn.Contains(Elements.Fire) && elementsInColumn.Contains(Elements.Air))
                    {
                        beatSource.SetParameterToInstance("PlayElement", 8);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.FireAir] = runesongAtEndOfTurn.chordsPresent[ChordTypes.FireAir] + 1;
                    }
                    if (elementsInColumn.Contains(Elements.Ice) && elementsInColumn.Contains(Elements.Earth))
                    {
                        beatSource.SetParameterToInstance("PlayElement", 9);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.IceEarth] = runesongAtEndOfTurn.chordsPresent[ChordTypes.IceEarth] + 1;
                    }
                    if (elementsInColumn.Contains(Elements.Ice) && elementsInColumn.Contains(Elements.Air))
                    {
                        beatSource.SetParameterToInstance("PlayElement", 10);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.IceAir] = runesongAtEndOfTurn.chordsPresent[ChordTypes.IceAir] + 1;
                    }
                    if (elementsInColumn.Contains(Elements.Earth) && elementsInColumn.Contains(Elements.Air))
                    {
                        beatSource.SetParameterToInstance("PlayElement", 11);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.EarthAir] = runesongAtEndOfTurn.chordsPresent[ChordTypes.EarthAir] + 1;
                    }

                    // Three elements

                    if (elementsInColumn.Contains(Elements.Fire) &&
                        elementsInColumn.Contains(Elements.Ice) &&
                        elementsInColumn.Contains(Elements.Earth))
                    {
                        beatSource.SetParameterToInstance("PlayElement", 12);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.FireIceEarth] = runesongAtEndOfTurn.chordsPresent[ChordTypes.FireIceEarth] + 1;
                    }

                    if (elementsInColumn.Contains(Elements.Fire) &&
                        elementsInColumn.Contains(Elements.Ice) &&
                        elementsInColumn.Contains(Elements.Air))
                    {
                        beatSource.SetParameterToInstance("PlayElement", 13);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.FireIceAir] = runesongAtEndOfTurn.chordsPresent[ChordTypes.FireIceAir] + 1;
                    }

                    if (elementsInColumn.Contains(Elements.Ice) &&
                        elementsInColumn.Contains(Elements.Earth) &&
                        elementsInColumn.Contains(Elements.Air))
                    {
                        beatSource.SetParameterToInstance("PlayElement", 15);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.IceEarthAir] = runesongAtEndOfTurn.chordsPresent[ChordTypes.IceEarthAir] + 1;
                    }

                    if (elementsInColumn.Contains(Elements.Fire) &&
                        elementsInColumn.Contains(Elements.Ice) &&
                        elementsInColumn.Contains(Elements.Earth) &&
                        elementsInColumn.Contains(Elements.Air))
                    {
                        beatSource.SetParameterToInstance("PlayElement", 16);
                        runesongAtEndOfTurn.chordsPresent[ChordTypes.FireIceEarthAir] = runesongAtEndOfTurn.chordsPresent[ChordTypes.FireIceEarthAir] + 1;
                    }
                }
            }
        }
    }

   
    private void OnDestroy()
    {
        stopPlayingRunesong.RemoveAllListeners();
    }

}
