using System;
using System.Collections;
using UnityEngine;

// Pause menu: the panel is visible only while the game is Paused.
// The buttons call GameManager (resume, restart, main menu).
public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private float clickDelay = 0.15f;

    private bool busy;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("PauseMenuUI needs a GameManager in the scene.");
            return;
        }

        GameManager.Instance.OnStateChanged += HandleState;
        HandleState(GameManager.Instance.State);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= HandleState;
    }

    private void HandleState(GameState state)
    {
        panel.SetActive(state == GameState.Paused);
        busy = false;
    }

    public void OnResumeClicked() { Run(() => GameManager.Instance.Resume()); }
    public void OnRestartClicked() { Run(() => GameManager.Instance.Restart()); }
    public void OnMainMenuClicked() { Run(() => GameManager.Instance.GoToMainMenu()); }

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