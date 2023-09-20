using UnityEngine;

public class CarSpecificationsManager : MonoBehaviour
{
    private static CarSpecificationsManager Instance;

    [SerializeField] private CarSpecificationSO[] carSpecificationSOArray;
    private CarSpecificationSO currentCarSpecification;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (var specification in carSpecificationSOArray)
        {
            if (!PlayerPrefs.HasKey($"Car{specification}_Price"))
            {
                PlayerPrefs.SetInt($"Car{specification}_Price", specification.Price);
            }
        }
    }

    public void SetCurrentCar(int index)
    {
        if (index >= 0 && index < carSpecificationSOArray.Length)
        {
            currentCarSpecification = carSpecificationSOArray[index];
        }
    }

    public CarSpecificationSO GetCurrentCarSpecification() => currentCarSpecification;

    public CarSpecificationSO[] GetCarSpecificationSOArray() => carSpecificationSOArray;

    public static CarSpecificationsManager Get() => Instance;
}
