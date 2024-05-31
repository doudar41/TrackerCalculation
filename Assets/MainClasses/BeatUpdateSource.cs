using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class BeatUpdateSource : MonoBehaviour
{
    class TimelineInfo
    {
        public int CurrentMusicBar = 0;
        public FMOD.StringWrapper LastMarker = new FMOD.StringWrapper();
        public FMOD.Studio.EVENT_CALLBACK_TYPE eventType;
        public FMOD.Studio.EventInstance commandEvent;
        public string path;
    }

    public static BeatUpdateSource beatSource;

    public FMODUnity.EventReference EventName;
    public UnityEvent OnBar;

    TimelineInfo timelineInfo;
    GCHandle timelineHandle;
    string tempMarkerName;
    int currentBeat = -1;

    FMOD.Studio.EVENT_CALLBACK beatCallback;
    FMOD.Studio.EventInstance musicInstance;

    public void InitializeMusicEvent(FMODUnity.EventReference musicEvent)
    {
        musicInstance.release();
        timelineInfo = new TimelineInfo();
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
        musicInstance = FMODUnity.RuntimeManager.CreateInstance(musicEvent);
        timelineHandle = GCHandle.Alloc(timelineInfo);
        musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));
        musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.ALL);
    }

    private void Awake()
    {
        if (beatSource != null && beatSource != this)
        {
            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(this);
            beatSource = this;
        }
    }

    private void Start()
    {

        GamesMainEvents.LoseBattle += StopBattleMusic;
        StartBattleMusic(EventName);
    }

    public void StartBattleMusic(FMODUnity.EventReference musicEvent)
    {
        InitializeMusicEvent(musicEvent);
        musicInstance.start();
        musicInstance.setParameterByName("PlayControl", 0);
    }

    public void SetFMODInstance(FMOD.Studio.EventInstance instance)
    {
        musicInstance = instance;
    }

    public void SetParameterToInstance(string name, int value)
    {

        musicInstance.setParameterByName(name, value);
    }

    public void StopBattleMusic()
    {
        musicInstance.setParameterByName("PlayControl", 1);
    }

    private void Update()
    {
        musicInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE result);

        if (result == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            if (timelineInfo.CurrentMusicBar != currentBeat)
            {
                currentBeat = timelineInfo.CurrentMusicBar;
                OnBar.Invoke();
            }

            string switchName = (string)timelineInfo.LastMarker;

            if(tempMarkerName != switchName)
            {
                tempMarkerName = switchName;
                GamesMainEvents.UpdateOnBeat(switchName);
            }
        }
    }


    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            // Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.START_EVENT_COMMAND:
                    {
                        timelineInfo.eventType = FMOD.Studio.EVENT_CALLBACK_TYPE.START_EVENT_COMMAND;

                        FMOD.Studio.EventInstance inst = new FMOD.Studio.EventInstance(parameterPtr);
                        inst.getDescription(out FMOD.Studio.EventDescription desc);
                        //inst.setParameterByName("LoudnessLocal", 0);
                        desc.getPath(out string path);
                        timelineInfo.commandEvent = inst;
                        timelineInfo.path = path;
                        //Debug.Log("START_EVEN_COMMAND: Playing " + path);
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.SOUND_STOPPED:
                    {
                        FMOD.Studio.EventInstance inst = new FMOD.Studio.EventInstance(parameterPtr);
                        inst.getDescription(out FMOD.Studio.EventDescription desc);
                        //Debug.Log("STOP_EVENT_COMMAND: Playing " + desc);
                        timelineInfo.commandEvent = inst;

                    }
                    break;

                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.CurrentMusicBar = parameter.bar;
                        break;
                    }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.LastMarker = parameter.name;
                        break;
                    }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                    {
                        // Now the event has been destroyed, unpin the timeline memory so it can be garbage collected
                        timelineHandle.Free();
                        break;
                    }
            }
        }
        return FMOD.RESULT.OK;
    }



    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
    }
}
