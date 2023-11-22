using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NitroUI : MonoBehaviour
{
    private static NitroUI Instance;

    [SerializeField] private Image backgroundLighter;
    [SerializeField] private Image nitroBar;

    private const float nitroFillSpeed = 0.05f;
    private const float nitroRefillSpeed = 0.012f;

    private Coroutine decreaseCoroutine;
    private Coroutine refillCoroutine;


    public bool HasNitro()
    {
        return nitroBar.fillAmount > 0;
    }

    public static NitroUI Get() => Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        backgroundLighter.gameObject.SetActive(false);

        GameInputManager.Get().OnNitroActionReady += NitroUI_OnNitroAction;
        GameInputManager.Get().OnNitroActionCanceled += NitroUI_OnNitroActionCanceled;
    }

    private void NitroUI_OnNitroActionCanceled(object sender, System.EventArgs e)
    {
        backgroundLighter.gameObject.SetActive(false);
        StartRefillCoroutine();
        StopDecreaseCoroutine();
    }

    private void NitroUI_OnNitroAction(object sender, System.EventArgs e)
    {
        backgroundLighter.gameObject.SetActive(true);
        StartDecreaseCoroutine();
        StopRefillCoroutine();
    }

    private IEnumerator DecreaseNitro()
    {
        while (nitroBar.fillAmount > 0f && CarGameManager.Get().IsGamePlaying())
        {
            nitroBar.fillAmount -= nitroFillSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator RefillNitro()
    {
        while (nitroBar.fillAmount < 1f)
        {
            nitroBar.fillAmount += nitroRefillSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private void StartDecreaseCoroutine()
    {
        if (decreaseCoroutine == null)
        {
            decreaseCoroutine = StartCoroutine(DecreaseNitro());
        }
    }

    private void StopDecreaseCoroutine()
    {
        if (decreaseCoroutine != null)
        {
            StopCoroutine(decreaseCoroutine);
            decreaseCoroutine = null;
        }
    }

    private void StartRefillCoroutine()
    {
        if (refillCoroutine == null)
        {
            refillCoroutine = StartCoroutine(RefillNitro());
        }
    }

    private void StopRefillCoroutine()
    {
        if (refillCoroutine != null)
        {
            StopCoroutine(refillCoroutine);
            refillCoroutine = null;
        }
    }
}
