using System;
using UnityEngine;

public class CarGameManager : MonoBehaviour
{
    private static CarGameManager instatnce;

    [SerializeField] private GameObject[] carIconArray;

    public enum State
    {
        CountDownToStart,
        GamePlaying,
        GameOver
    }

    private State state;

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public event EventHandler OnTimerFinished;
    public event EventHandler OnRaceFinished;

    private float countDownToStartTimer = 4;

    private bool isGameOver = false;

    private void Awake()
    {
        instatnce = this;

        state = State.CountDownToStart;

        Time.timeScale = 1;

        foreach (GameObject car in carIconArray)
        {
            car.SetActive(true);
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.CountDownToStart:
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = State.CountDownToStart
                });

                countDownToStartTimer -= Time.deltaTime;
                if (countDownToStartTimer < 0)
                {
                    state = State.GamePlaying;

                }
                break;
            case State.GamePlaying:
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = State.GamePlaying
                });

                if (isGameOver)
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state
                    });
                }
                break;
            case State.GameOver:
                // player was selected race mood
                if (PlayerPrefs.GetInt(PlayerPrefsKeys.SelectedTimer.ToString()) == 0)
                {
                    OnRaceFinished?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    // player was selected timer mood.
                    OnTimerFinished?.Invoke(this, EventArgs.Empty);
                }
                break;
        }
    }

    public bool IsGamePlaying() => state == State.GamePlaying;

    public void SetGameOver(bool isGameOver) => this.isGameOver = isGameOver;

    public float GetCountDownToStartTimer() => countDownToStartTimer;

    public static CarGameManager Get() => instatnce;
}
