using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class RacePositionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalCarsText;
    [SerializeField] private TextMeshProUGUI currentPlayerCarPositionText;

    [SerializeField] private int CheckpointProximity = 20;

    public List<Transform> checkPoints;

    private Transform playerCar;
    public List<Transform> aiCars;

    List<Transform> allCars;
    List<Transform> sortedCars;

    int playerPosition;
    int index = 0;


    private void Awake()
    {
        allCars = new List<Transform>();

        // Add only visiable cars
        foreach (var aiCar in aiCars)
        {
            if (aiCar.gameObject.activeSelf)
            {
                allCars.Add(aiCar);
            }
        }

        totalCarsText.text = allCars.Count().ToString();
    }

    private void Start()
    {
        playerCar = CarSelectionManager.Get().GetCurrentCarController().transform;
    }

    private void Update()
    {
        if (allCars.Any(car => Vector3.Distance(car.position, checkPoints[index].position) < CheckpointProximity))
        {
            sortedCars = allCars
               .OrderBy(car => Vector3.Distance(car.position, checkPoints[index].position))
               .ToList();

            if (index == checkPoints.Count - 1)
            {
                index = 0;
            }
            else
            {
                index++;
            }
        }
        else
        {
            sortedCars = allCars
                .OrderBy(car => Vector3.Distance(car.position, checkPoints[index].position))
                .ToList();
        }

        playerPosition = sortedCars.IndexOf(playerCar) + 1;

        currentPlayerCarPositionText.text = playerPosition.ToString();

       // If the car is out of the track remove it and update total car text.
        foreach (var aiCar in aiCars)
        {
            if (!aiCar.gameObject.activeSelf)
            {
                allCars.Remove(aiCar);
            }
        }

        totalCarsText.text = allCars.Count().ToString();
    }
}
