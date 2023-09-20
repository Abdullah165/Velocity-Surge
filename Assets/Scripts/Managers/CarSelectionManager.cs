using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

public class CarSelectionManager : MonoBehaviour
{
    private static CarSelectionManager Instance;

    [SerializeField] private CinemachineFreeLook cinemachineFreeLook;
    [SerializeField] private GameObject[] carModels;
    private int currentCarIndex = 0;

    private Difficulty selectedDifficulty;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        currentCarIndex = PlayerPrefs.GetInt(PlayerPrefsKeys.SelectedCar.ToString(), 0);

        EnablePlayerControl();
        EnableAIControlForRestCars();

        cinemachineFreeLook.Follow = carModels[currentCarIndex].transform;
        cinemachineFreeLook.LookAt = carModels[currentCarIndex].transform;

        int difficultyValue = PlayerPrefs.GetInt(PlayerPrefsKeys.SelectedDifficulty.ToString());
        selectedDifficulty = (Difficulty)difficultyValue;

        // if player selected racing mood show cars depending on difficulty.
        if (PlayerPrefs.GetInt(PlayerPrefsKeys.SelectedTimer.ToString()) == 0)
        {
            SetNumberOfCarsBasedOnDifficulty();
        }
        else
        {
            // if player selected timer mood then hide all cars except of course player's car.
            HideAllCars();
        }

        // scale current car icon in Minimap so player can know where is he in Minimap.
        GetCurrentCarController().GetCarIcon().localScale = new Vector3(25, 35, 25);
    }

    public static CarSelectionManager Get()
    {
        return Instance;
    }

    public CarController GetCurrentCarController()
    {
        return carModels[currentCarIndex].GetComponent<CarController>();
    }

    private void EnablePlayerControl()
    {
        carModels[currentCarIndex].GetComponent<CarController>().enabled = true;
        carModels[currentCarIndex].GetComponent<AICarController>().enabled = false;

        // Disable CarController Script for the rest of the Cars (AI Cars) except player's car.
        foreach (GameObject car in carModels)
        {
            if (car == carModels[currentCarIndex]) continue;
            car.GetComponent<CarController>().enabled = false;
        }
    }

    private void EnableAIControlForRestCars()
    {
        foreach (GameObject car in carModels)
        {
            // Enable CarAI script for the rest of the Cars except player's car.
            if (car == carModels[currentCarIndex]) continue;
            car.GetComponent<AICarController>().enabled = true;
        }
    }

    private void SetNumberOfCarsBasedOnDifficulty()
    {
        switch (selectedDifficulty)
        {
            case Difficulty.Easy:
                ShowQuarterOfCars();
                break;
            case Difficulty.Medium:
                ShowHalfOfCars();
                break;
            case Difficulty.Hard:
                ShowAllCars();
                break;
        }
    }

    private void ShowQuarterOfCars()
    {
        int[] randomCars = new int[carModels.Length / 4];

        for (int index = 0; index < carModels.Length; index++)
        {
            // if index == currentIndex means it's player's car so don't make it disable.
            if (index == currentCarIndex) continue;

            carModels[index].SetActive(false);

            if (index < (carModels.Length / 4))
            {
                int randomIndex = Random.Range(0, carModels.Length);

                // I once test a game and suddenly i don't see any AI car with me because every time randomIndex generated was same
                // index of the player car so here 👇👇 i make sure to generate AI cars if that happen again.
                if (randomIndex == currentCarIndex)
                {
                    if (randomIndex < carModels.Length)
                    {
                        randomIndex++;
                        randomCars[index] = randomIndex;
                    }
                }
                else
                {
                    randomCars[index] = randomIndex;
                }
            }
        }

        for (int index = 0; index < randomCars.Length; index++)
        {
            // Show the car which was randomly selected.
            carModels[randomCars[index]].SetActive(true);
        }
    }

    private void ShowHalfOfCars()
    {
        int[] randomCars = new int[carModels.Length / 2];

        for (int index = 0; index < carModels.Length; index++)
        {
            // if index == currentIndex means it's player's car so don't make it disable.
            if (index == currentCarIndex) continue;

            carModels[index].SetActive(false);

            if (index < (carModels.Length / 2))
            {
                int randomIndex = Random.Range(0, carModels.Length);

                // I once test a game and suddenly i don't see any AI car with me because every time randomIndex generated was same
                // index of the player car so here 👇👇 i make sure to generate AI cars if that happen again.
                if (randomIndex == currentCarIndex)
                {
                    if (randomIndex < carModels.Length)
                    {
                        randomIndex++;
                        randomCars[index] = randomIndex;
                    }
                }
                else
                {
                    randomCars[index] = randomIndex;
                }
            }
        }

        for (int index = 0; index < randomCars.Length; index++)
        {
            // Show the car which was randomly selected.
            carModels[randomCars[index]].SetActive(true);
        }
    }

    private void ShowAllCars()
    {
        for (int index = 0; index < carModels.Length; index++)
        {
            carModels[index].SetActive(true);
        }
    }

    private void HideAllCars()
    {
        for (int index = 0; index < carModels.Length; index++)
        {
            // don't disable the player's car.
            if (index == currentCarIndex) continue;
            carModels[index].SetActive(false);
        }
    }
}
