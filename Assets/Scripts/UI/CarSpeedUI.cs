using UnityEngine;
using TMPro;
//using UnityEngine.UI;

public class CarSpeedUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI carSpeedValue;

    //public Button accelerationButton; 

    //private void Awake()
    //{
    //    accelerationButton.onClick.AddListener(() =>
    //    {
    //        CarSelectionManager.Get().GetCurrentCarController().SetMotorInput(GameInputManager.Get().GetMovementVertical());
    //    });  
    //}

    private void FixedUpdate()
    {
        carSpeedValue.text = ((int) CarSelectionManager.Get().GetCurrentCarController().GetCarSpeed()).ToString();
    }

}
