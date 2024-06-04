using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrackerUI : MonoBehaviour
{
    [SerializeField] List<Toggle> neutralToggles = new List<Toggle>(); //Can be Visual Elements
    [SerializeField] List<Toggle> fireToggles = new List<Toggle>();
    [SerializeField] List<Toggle> iceToggles = new List<Toggle>();
    [SerializeField] List<Toggle> earthToggles = new List<Toggle>();
    [SerializeField] List<Toggle> airToggles = new List<Toggle>();
    [SerializeField] Sprite spritePlayhead, spriteNormal;
    [SerializeField] TextMeshProUGUI runepoolText;

    [SerializeField] Tracker tracker;

    
    public void runepoolShow(int amount)
    {
        runepoolText.text = amount.ToString();
    }


    public void NextColumn(int columnIndex)
    {
        StartCoroutine(DelayBeforePlayhead(columnIndex));
    }


    IEnumerator DelayBeforePlayhead(int columnIndex)
    {
        yield return new WaitForSeconds(GamesMainEvents.audioLatency);

        if (columnIndex > neutralToggles.Count || columnIndex < 0) yield return null;

        if (columnIndex > 7)
        {
            neutralToggles[neutralToggles.Count - 1].image.sprite = spriteNormal;
            neutralToggles[columnIndex - 1].image.sprite = spriteNormal;
            fireToggles[fireToggles.Count - 1].image.sprite = spriteNormal;
            fireToggles[columnIndex - 1].image.sprite = spriteNormal;
            iceToggles[neutralToggles.Count - 1].image.sprite = spriteNormal;
            iceToggles[columnIndex - 1].image.sprite = spriteNormal;
            earthToggles[neutralToggles.Count - 1].image.sprite = spriteNormal;
            earthToggles[columnIndex - 1].image.sprite = spriteNormal;
            airToggles[neutralToggles.Count - 1].image.sprite = spriteNormal;
            airToggles[columnIndex - 1].image.sprite = spriteNormal;

        }
        else
        {
            if (columnIndex == 0)
            {
                neutralToggles[columnIndex].image.sprite = spritePlayhead;
                fireToggles[columnIndex].image.sprite = spritePlayhead;
                iceToggles[columnIndex].image.sprite = spritePlayhead;
                earthToggles[columnIndex].image.sprite = spritePlayhead;
                airToggles[columnIndex].image.sprite = spritePlayhead;
            }
            else
            {
                //print(columnIndex);
                neutralToggles[columnIndex - 1].image.sprite = spriteNormal;
                neutralToggles[columnIndex].image.sprite = spritePlayhead;
                fireToggles[columnIndex - 1].image.sprite = spriteNormal;
                fireToggles[columnIndex].image.sprite = spritePlayhead;
                iceToggles[columnIndex - 1].image.sprite = spriteNormal;
                iceToggles[columnIndex].image.sprite = spritePlayhead;
                earthToggles[columnIndex - 1].image.sprite = spriteNormal;
                earthToggles[columnIndex].image.sprite = spritePlayhead;
                airToggles[columnIndex - 1].image.sprite = spriteNormal;
                airToggles[columnIndex].image.sprite = spritePlayhead;
            }

        }
    }
}
