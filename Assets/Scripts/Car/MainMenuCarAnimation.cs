using UnityEngine;

public class MainMenuCarAnimation : MonoBehaviour
{
    private const float rotationSpeed = 30;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
