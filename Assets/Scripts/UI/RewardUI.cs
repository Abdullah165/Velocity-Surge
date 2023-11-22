using UnityEngine;
using UnityEngine.UI;

public class RewardUI : MonoBehaviour
{

    [SerializeField] private Button backButton;

    private void Awake()
    {
        gameObject.SetActive(false);

        backButton.onClick.AddListener(() =>
        {
            SceneLoadingManager.Load(SceneLoadingManager.Scene.MainMenuScene);
        });
    }
}
