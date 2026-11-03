using System.Collections;
using UnityEngine;

// A street lamp that is sometimes dead and sometimes flickers.
// OnEnable runs every time the pooled road segment is reused, so lamps re-roll each time.
public class StreetLamp : MonoBehaviour
{
    [SerializeField] private Light lampLight;
    [SerializeField] private Renderer bulb;

    [SerializeField, Range(0f, 1f)] private float offChance = 0.7f;
    [SerializeField, Range(0f, 1f)] private float flickerChance = 0.4f;

    private void OnEnable()
    {
        float roll = Random.value;

        if (roll < offChance)
        {
            SetLit(false);
        }
        else
        {
            SetLit(true);
            if (roll < offChance + flickerChance)
                StartCoroutine(Flicker());
        }
    }

    private void SetLit(bool lit)
    {
        if (lampLight != null) lampLight.enabled = lit;
        if (bulb != null) bulb.enabled = lit;
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 3f));

            SetLit(false);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
            SetLit(true);

            // Sometimes a quick second blink.
            if (Random.value < 0.5f)
            {
                yield return new WaitForSeconds(0.08f);
                SetLit(false);
                yield return new WaitForSeconds(0.06f);
                SetLit(true);
            }
        }
    }
}