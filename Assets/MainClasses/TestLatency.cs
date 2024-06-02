using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLatency : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    [SerializeField] float delay = 0.75f;
    BeatUpdateSource beatSource;
    bool t = false;

    private void Start()
    {
        beatSource = FindAnyObjectByType<BeatUpdateSource>();
        GamesMainEvents.UpdateOnBeat += UpdateOnBeat;
    }


    public void UpdateOnBeat(string marker)
    {


        int.TryParse(marker, out int countParse);
        //if (countParse % 2 == 0 && attackCount == -1) return;

        if (countParse % 2 != 0)
        {
            beatSource.SetParameterToInstance("PlayElement", 1);
            StartCoroutine(SetAudioDelayON());

            /*            if (t)
                        {
                            beatSource.SetParameterToInstance("PlayElementEnemy01", 0);
                            t = false;

                            StartCoroutine(SetAudioDelayON());

                        }
                        else
                        {
                            beatSource.SetParameterToInstance("PlayElementEnemy01", 1);
                            t = true;
                            StartCoroutine(SetAudioDelayON());
                        }*/
        }
        else
        {
            beatSource.SetParameterToInstance("PlayElementEnemy01", 0);
        }



    }


    public void ChangeDelay(float value)
    {
        delay = value;
    }

     IEnumerator SetAudioDelayON()
    {
        yield return new WaitForSeconds(delay);
        if(toggle.isOn) toggle.isOn = false;
        else toggle.isOn = true;
    }  
    
    IEnumerator SetAudioDelayOFF()
    {
        yield return new WaitForSeconds(delay);
        toggle.isOn = false;
    }
}
