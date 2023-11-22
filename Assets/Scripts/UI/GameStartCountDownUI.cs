using UnityEngine;
using TMPro;

public class GameStartCountDownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countDownText;

    private const int MINIMUM_COUNTDOWN_TIMER = 1;

    // Start is called before the first frame update
    void Start()
    {
        CarGameManager.Get().OnStateChanged += GameStartCountDownUI_OnStateChanged;
    }

    private void GameStartCountDownUI_OnStateChanged(object sender, CarGameManager.OnStateChangedEventArgs e)
    {
        if (e.state == CarGameManager.State.CountDownToStart)
        {
            if (CarGameManager.Get().GetCountDownToStartTimer() < MINIMUM_COUNTDOWN_TIMER)
            {
                countDownText.text = "GO!";
            }
            else
            {
                countDownText.text = ((int)CarGameManager.Get().GetCountDownToStartTimer()).ToString();
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
