using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialStuffs : MonoBehaviour
{
    public List<Phases> Tuts;
    public static bool trigger;
    public static triggers activeTriggers;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Tuting());
    }

    IEnumerator Tuting()
    {
        foreach (Phases phase in Tuts)
        {
            float tempTime = Time.timeScale;
            yield return new WaitForSeconds(phase.timeToThis);
            phase.TextToChange.enabled = true;
            phase.TextToChange.text = phase.TutText;
            activeTriggers = phase.eventTrigger;
            Time.timeScale = phase.TimeScaling;
            yield return new WaitUntil(() => trigger);
            trigger = false;
            phase.TextToChange.enabled = false;
            Time.timeScale = tempTime;
        }
    }

    public static void changeTrigger(triggers trig)
    {
        if (trig == activeTriggers)
        {
            trigger = true;
        }
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            changeTrigger(triggers.Clicked);
        }
    }

    [Serializable]
    public struct Phases
    {
        public TMP_Text TextToChange;
        public string TutText;
        public float timeToThis;
        public float TimeScaling;
        public triggers eventTrigger;
    }
    public enum triggers 
    {
        TowerPlaced,
        GathererPlaced,
        Clicked
    }
}
