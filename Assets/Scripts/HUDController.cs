using TMPro;
using UnityEngine;
using UnityEngine.UI;

// In-game HUD: speed, fuel bar, lives, score and high score.
// It only reads data and listens to events, so it can be removed without breaking the game.
public class HUDController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerFuel playerFuel;

    [Header("Texts")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private float kmhPerSpeedUnit = 4f;

    [Header("Fuel bar")]
    [SerializeField] private Slider fuelSlider;
    [SerializeField] private Image fuelFill;
    [SerializeField] private Color fuelNormalColor = new Color32(0x00, 0xE5, 0xFF, 255);
    [SerializeField] private Color fuelLowColor = new Color32(0xFF, 0x3B, 0x30, 255);
    [SerializeField] private Color fuelLowFlashColor = new Color32(0xFF, 0xB0, 0x20, 255);

    [Header("Lives")]
    [SerializeField] private Image[] lifeIcons;
    [SerializeField] private Color lifeOnColor = new Color32(0xFF, 0x2B, 0xD6, 255);
    [SerializeField] private Color lifeOffColor = new Color32(0x3A, 0x2A, 0x44, 255);

    private void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.OnLivesChanged += UpdateLives;
            UpdateLives(playerHealth.Lives);
        }

        if (playerFuel != null)
        {
            playerFuel.OnFuelChanged += UpdateFuel;
            UpdateFuel(playerFuel.Normalized);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateScore;
            UpdateScore(GameManager.Instance.Score);
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null) playerHealth.OnLivesChanged -= UpdateLives;
        if (playerFuel != null) playerFuel.OnFuelChanged -= UpdateFuel;
        if (GameManager.Instance != null) GameManager.Instance.OnScoreChanged -= UpdateScore;
    }

    private void Update()
    {
        float kmh = GameSpeed.Current * kmhPerSpeedUnit;
        speedText.text = $"{kmh:0}<size=40%> km/h</size>";

        if (playerFuel != null && fuelFill != null)
        {
            if (playerFuel.IsLow)
            {
                float pulse = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * 8f);
                fuelFill.color = Color.Lerp(fuelLowColor, fuelLowFlashColor, pulse);
            }
            else
            {
                fuelFill.color = fuelNormalColor;
            }
        }
    }

    private void UpdateLives(int lives)
    {
        for (int i = 0; i < lifeIcons.Length; i++)
            lifeIcons[i].color = i < lives ? lifeOnColor : lifeOffColor;
    }

    private void UpdateFuel(float normalized)
    {
        fuelSlider.value = normalized;
    }

    private void UpdateScore(int score)
    {
        scoreText.text = score.ToString();

        // The best score climbs live once the player passes the saved record.
        int best = Mathf.Max(GameManager.Instance.HighScore, score);
        bestText.text = $"BEST {best}";
    }
}