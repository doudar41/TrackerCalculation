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
            foreach (Elements element in System.Enum.GetValues(typeof(Elements)))
            {
                if (trackerRowList[element].GetRunePositionFromRow(currentColumn+1))
                {
                    // trackerRowList contain sequenceRow of e element. element is enum Elements 
                    
                    elementsInColumn.Add(element); 
                    if (!runesongAtEndOfTurn.savedRunesongPattern.TryAdd(element, trackerRowList[element]))
                    {

                        runesongAtEndOfTurn.savedRunesongPattern[element] = trackerRowList[element];
                        runesongAtEndOfTurn.occupiedRuneSlotsinRow[element] = runesongAtEndOfTurn.occupiedRuneSlotsinRow[element] + 1;
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


   
    private void OnDestroy()
    {
        stopPlayingRunesong.RemoveAllListeners();
    }

}
