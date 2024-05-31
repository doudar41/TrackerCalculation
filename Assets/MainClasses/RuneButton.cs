
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(Toggle))]
public class RuneButton : MonoBehaviour
{
    Toggle runeToggle;
    public UnityEvent<Elements,int, bool> sendRune;
    public int runePosition;
    public Elements element;

    void Awake()
    {
        runeToggle = GetComponent<Toggle>();
        runeToggle.onValueChanged.AddListener(runeClick);

        // This can be used for automatic element setup
        // In case of visual elements it possible that element can send to tracker any information and this script not needed at all

        string rowEffect =  gameObject.transform.tag;
        switch (rowEffect)
        {
            case "Neutral":
                element = Elements.Neutral;
                break;
            case "Fire":
                element = Elements.Fire;
                break;
            case "Ice":
                element = Elements.Ice;
                break;
            case "Earth":
                element = Elements.Earth;
                break;
            case "Air":
                element = Elements.Air;
                break;
        }
    }

    public void runeClick(bool onOff)
    {
        sendRune.Invoke(element, runePosition, onOff);
    }


    private void OnDestroy()
    {
        sendRune.RemoveAllListeners();
    }
}
