using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            Time.timeScale = 0.25f;
            StartCoroutine(LoseLVL());
        }else if (IsWin())
        {
            Debug.Log("You Win");
            ContrAnim.SetBool(Win,true);
            Time.timeScale = 1;
            StartCoroutine(WinLVL());
        }

        LivesDisplay.text = ""+Lives;
        CounterDisplay.text = CurEnemyCount + "/" + AllEnemyCount;
    }

    private IEnumerator WinLVL()
    {
        yield return new WaitForSeconds(2);
        FinishLVL();
    }

    private IEnumerator LoseLVL()
    {
        yield return new WaitForSeconds(1);
        FailedLVL();
    }

    private void FinishLVL()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        Levels.MarkLevelComplete(currentScene);
        Debug.Log("Level finished: " + currentScene);

        // Show debug of all levels
        Levels.DebugLevelStatus();

        SceneManager.LoadScene(1); // back to Level Select
    }

    private void FailedLVL()
    {
        SceneManager.LoadScene(1);
    }

    public void SetCurEnemCount()
    {
        CurEnemyCount++;
    }

    public bool IsWin()
    {
        if (CurEnemyCount == AllEnemyCount)
        {
            EnemyBehaviour[] enems =
                FindObjectsByType<EnemyBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (enems.Length==0)
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
