using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class PlayerCashUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerCashText;
    [SerializeField] private TextMeshProUGUI perfectDriftText;

    private const float duration = 1.5f;

    private int oldPlayerCash;
    private int newPlayerCash;

    private Vector3 perfectDriftTextOriginalPoistion;

    private void Start()
    {
        oldPlayerCash = PlayerPrefs.GetInt(PlayerPrefsKeys.PlayerCash.ToString(), 0);
        playerCashText.text = oldPlayerCash.ToString();

        perfectDriftText.gameObject.SetActive(false);
        perfectDriftTextOriginalPoistion = perfectDriftText.transform.localPosition;

        CarSelectionManager.Get().GetCurrentCarController().OnCarDrifted += PlayerCashUI_OnCarDrifted;

    }

    private void PlayerCashUI_OnCarDrifted(object sender, System.EventArgs e)
    {
        UpdatePlayerCash();

        perfectDriftText.gameObject.SetActive(true);

        StartCoroutine(LerpPlayerCashValue());
    }

    private void UpdatePlayerCash()
    {
        oldPlayerCash = PlayerPrefs.GetInt(PlayerPrefsKeys.PlayerCash.ToString(), 0);
        int reward = 10;
        PlayerPrefs.SetInt(PlayerPrefsKeys.PlayerCash.ToString(), reward + oldPlayerCash);

        newPlayerCash = PlayerPrefs.GetInt(PlayerPrefsKeys.PlayerCash.ToString(), 0);
    }

    IEnumerator LerpPlayerCashValue()
    {
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;

            playerCashText.text = Mathf.RoundToInt(Mathf.Lerp(oldPlayerCash, newPlayerCash, t)).ToString();

            perfectDriftText.transform.localPosition = new Vector3(0,Mathf.Lerp(perfectDriftText.transform.localPosition.y, perfectDriftText.transform.localPosition.y + 2, t),0);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        perfectDriftText.gameObject.SetActive(false);
        ResetPerfectDriftTextPosition();
    }

    private void ResetPerfectDriftTextPosition()
    {
        perfectDriftText.transform.localPosition = perfectDriftTextOriginalPoistion;
    }
}
