using System;
using System.Collections.Generic;
using UnityEngine;
public static class GameManager
{
    public static List<Incursion> Incursions;
    public class InGameManager : MonoBehaviour
    {
        public int AllEnemies=0;
        public int ExitedEnemies=0;
        public int PlayerLives=3;
    }

    private static InGameManager InsideTheGame;

    public static void LoseLife(int Lives)
    {
        InsideTheGame.PlayerLives -= Lives;
    }

    public static void AddMaxEnem(int EnemyCount)
    {
        InsideTheGame.AllEnemies += EnemyCount;
    }

    public static void AddRemovedEnem(int EnemyCount)
    {
        InsideTheGame.ExitedEnemies += EnemyCount;
    }

    public static void MakingIGM()
    {
        if (!InsideTheGame)
        {
            GameObject ITG = new GameObject("GameManager");
            InsideTheGame=ITG.AddComponent<InGameManager>();
        }
        Incursions = new List<Incursion>();
        InsideTheGame.AllEnemies = 0;
    }
}
