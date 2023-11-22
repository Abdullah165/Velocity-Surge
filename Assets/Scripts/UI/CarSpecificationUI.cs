using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CarSpecificationUI : MonoBehaviour
{
    [SerializeField] private CarSpecificationSO carSpecificationSO;

    [SerializeField] private Image speedBar;
    [SerializeField] private Image accelerationBar;
    [SerializeField] private Image brakeBar;
    [SerializeField] private Image handlingBar;

    [SerializeField] private TextMeshProUGUI carPrice; 

    private const float duration = 0.6f;

    private void OnEnable()
    {
        StartCoroutine(LerpBarsValue());
    }

    IEnumerator LerpBarsValue()
    {
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;

            speedBar.fillAmount = Mathf.Lerp(0, carSpecificationSO.speed, t);
            accelerationBar.fillAmount = Mathf.Lerp(0, carSpecificationSO.acceleration, t);
            brakeBar.fillAmount = Mathf.Lerp(0, carSpecificationSO.brake, t);
            handlingBar.fillAmount = Mathf.Lerp(0, carSpecificationSO.handling, t);
            carPrice.text = Mathf.RoundToInt(Mathf.Lerp(0, carSpecificationSO.Price, t)).ToString();

            timeElapsed += Time.fixedDeltaTime;

            yield return null;
        }
    }
}
