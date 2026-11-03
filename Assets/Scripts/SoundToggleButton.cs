using TMPro;
using UnityEngine;

// A button that switches music or sound effects on and off, and shows the state.
public class SoundToggleButton : MonoBehaviour
{
    public enum Kind { Music, SoundEffects }

    [SerializeField] private Kind kind;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Color onColor = new Color32(0x00, 0xE5, 0xFF, 255);
    [SerializeField] private Color offColor = new Color32(0x7A, 0x83, 0x99, 255);

    private void OnEnable() { Refresh(); }
    private void Start() { Refresh(); }

    public void OnClicked()
    {
        AudioManager audio = AudioManager.Instance;
        if (audio == null) return;

        if (kind == Kind.Music) audio.SetMusicEnabled(!audio.MusicEnabled);
        else audio.SetSfxEnabled(!audio.SfxEnabled);

        audio.PlayUiClick();   
        Refresh();
    }

    private void Refresh()
    {
        AudioManager audio = AudioManager.Instance;
        if (audio == null || label == null) return;

        bool on = kind == Kind.Music ? audio.MusicEnabled : audio.SfxEnabled;
        string name = kind == Kind.Music ? "MUSIC" : "SOUND FX";
        label.text = $"{name}: {(on ? "ON" : "OFF")}";
        label.color = on ? onColor : offColor;
    }
}