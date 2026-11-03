using System;
using System.Collections;
using UnityEngine;

// Lives, damage, brief invincibility and hit feedback (red blink + camera shake).
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxLives = 3;
    [SerializeField] private float invincibleTime = 1.5f;
    [SerializeField] private float blinkInterval = 0.1f;
    [SerializeField] private Color hitColor = new Color(1f, 0.15f, 0.15f);

    [Header("Camera shake")]
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private float shakeDuration = 0.35f;
    [SerializeField] private float shakeMagnitude = 0.25f;

    public event Action<int> OnLivesChanged;
    public event Action OnHit;
    public event Action OnDied;

    public int Lives { get; private set; }
    public int MaxLives => maxLives;
    public bool IsDead => Lives <= 0;
    public bool IsInvincible { get; private set; }

    private MeshRenderer[] renderers;
    private MaterialPropertyBlock block;
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        Lives = maxLives;
        renderers = GetComponentsInChildren<MeshRenderer>();
        block = new MaterialPropertyBlock();

        if (cameraFollow == null && Camera.main != null)
            cameraFollow = Camera.main.GetComponent<CameraFollow>();
    }

    public void TakeHit()
    {
        if (IsDead || IsInvincible) return;

        Lives--;
        Debug.Log($"Lives: {Lives}/{maxLives}");  
        OnLivesChanged?.Invoke(Lives);
        OnHit?.Invoke();

        if (cameraFollow != null)
            cameraFollow.Shake(shakeDuration, shakeMagnitude);

        if (Lives <= 0) Die();
        else StartCoroutine(InvincibilityRoutine());
    }

    private void Die()
    {
        SetTint(true);
        GameSpeed.Current = 0f;    

        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = false;

        Debug.Log("Game over");
        OnDied?.Invoke();
    }

    private IEnumerator InvincibilityRoutine()
    {
        IsInvincible = true;

        float elapsed = 0f;
        bool red = true;
        while (elapsed < invincibleTime)
        {
            SetTint(red);
            red = !red;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        SetTint(false);
        IsInvincible = false;
    }

    private void SetTint(bool tinted)
    {
        foreach (MeshRenderer r in renderers)
        {
            if (tinted)
            {
                r.GetPropertyBlock(block);
                block.SetColor(BaseColorId, hitColor);
                r.SetPropertyBlock(block);
            }
            else
            {
                r.SetPropertyBlock(null);   // back to the original look
            }
        }
    }
}