using UnityEngine;
using UnityEngine.UI;

public class PlayMenuUI : MonoBehaviour
{
    [SerializeField] private Button raceButton;
    [SerializeField] private Button timerButton;
    [SerializeField] private Button backButton;

    [SerializeField] private GameObject raceMenuUI;
    [SerializeField] private GameObject timerMenuUI;

    private void OnEnable()
    {
        raceButton.Select();
        GameInputManager.Get().OnPauseAction += PlayMenuUI_OnPauseAction;
    }

    private void Awake()
    {
        Hide();

        raceButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.SelectedTimer.ToString(), 0);
            ShowRaceMenuUI();
        });

        timerButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.SelectedTimer.ToString(), 1);
            ShowTimerMenuUI();
        });

        backButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            SceneLoadingManager.Load(SceneLoadingManager.Scene.MainMenuScene);
        });

    }

    private void PlayMenuUI_OnPauseAction(object sender, System.EventArgs e)
    {
        SceneLoadingManager.Load(SceneLoadingManager.Scene.MainMenuScene);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowRaceMenuUI()
    {
        raceMenuUI.SetActive(true);
        gameObject.SetActive(false);
    }

    private void ShowTimerMenuUI()
    {
        timerMenuUI.SetActive(true);
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        GameInputManager.Get().OnPauseAction -= PlayMenuUI_OnPauseAction;
    }
}
