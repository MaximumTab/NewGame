using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FmodEvents : MonoBehaviour
{
    [field: Header("CardDrag")]
    [field: SerializeField] public EventReference CardDrag { get; private set; }
    [field: SerializeField] public EventReference popupsfx { get; private set; }
    [field: SerializeField] public EventReference levelfinished { get; private set; }
    [field: SerializeField] public EventReference levellose { get; private set; }
    public static FmodEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("found more than one FmodEvents in the scene");
        }
        instance = this;
    }    
}
