using UnityEngine;
using UnityEngine.UI;

public class PlaySelectionUI : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;

    [SerializeField] private GameObject playMenuUI;
    [SerializeField] private GameObject carSelectionUI;

    private Difficulty selectedDifficulty = Difficulty.Easy;
    private Season selectedSeason = Season.Winter;

    private void OnEnable()
    {
        nextButton.Select();
        GameInputManager.Get().OnPauseAction += PlaySelectionUI_OnPauseAction;
    }

    private void Awake()
    {
        Hide();

        nextButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.SelectedDifficulty.ToString(), (int)selectedDifficulty);
            PlayerPrefs.SetInt(PlayerPrefsKeys.SelectedSeason.ToString(), (int)selectedSeason);

            Hide();
            carSelectionUI.SetActive(true);
        });

        backButton.onClick.AddListener(() =>
        {
            Hide();
            playMenuUI.SetActive(true);
        });

    }

    private void PlaySelectionUI_OnPauseAction(object sender, System.EventArgs e)
    {
        Hide();
        playMenuUI.SetActive(true);
    }

    public void OnDifficultyDropdownChanged(int index)
    {
        selectedDifficulty = (Difficulty)index;
    }

    public void OnSeasonDropdownChanged(int index)
    {
        selectedSeason = (Season)index;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        GameInputManager.Get().OnPauseAction -= PlaySelectionUI_OnPauseAction;
    }
}
