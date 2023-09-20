using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    private static GameOverUI Instance;

    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainmenuButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scoreValueText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreValueText;
    [SerializeField] private TextMeshProUGUI newBestScoreText;
    [SerializeField] private TextMeshProUGUI youWinText;
    [SerializeField] private TextMeshProUGUI yourCashValueText;

    [SerializeField] private TimerUI timerUI;

    private float bestScore;
    private bool isPlayerWon;
    private bool canPlayLoseSound;

    private const float duration = 0.9f;
    private int oldPlayerCash;
    private int newPlayerCash;
    private int playerReward;

    private Difficulty selectedDifficulty;

    public static GameOverUI Get() => Instance;

    private void OnEnable()
    {
        restartButton.Select();

        if (isPlayerWon)
        {
            SoundManager.Get().PlayWinSound();
            youWinText.gameObject.SetActive(true);

            UpdateUIPlayerCash();
            StartCoroutine(LerpPlayerCashValue());
        }
        else
        {
            if (canPlayLoseSound)
            {
                SoundManager.Get().PlayLoseSound();
            }
            youWinText.gameObject.SetActive(false);
            yourCashValueText.text = PlayerPrefs.GetInt(PlayerPrefsKeys.PlayerCash.ToString(), 0).ToString();

            CarSelectionManager.Get().GetCurrentCarController().StopCarAudio();
        }
    }

    private void Awake()
    {
        Instance = this;

        restartButton.onClick.AddListener(() =>
        {
            SceneLoadingManager.Load();
        });

        mainmenuButton.onClick.AddListener(() =>
        {
            SceneLoadingManager.Load(SceneLoadingManager.Scene.MainMenuScene);
        });
    }

    void Start()
    {
        Hide();
        CarGameManager.Get().OnRaceFinished += GameOverUI_OnRaceFinished;
        CarGameManager.Get().OnTimerFinished += GameOverUI_OnTimerFinished;
        CarSelectionManager.Get().GetCurrentCarController().OnPlayerWin += GameOverUI_OnPlayerWin;

        bestScore = PlayerPrefs.GetFloat(PlayerPrefsKeys.BestScore.ToString(), 0.0f);
        UpdateBestScore();

        int difficultyValue = PlayerPrefs.GetInt(PlayerPrefsKeys.SelectedDifficulty.ToString());
        selectedDifficulty = (Difficulty)difficultyValue;
    }

    private void GameOverUI_OnPlayerWin(object sender, CarController.OnPlayerWinEventArgs e)
    {
        isPlayerWon = e.isPlayerWon;
    }

    private void GameOverUI_OnTimerFinished(object sender, System.EventArgs e)
    {
        scoreText.gameObject.SetActive(true);
        scoreValueText.gameObject.SetActive(true);
        bestScoreText.gameObject.SetActive(true);
        bestScoreValueText.gameObject.SetActive(true);

        //show only when player select race mood.
        youWinText.gameObject.SetActive(false);

        scoreValueText.text = timerUI.GetTimerValueText();

        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.BestScore.ToString()))
        {
            bestScore = timerUI.GetCurrentTimer();
            PlayerPrefs.SetFloat(PlayerPrefsKeys.BestScore.ToString(), bestScore);
        }

        bestScore = PlayerPrefs.GetFloat(PlayerPrefsKeys.BestScore.ToString());
        if (bestScore > timerUI.GetCurrentTimer())
        {
            bestScore = timerUI.GetCurrentTimer();
            PlayerPrefs.SetFloat(PlayerPrefsKeys.BestScore.ToString(), bestScore);

            playerReward = 200;
            newBestScoreText.gameObject.SetActive(true);
        }

        UpdateBestScore();
    }

    private void UpdateBestScore()
    {
        bestScore = PlayerPrefs.GetFloat(PlayerPrefsKeys.BestScore.ToString());

        float minutes = Mathf.FloorToInt(bestScore / 60);
        float seconds = Mathf.FloorToInt(bestScore % 60);
        if (bestScore >= 60)
        {
            bestScoreValueText.text = string.Format("{0:0}:{1:00}", minutes, seconds) + " min";
        }
        else
        {
            bestScoreValueText.text = string.Format("{0:0}:{1:00}", minutes, seconds) + " sec";
        }
    }

    private void GameOverUI_OnRaceFinished(object sender, System.EventArgs e)
    {
        SetPlayerRewardBasedOnDifficulty();

        scoreText.gameObject.SetActive(false);
        scoreValueText.gameObject.SetActive(false);
        bestScoreText.gameObject.SetActive(false);
        bestScoreValueText.gameObject.SetActive(false);
        newBestScoreText.gameObject.SetActive(false);

        canPlayLoseSound = true;
    }

    private void UpdateUIPlayerCash()
    {
        oldPlayerCash = PlayerPrefs.GetInt(PlayerPrefsKeys.PlayerCash.ToString(), 0);
        PlayerPrefs.SetInt(PlayerPrefsKeys.PlayerCash.ToString(), playerReward + oldPlayerCash);

        newPlayerCash = PlayerPrefs.GetInt(PlayerPrefsKeys.PlayerCash.ToString(), 0);
        yourCashValueText.text = newPlayerCash.ToString();
    }

    IEnumerator LerpPlayerCashValue()
    {
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;

            yourCashValueText.text = Mathf.RoundToInt(Mathf.Lerp(oldPlayerCash, newPlayerCash, t)).ToString();

            timeElapsed += Time.deltaTime;

            yield return null;
        }
    }

    private void SetPlayerRewardBasedOnDifficulty()
    {
        switch (selectedDifficulty)
        {
            case Difficulty.Easy:
                playerReward = 100;
                break;
            case Difficulty.Medium:
                playerReward = 200;
                break;
            case Difficulty.Hard:
                playerReward = 400;
                break;
        }
    }

    private void Hide() => gameObject.SetActive(false);

    public void Show() => gameObject.SetActive(true);
}
