using UnityEngine.SceneManagement;

public static class SceneLoadingManager
{
    public enum Scene
    {
        MainMenuScene,
        WinterScene,
        SummerScene,
        FallScene,
        SpringScene,
    }

    public static void Load(Scene targetScene)
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
    
    //Load same scene like restart button.
    public static void Load()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
