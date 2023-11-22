using UnityEngine;
using TMPro;

public class CarSpeedUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI carSpeedValue;

    private void FixedUpdate()
    {
        carSpeedValue.text = ((int) CarSelectionManager.Get().GetCurrentCarController().GetCarSpeed()).ToString();
    }
}
