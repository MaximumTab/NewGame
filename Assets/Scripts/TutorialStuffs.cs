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
    public Light LightChange;
    private bool Cutscene;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Tuting());
    }

    IEnumerator Tuting()
    {
        Cutscene = true;
        foreach (Phases phase in Tuts)
        {
            float tempTime = Time.timeScale;
            yield return new WaitForSeconds(phase.timeToThis);
            phase.TextToChange.transform.parent.gameObject.SetActive(true);
            phase.TextToChange.text = phase.TutText;
            LightChange.intensity = phase.lightIntensity;
            activeTriggers = phase.eventTrigger;
            Time.timeScale = phase.TimeScaling;
            yield return new WaitUntil(() => trigger);
            trigger = false;
            phase.TextToChange.transform.parent.gameObject.SetActive(false);
            LightChange.intensity = 1;
            Time.timeScale = tempTime;
            yield return null;
        }

        Cutscene = false;
        yield return null;
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
        if (Input.anyKey&&Cutscene)
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
        public float lightIntensity;
    }
    public enum triggers 
    {
        TowerPlaced,
        GathererPlaced,
        Clicked
    }
}
