using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolumeController : MonoBehaviour
{
    [SerializeField] private Volume volume;

    private Vignette vignette;

    // Start is called before the first frame update
    void Start()
    {
        // Check if the Volume component has a Vignette override
        if (volume.profile.TryGet(out Vignette vignette))
        {
            this.vignette = vignette;
        }

        GameInputManager.Get().OnNitroActionReady += GlobalVolumeController_OnNitroActionReady;
        GameInputManager.Get().OnNitroActionCanceled += GlobalVolumeController_OnNitroActionCanceled;
    }

    private void GlobalVolumeController_OnNitroActionCanceled(object sender, System.EventArgs e)
    {
        vignette.intensity.value = 0.196f;
        vignette.smoothness.value = 0.279f;
    }

    private void GlobalVolumeController_OnNitroActionReady(object sender, System.EventArgs e)
    {
        vignette.intensity.value = 0.259f; 
        vignette.smoothness.value = 0.349f; 
    }
}
