using UnityEngine;
using UnityEngine.UI;

public class MobileUI : MonoBehaviour
{
    [SerializeField] private Button accelerationButton;
    [SerializeField] private Button pauseButton;

    private void Start()
    {
        CarGameManager.Get().OnStateChanged += MobileUI_OnStateChanged;
    }

    private void MobileUI_OnStateChanged(object sender, CarGameManager.OnStateChangedEventArgs e)
    {
        if(e.state == CarGameManager.State.GameOver)
        {
            //disable acceleration and pause button (that's for the player can not interact with anything unless GameOverUI menu)
            accelerationButton.GetComponent<Image>().raycastTarget = false;
            accelerationButton.interactable = false;

            pauseButton.interactable = false;
            pauseButton.GetComponent<Image>().raycastTarget = false;
        }
    }
}
