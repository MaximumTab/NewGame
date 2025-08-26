using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int Lives = 3;
    public int AllEnemyCount;
    public int CurEnemyCount = 0;

    private void Start()
    {
        CurEnemyCount = 0;
        AllEnemyCount = 0;
    }

    private void Update()
    {
        if (Lives <= 0)
        {
            Debug.Log("You Failed");
            Time.timeScale = 0;
        }
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
}
