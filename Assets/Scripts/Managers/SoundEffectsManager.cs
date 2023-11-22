using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager Instance;

    [SerializeField] private OptionsMenuUI optionsMenuUI;
    [SerializeField] private AudioSource[] audioSources;

    [SerializeField] private AudioSource playerWinSound;
    [SerializeField] private AudioSource playerLoseSound;
    [SerializeField] private AudioSource tireScreechSound;
    [SerializeField] private AudioSource bounsSound;
    [SerializeField] private AudioSource countDownSound;
    [SerializeField] private AudioSource nitroSound;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.SoundEffectsVolume.ToString()))
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.SoundEffectsVolume.ToString(), 1);
            LoadSoundEffectVolume();
        }
        else
        {
            LoadSoundEffectVolume();
        }

        CarSelectionManager.Get().GetCurrentCarController().OnCarDrifted += SoundManager_OnCarDrifted;

        countDownSound.Play();
    }


    private void SoundManager_OnCarDrifted(object sender, System.EventArgs e)
    {
        bounsSound.Play();
        tireScreechSound.Play();
    }

    public void PlayNitroSound() => nitroSound.Play();
    public void StopNitroSound() => nitroSound.Stop();

    public void PlayWinSound() => playerWinSound.Play();

    public void PlayLoseSound() => playerLoseSound.Play();

    public void ChangeSoundEffectVolume()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.volume = optionsMenuUI.GetSoundEffectVolume();
        }
        SaveSoundEffectVolume();
    }

    private void LoadSoundEffectVolume()
    {
        optionsMenuUI.SetSoundEffectVolume(PlayerPrefs.GetFloat(PlayerPrefsKeys.SoundEffectsVolume.ToString()));
    }

    private void SaveSoundEffectVolume()
    {
        PlayerPrefs.SetFloat(PlayerPrefsKeys.SoundEffectsVolume.ToString(), optionsMenuUI.GetSoundEffectVolume());
    }

    public float GetAudioSourceVolume()
    {
        return optionsMenuUI.GetSoundEffectVolume();
    } 

    public static SoundManager Get()
    {
        return Instance;
    }
}
