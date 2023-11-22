using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimationUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public Transform buttonTransform;

    private const float animationTime = 0.1f;

    [SerializeField] private Vector3 originalSize;
    [SerializeField] private Vector3 doubleSize;

    private void Awake()
    {
        originalSize = buttonTransform.localScale;
        doubleSize = originalSize * 1.2f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Do something when the pointer enters the collider of the GameObject.
        LeanTween.scale(buttonTransform.gameObject, doubleSize, animationTime).setEase(LeanTweenType.easeInOutSine);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Start an animation to scale the button back to its original size and change its color back to its original color.
        LeanTween.scale(buttonTransform.gameObject, originalSize, animationTime).setEase(LeanTweenType.easeInOutSine);
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Change the button color to green when the button is selected.
        LeanTween.scale(buttonTransform.gameObject, doubleSize, animationTime).setEase(LeanTweenType.easeInOutSine);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        // Change the button color to white when the button is deselected.
        LeanTween.scale(buttonTransform.gameObject, originalSize, animationTime).setEase(LeanTweenType.easeInOutSine);
    }
}
