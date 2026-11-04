using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Main menu: Start loads the game scene, Exit quits.
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private int gameSceneIndex = 1;
    [SerializeField] private float clickDelay = 0.2f;

    private bool busy;
    [SerializeField] private TMP_Text bestText;

    private void Start()
    {
        if (bestText != null)
            bestText.text = $"BEST  {PlayerPrefs.GetInt("HighScore", 0)}";
    }
    
    public void OnStartClicked()
    {
        Run(() =>
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
            }
            else
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(gameSceneIndex);
            }
        });
    }

    public void OnExitClicked()
    {
        Run(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }

    // Plays the click sound, waits a moment so it can be heard, then does the action.
    private void Run(Action action)
    {
        if (busy) return;
        busy = true;
        StartCoroutine(Routine(action));
    }

    private IEnumerator Routine(Action action)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUiClick();
        yield return new WaitForSecondsRealtime(clickDelay);
        action();
    }
}