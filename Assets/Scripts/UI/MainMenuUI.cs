using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button aboutButton;
    [SerializeField] private Button rewardsButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private GameObject playMenuUI;
    [SerializeField] private GameObject aboutMenuUI;
    [SerializeField] private GameObject rewardMenuUI;

    private void OnEnable()
    {
        playButton.Select();
    }

    private void Awake()
    {

        playButton.onClick.AddListener(() =>
        {
            ShowPlayMenuUI();
        });

        aboutButton.onClick.AddListener(() =>
        {
            aboutMenuUI.SetActive(true);
            gameObject.SetActive(false);
        });

        rewardsButton.onClick.AddListener(() =>
        {
            rewardMenuUI.SetActive(true);
            gameObject.SetActive(false);
        });

        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        Time.timeScale = 1;
    }

    private void Start()
    {
        GameInputManager.Get().OnPauseAction += MainMenuUI_OnPauseAction;
    }

    private void MainMenuUI_OnPauseAction(object sender, System.EventArgs e)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }

    private void ShowPlayMenuUI()
    {
        gameObject.SetActive(false);
        playMenuUI.SetActive(true);
    }

    private void OnDisable()
    {
        GameInputManager.Get().OnPauseAction -= MainMenuUI_OnPauseAction;
    }

}
