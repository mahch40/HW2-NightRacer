using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Main menu: Start loads the game scene, Exit quits.
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private int gameSceneIndex = 1;
    [SerializeField] private float clickDelay = 0.2f;

    private bool busy;

    public void OnStartClicked()
    {
        Run(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(gameSceneIndex);
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