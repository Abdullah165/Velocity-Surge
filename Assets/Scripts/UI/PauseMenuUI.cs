using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button recoverButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private GameObject optionsMenuUI;

    private CarController carController;
    //private Season selectedSeason;

    private void OnEnable()
    {
        resumeButton.Select();
    }
    private void Awake()
    {
        carController = CarSelectionManager.Get().GetCurrentCarController();

        //int season = PlayerPrefs.GetInt(PlayerPrefsKeys.SelectedSeason.ToString());
        //selectedSeason = (Season)season;

        resumeButton.onClick.AddListener(() =>
        {
            TogglePauseGame();
        });

        recoverButton.onClick.AddListener(() =>
        {
            carController.Recover();
            TogglePauseGame();
        });

        restartButton.onClick.AddListener(() =>
        {
            SceneLoadingManager.Load();
        });

        optionsButton.onClick.AddListener(() =>
        {
            Hide();
            optionsMenuUI.SetActive(true);
        });

        quitButton.onClick.AddListener(() =>
        {
            SceneLoadingManager.Load(SceneLoadingManager.Scene.MainMenuScene);
        });
    }

    void Start()
    {
        Hide();
        GameInputManager.Get().OnPauseAction += SettingMenuUI_OnPauseAction;
    }

    private void SettingMenuUI_OnPauseAction(object sender, System.EventArgs e)
    {
        TogglePauseGame();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void TogglePauseGame()
    {
        gameObject.SetActive(!gameObject.activeSelf);

        if (!gameObject.activeSelf)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }
}
