using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    [SerializeField] private AudioSource buttonAudio;

    public void HoverButtonSound()
    {
        buttonAudio.Play();
    }
}
