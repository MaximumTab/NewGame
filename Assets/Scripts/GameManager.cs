using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int Lives = 3;
    public int AllEnemyCount;
    public int CurEnemyCount = 0;
    [SerializeField] private TMP_Text LivesDisplay;
    [SerializeField] private TMP_Text CounterDisplay;


    private void Start()
    {
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (Lives <= 0)
        {
            Debug.Log("You Failed");
            Time.timeScale = 0;
        }
        
        LivesDisplay.text = ""+Lives;
        CounterDisplay.text = CurEnemyCount + "/" + AllEnemyCount;
    }

    public void SetCurEnemCount()
    {
        CurEnemyCount++;
    }

    public void SetEnemyCount(int Count)
    {
        AllEnemyCount += Count;
    }

    public void LoseALife(int lives)
    {
        Lives -= lives;
    }

    public void ToggleSpeed()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 2;
        }
        else if (Time.timeScale == 2)
        {
            Time.timeScale = 1;
        }
    }
}
