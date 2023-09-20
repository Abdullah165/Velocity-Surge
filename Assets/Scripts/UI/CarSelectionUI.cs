using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarSelectionUI : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button backButton;

    [SerializeField] private Button nextCarButton;
    [SerializeField] private Button previousCarButton;

    [SerializeField] private TextMeshProUGUI playerCashText;
    [SerializeField] private TextMeshProUGUI carPriceText;

    [SerializeField] private GameObject playMenuUI;

    [SerializeField] private GameObject[] carModels;

    private int currentIndex = 0;

    private Season selectedSeason;

    private int playerCashAmount = 100;
    private int carPrice;
    private const int freePrice = 0;

    private void OnEnable()
    {
        nextButton.Select();
        GameInputManager.Get().OnPauseAction += CarSelectionUI_OnPauseAction;
    }

    private void Awake()
    {
        Hide();
        nextButton.onClick.AddListener(() =>
        {
            CarSpecificationSO currentCarSpecification = CarSpecificationsManager.Get().GetCurrentCarSpecification();
            int currentCarPrice = PlayerPrefs.GetInt($"Car{currentCarSpecification}_Price");

            // player select allowed car and ready to play.
            if (currentCarPrice == freePrice)
            {
                LoadTargetScene();
            }
        });

        buyButton.onClick.AddListener(() =>
        {
            playerCashAmount = PlayerPrefs.GetInt(PlayerPrefsKeys.PlayerCash.ToString(), 0);
            UpdateCarPriceUI();
            PlayerPrefs.SetInt(PlayerPrefsKeys.PlayerCash.ToString(), playerCashAmount);
            playerCashText.text = playerCashAmount.ToString();

            nextButton.gameObject.SetActive(true);
            buyButton.gameObject.SetActive(false);

            nextButton.Select();
        });

        backButton.onClick.AddListener(() =>
        {
            Hide();
            playMenuUI.SetActive(true);
        });

        nextCarButton.onClick.AddListener(() =>
        {
            SelectNextCar();

            if (CanBuyNewCar())
            {
                buyButton.gameObject.SetActive(true);
                nextButton.gameObject.SetActive(false);
            }
            else
            {
                nextButton.gameObject.SetActive(true);
                buyButton.gameObject.SetActive(false);
            }
        });

        previousCarButton.onClick.AddListener(() =>
        {
            SelectPreviousCar();

            if (CanBuyNewCar())
            {
                buyButton.gameObject.SetActive(true);
                nextButton.gameObject.SetActive(false);
            }
            else
            {
                nextButton.gameObject.SetActive(true);
                buyButton.gameObject.SetActive(false);
            }
        });
    }

    void Start()
    {
        foreach (GameObject car in carModels)
        {
            car.SetActive(false);
        }

        LoadCarPrice();

        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.PlayerCash.ToString()))
        {
            // Set value to PlayerCash key only once.
            PlayerPrefs.SetInt(PlayerPrefsKeys.PlayerCash.ToString(), playerCashAmount);
        }

        UpdatePlayerCashTextUI();

        buyButton.gameObject.SetActive(false);

        currentIndex = PlayerPrefs.GetInt(PlayerPrefsKeys.SelectedCar.ToString(), 0);

        carModels[currentIndex].SetActive(true);
        CarSpecificationsManager.Get().SetCurrentCar(currentIndex);

        int seasonValue = PlayerPrefs.GetInt(PlayerPrefsKeys.SelectedSeason.ToString());
        selectedSeason = (Season)seasonValue;
    }

    private void UpdatePlayerCashTextUI()
    {
        playerCashAmount = PlayerPrefs.GetInt(PlayerPrefsKeys.PlayerCash.ToString(), 0);
        playerCashText.text = playerCashAmount.ToString();
    }

    private void UpdateCarPriceUI()
    {
        playerCashAmount -= carPrice;

        CarSpecificationSO currentCarSpecification = CarSpecificationsManager.Get().GetCurrentCarSpecification();

        if (currentCarSpecification != null)
        {
            currentCarSpecification.Price = 0;
            PlayerPrefs.SetInt($"Car{currentCarSpecification}_Price", currentCarSpecification.Price);

            carPrice = currentCarSpecification.Price;
            carPriceText.text = carPrice.ToString();
        }
    }

    private void LoadCarPrice()
    {
        foreach (var currentCarSpecification in CarSpecificationsManager.Get().GetCarSpecificationSOArray())
        {
            int carPrice = PlayerPrefs.GetInt($"Car{currentCarSpecification}_Price");
            currentCarSpecification.Price = carPrice;
            carPriceText.text = carPrice.ToString();
        }
    }

    private void CarSelectionUI_OnPauseAction(object sender, System.EventArgs e)
    {
        Hide();
        playMenuUI.SetActive(true);
    }

    private void SelectNextCar()
    {
        carModels[currentIndex].SetActive(false);
        currentIndex++;

        if (currentIndex == carModels.Length)
        {
            currentIndex = 0;
        }

        carModels[currentIndex].SetActive(true);
        CarSpecificationsManager.Get().SetCurrentCar(currentIndex); // Update the current car specification

        PlayerPrefs.SetInt(PlayerPrefsKeys.SelectedCar.ToString(), currentIndex);
    }

    private void SelectPreviousCar()
    {
        carModels[currentIndex].SetActive(false);
        currentIndex--;

        if (currentIndex == -1)
        {
            currentIndex = carModels.Length - 1;
        }

        carModels[currentIndex].SetActive(true);
        CarSpecificationsManager.Get().SetCurrentCar(currentIndex); // Update the current car specification

        PlayerPrefs.SetInt(PlayerPrefsKeys.SelectedCar.ToString(), currentIndex);
    }

    private bool CanBuyNewCar()
    {
        CarSpecificationSO currentCarSpecification = CarSpecificationsManager.Get().GetCurrentCarSpecification();

        if (currentCarSpecification != null)
        {
            carPrice = currentCarSpecification.Price;
            if (playerCashAmount >= carPrice && carPrice != freePrice)
            {
                return true;
            }
        }
        return false;
    }

    private void LoadTargetScene()
    {
        // Load the corresponding scene based on the selected season
        switch (selectedSeason)
        {
            case Season.Winter:
                SceneLoadingManager.Load(SceneLoadingManager.Scene.WinterScene);
                break;
            case Season.Summer:
                SceneLoadingManager.Load(SceneLoadingManager.Scene.SummerScene);
                break;
            case Season.Fall:
                SceneLoadingManager.Load(SceneLoadingManager.Scene.FallScene);
                break;
            case Season.Spring:
                SceneLoadingManager.Load(SceneLoadingManager.Scene.SpringScene);
                break;
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        GameInputManager.Get().OnPauseAction -= CarSelectionUI_OnPauseAction;
    }
}
