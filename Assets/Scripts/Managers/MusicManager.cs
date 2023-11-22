using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager Instance;

    private AudioSource audioSource;

    [SerializeField] private OptionsMenuUI optionsMenuUI;

    private void Awake()
    {
        Instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.MusicVolume.ToString()))
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.MusicVolume.ToString(), 1);
            LoadMusicVolume();
        }
        else
        {
            LoadMusicVolume();
        }

    }

    public void ChangeMusicVolume()
    {
        audioSource.volume = optionsMenuUI.GetMusicVolume();
        SaveMusicVolume();
    }

    private void LoadMusicVolume()
    {
        optionsMenuUI.SetMusicVolume(PlayerPrefs.GetFloat(PlayerPrefsKeys.MusicVolume.ToString()));
        audioSource.volume = optionsMenuUI.GetMusicVolume();
    }

    private void SaveMusicVolume()
    {
        PlayerPrefs.SetFloat(PlayerPrefsKeys.MusicVolume.ToString(), optionsMenuUI.GetMusicVolume());
    }

    public static MusicManager Get()
    {
        return Instance;
    }
}
