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

    private void OnEnable()
    {
        resumeButton.Select();
    }
    private void Awake()
    {
        resumeButton.onClick.AddListener(() =>
        {
            TogglePauseGame();
        });

        recoverButton.onClick.AddListener(() =>
        {
            CarSelectionManager.Get().GetCurrentCarController().Recover();
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
        GameInputManager.Get().OnPauseAction += PauseMenuUI_OnPauseAction;
    }

    private void PauseMenuUI_OnPauseAction(object sender, System.EventArgs e)
    {
        TogglePauseGame();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    public void TogglePauseGame()
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
