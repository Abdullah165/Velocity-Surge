using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    private float timerValue;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI bestTimerScoreText;

    private float bestScore;

    private void Start()
    {
        timerValue = 0;
        CarGameManager.Get().OnStateChanged += TimerUI_OnStateChanged;

        //player select the timer mood.
        if (PlayerPrefs.GetInt(PlayerPrefsKeys.SelectedTimer.ToString()) == 0)
        {
            Hide();
        }

        UpdateBestTimerScore();
    }

    private void TimerUI_OnStateChanged(object sender, CarGameManager.OnStateChangedEventArgs e)
    {
        if (e.state == CarGameManager.State.GamePlaying)
        {
            if (PlayerPrefs.GetInt(PlayerPrefsKeys.SelectedTimer.ToString()) == 1)
            {
                Show();
            }
        }
        else
        {
            Hide();
        }
    }

    void Update()
    {
        timerValue += Time.deltaTime;

        DisplayTime();
    }

    private void DisplayTime()
    {
        float minutes = Mathf.FloorToInt(timerValue / 60);
        float seconds = Mathf.FloorToInt(timerValue % 60);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    public string GetTimerValueText()
    {
        if(timerValue > 60)
        {
           return  timerText.text + " min";
        }
        else
        {
            return  timerText.text + " sec";
        }
    }

    public float GetCurrentTimer() => timerValue;

    private void UpdateBestTimerScore()
    {
        bestScore = PlayerPrefs.GetFloat(PlayerPrefsKeys.BestScore.ToString());

        float minutes = Mathf.FloorToInt(bestScore / 60);
        float seconds = Mathf.FloorToInt(bestScore % 60);
        if (bestScore >= 60)
        {
            bestTimerScoreText.text = string.Format("{0:0}:{1:00}", minutes, seconds) + " min";
        }
        else
        {
            bestTimerScoreText.text = string.Format("{0:0}:{1:00}", minutes, seconds) + " sec";
        }
    }
}
