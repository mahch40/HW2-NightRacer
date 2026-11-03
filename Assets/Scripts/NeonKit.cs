using UnityEngine;

// Colors the neon strips and the ground glow of one car.
// Uses property blocks, so the shared materials are never modified.
public class NeonKit : MonoBehaviour
{
    [SerializeField] private Color color = new Color32(0xFF, 0x2B, 0xD6, 255);
    [SerializeField] private float emissionIntensity = 3.5f;
    [SerializeField, Range(0f, 1f)] private float groundGlowAlpha = 0.45f;

    [SerializeField] private Renderer[] strips;
    [SerializeField] private Renderer groundGlow;

    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    private MaterialPropertyBlock block;

    private void Awake()
    {
        Apply();
    }

    private void OnValidate()
    {
        Apply();
    }

    private void Apply()
    {
        if (block == null) block = new MaterialPropertyBlock();

        if (strips != null)
        {
            foreach (Renderer strip in strips)
            {
                if (strip == null) continue;
                strip.GetPropertyBlock(block);
                block.SetColor(EmissionColorId, color * emissionIntensity);
                strip.SetPropertyBlock(block);
            }
        }

        if (groundGlow != null)
        {
            groundGlow.GetPropertyBlock(block);
            Color glow = color;
            glow.a = groundGlowAlpha;
            block.SetColor(BaseColorId, glow);
            groundGlow.SetPropertyBlock(block);
        }
    }
}