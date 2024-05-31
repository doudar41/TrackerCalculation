using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowCount : MonoBehaviour
{
    private void Start()
    {
        RuneButton[] children = transform.GetComponentsInChildren<RuneButton>();
        int count = 0;
        foreach(RuneButton r in children)
        {
            r.runePosition = count*2;
            count++;
        }
    }
}
