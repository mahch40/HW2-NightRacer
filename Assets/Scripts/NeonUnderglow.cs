using UnityEngine;

// Neon light under the car that gets stronger and wider as the game speeds up.
public class NeonUnderglow : MonoBehaviour
{
    [SerializeField] private Light glowLight;
    [SerializeField] private Renderer[] glowRenderers;   // the emissive strips

    [Header("Color")]
    [SerializeField] private Color neonColor = new Color(0f, 0.9f, 1f);

    [Header("Speed mapping")]
    [SerializeField] private float minSpeed = 20f;
    [SerializeField] private float maxSpeed = 55f;

    [Header("Light")]
    [SerializeField] private float minIntensity = 4f;
    [SerializeField] private float maxIntensity = 14f;
    [SerializeField] private float minRange = 5f;
    [SerializeField] private float maxRange = 8f;

    [Header("Emissive strips (HDR multiplier)")]
    [SerializeField] private float minEmission = 1.5f;
    [SerializeField] private float maxEmission = 5f;

    [SerializeField] private float smoothing = 4f;

    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    private Material[] materials;
    private float level;   // 0 = slow, 1 = top speed

    private void Awake()
    {
        // material gives each strip its own instance, so the shared asset stays untouched.
        materials = new Material[glowRenderers.Length];
        for (int i = 0; i < glowRenderers.Length; i++)
            materials[i] = glowRenderers[i].material;
    }

    private void Start()
    {
        if (glowLight != null) glowLight.color = neonColor;
        Apply();
    }

    private void Update()
    {
        float target = Mathf.InverseLerp(minSpeed, maxSpeed, GameSpeed.Current);
        level = Mathf.Lerp(level, target, smoothing * Time.deltaTime);
        Apply();
    }

    private void Apply()
    {
        if (glowLight != null)
        {
            glowLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, level);
            glowLight.range = Mathf.Lerp(minRange, maxRange, level);
        }

        Color emission = neonColor * Mathf.Lerp(minEmission, maxEmission, level);
        foreach (Material m in materials)
            m.SetColor(EmissionColorId, emission);
    }
}