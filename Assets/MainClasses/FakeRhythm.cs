using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.Events;

public class FakeRhythm : MonoBehaviour
{
    string counter = "0";
    int counterInt = 0;
    [SerializeField] float delay =0.5f;

    public UnityEvent<string> counterEvent;
    // Start is called before the first frame update
    void Start()
    {
        counter = counterInt.ToString();
        StartCoroutine(FakeRhythmCounter());
    }

    IEnumerator FakeRhythmCounter()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            counterEvent.Invoke(counter);
            counterInt++;
            if (counterInt > 15) { counterInt = 0; }
            counter = counterInt.ToString();

        }

    }
}
