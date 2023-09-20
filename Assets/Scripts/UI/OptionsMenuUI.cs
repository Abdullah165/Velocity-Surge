using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundEffectSlider;
    [SerializeField] private Button backButton;

    [SerializeField] private GameObject settingMenuUI;

    private void OnEnable()
    {
        musicSlider.Select();
    }

    void Start()
    {
        GameInputManager.Get().OnPauseAction += OptionsMenuUI_OnPauseAction;
        Hide();
    }

    private void OptionsMenuUI_OnPauseAction(object sender, System.EventArgs e)
    {
        Hide();
    }

    public void Back()
    {
        Hide();
        settingMenuUI.SetActive(true);
    }

    public void ChangeMusicVolume()
    {
        MusicManager.Get().ChangeMusicVolume();
    }

    public void ChangeSoundEffectVolume()
    {
        SoundManager.Get().ChangeSoundEffectVolume();
    }

    public float GetSoundEffectVolume()
    {
        return soundEffectSlider.value;
    }

    public void SetSoundEffectVolume(float soundEffectsVolume)
    {
        soundEffectSlider.value = soundEffectsVolume;
    }

    public float GetMusicVolume()
    {
        return musicSlider.value;
    }

    public void SetMusicVolume(float musicVolume)
    {
        musicSlider.value = musicVolume;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
