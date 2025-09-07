using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class AudioManager : MonoBehaviour
{
    private List<EventInstance> eventInstances;

    private List<StudioEventEmitter> eventEmitters;
    private EventInstance musicEventInstance;


    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("found more than one AudioManager in the scene");
        }
        instance = this;
        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();
    }

    private void Start()
    {
        InitializeMusic(FmodEvents.instance.music);
    }
    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventinstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventinstance);
        return eventinstance;



    }
    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }

}



