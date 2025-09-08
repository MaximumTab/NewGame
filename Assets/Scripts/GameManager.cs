using System;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public int Lives = 3;
    public int AllEnemyCount;
    public int CurEnemyCount = 0;
    [SerializeField] private TMP_Text LivesDisplay;
    [SerializeField] private TMP_Text CounterDisplay;
    private Animator ContrAnim;
    private static readonly int Lose = Animator.StringToHash("Lose");
    private static readonly int Win = Animator.StringToHash("Win");


    private void Start()
    {
        Time.timeScale = 1;
        ContrAnim=gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Lives <= 0)
        {
            Debug.Log("You Failed");
            ContrAnim.SetBool(Lose,true);
            //Time.timeScale = 0;
        }else if (IsWin())
        {
            Debug.Log("You Win");
            ContrAnim.SetBool(Win,true);
        }

        LivesDisplay.text = ""+Lives;
        CounterDisplay.text = CurEnemyCount + "/" + AllEnemyCount;
    }

    public void SetCurEnemCount()
    {
        CurEnemyCount++;
    }

    public bool IsWin()
    {
        if (CurEnemyCount == AllEnemyCount)
        {
            EnemyBehaviour[] allEnems = FindObjectsByType<EnemyBehaviour>(FindObjectsInactive.Include,FindObjectsSortMode.None);
            if (allEnems.Length==0)
            {
                return true;
            }
        }

        return false;

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
