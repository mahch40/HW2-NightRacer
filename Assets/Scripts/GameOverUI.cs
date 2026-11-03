using System;
using System.Collections;
using TMPro;
using UnityEngine;

// Game over screen: fades in shortly after the run ends and shows the final score.
public class GameOverUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private CanvasGroup panelGroup;

    [Header("Texts")]
    [SerializeField] private TMP_Text reasonText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestText;

    [Header("References")]
    [SerializeField] private PlayerFuel playerFuel;

    [Header("Timing")]
    [SerializeField] private float showDelay = 0.8f;
    [SerializeField] private float fadeTime = 0.4f;
    [SerializeField] private float clickDelay = 0.15f;

    private bool busy;
    private Coroutine showRoutine;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameOverUI needs a GameManager in the scene.");
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
        busy = false;

        if (state == GameState.GameOver)
        {
            showRoutine = StartCoroutine(ShowRoutine());
        }
        else
        {
            if (showRoutine != null) StopCoroutine(showRoutine);
            panel.SetActive(false);
        }
    }

    private IEnumerator ShowRoutine()
    {
        yield return new WaitForSecondsRealtime(showDelay);

        int score = GameManager.Instance.Score;
        int best = Mathf.Max(GameManager.Instance.HighScore, score);

        reasonText.text = (playerFuel != null && playerFuel.IsEmpty) ? "OUT OF FUEL" : "CRASHED";
        scoreText.text = $"SCORE  {score}";
        bestText.text = $"BEST  {best}";

        panelGroup.alpha = 0f;
        panel.SetActive(true);

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            panelGroup.alpha = Mathf.Clamp01(t / fadeTime);
            yield return null;
        }
        panelGroup.alpha = 1f;
    }

    public void OnRestartClicked() { Run(() => GameManager.Instance.Restart()); }
    public void OnMainMenuClicked() { Run(() => GameManager.Instance.GoToMainMenu()); }

    private void Run(Action action)
    {
        if (busy) return;
        busy = true;
        StartCoroutine(ClickRoutine(action));
    }

    private IEnumerator ClickRoutine(Action action)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUiClick();
        yield return new WaitForSecondsRealtime(clickDelay);
        action();
    }
}