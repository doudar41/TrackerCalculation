using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Player player;
    public int totalHealth = 100;
    public int currentHealth;
    public Slider _slider;

    void Start()
    {
        totalHealth = player.ReturnMaxHealth();
        currentHealth = player.ReturnCurrentHealth();
    }

    // Update is called once per frame
    // void Update()
    // {

    // }

    public int UpdateMaxHealth()
    {
        return 1;
    }

    public void UpdateHealthMeter()
    {
        int value;
        float floatValue;

        value = 100 * currentHealth/totalHealth;
        floatValue = value / 100f;
        _slider.value = floatValue;
    }

    public void ZOMGIGOTHIT()
    {
        _slider.value = 0.5f;
    }



}
