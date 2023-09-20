using UnityEngine;
using UnityEngine.UI;

public class AboutMenuUI : MonoBehaviour
{
    [SerializeField] private Button youtubeArabicButton;
    [SerializeField] private Button youtubeEnglishButton;
    [SerializeField] private Button githubButton;
    [SerializeField] private Button backButton;

    private void OnEnable()
    {
        youtubeArabicButton.Select();
    }

    private void Awake()
    {
        Hide();

        youtubeArabicButton.onClick.AddListener(() =>
        {
            Application.OpenURL("https://www.youtube.com/channel/UCQ2dvORH76mny930SaJbmnA");
        });
        
        youtubeEnglishButton.onClick.AddListener(() =>
        {
            Application.OpenURL("https://www.youtube.com/channel/UCrnKMmlzEjayfqVLZij9tAA");
        });


        githubButton.onClick.AddListener(() =>
        {
            Application.OpenURL("https://github.com/Abdullah165/Crazy_Fast.git");
        });

        backButton.onClick.AddListener(() =>
        {
            SceneLoadingManager.Load(SceneLoadingManager.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        GameInputManager.Get().OnPauseAction +=AboutMenuUI_OnPauseAction;
    }

    private void AboutMenuUI_OnPauseAction(object sender, System.EventArgs e)
    {
        SceneLoadingManager.Load(SceneLoadingManager.Scene.MainMenuScene);
    }

    private void Hide() => gameObject.SetActive(false);

    private void OnDisable()
    {
        GameInputManager.Get().OnPauseAction -= AboutMenuUI_OnPauseAction;
    }
}