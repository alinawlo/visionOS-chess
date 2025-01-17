using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro; // Make sure to include this for TextMeshProUGUI

public class GamesListLoader : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform contentPanel;
    [SerializeField] private float spacing = 100f; // Adjust this value based on your needs for spacing between buttons
    [SerializeField] private UIManager uiManager;
    // [SerializeField] private TextMeshPro searchInputField; // Reference to the search input field
    // [SerializeField] private Transform searchButton;
    
    private List<GameObject> gameButtons = new List<GameObject>(); // Add this line
    private List<GameData> allGames;

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        LoadGamesList();
        // searchInputField.selectionColor = new Color32(0, 120, 255, 255); // RGBA blue with full alpha
    }
    
    public void OnSearchButtonClicked()
    {
        // Debug.Log("Search button was clicked. " + searchInputField.text);
        // string searchText = searchInputField.text;
        // FilterGameList(searchText.ToLower());
    }

    public void FilterGameList(string searchText)
    {
        Debug.Log($"Filtering game list with search text: {searchText}");
        foreach (GameObject buttonObj in gameButtons)
        {
            if (buttonObj == null)
            {
                Debug.LogError("A button GameObject in gameButtons is null.");
                continue; // Skip this iteration
            }
            
            GameButton gameButton = buttonObj.GetComponent<GameButton>(); // Get the GameButton component
            if (gameButton != null && gameButton.gameData != null) // Check if the GameButton and GameData are not null
            {
                bool isMatch = string.IsNullOrEmpty(searchText) ||
                            (!string.IsNullOrEmpty(gameButton.gameData.title) && gameButton.gameData.title.ToLower().Contains(searchText));
                buttonObj.SetActive(isMatch);
            }
            else
            {
                Debug.LogError($"GameButton component or GameData not found on button object: {buttonObj.name}");
            }       
        }
    }

    
    private void LoadGamesList()
    {
        gameButtons.Clear();
        // string filePath = System.IO.Path.Combine(Application.dataPath, "HomeScreen/games_list.json");
        // string jsonText = System.IO.File.ReadAllText(filePath);
        string jsonText = "{\"inputs\": [{\"id\": 1,\"title\": \"Chess\",\"description\": \"A two-player strategy board game played on a checkered board with 64 squares arranged in an 8×8 grid.\"},{\"id\": 2,\"title\": \"Checkers\",\"description\": \"A strategy board game for two players which involves diagonal moves of uniform game pieces and mandatory captures by jumping over opponent pieces.\"},{\"id\": 3,\"title\": \"Backgammon\",\"description\": \"One of the oldest known board games. Its history can be traced back nearly 5,000 years to archaeological discoveries in the Middle East.\"}]}";
        GamesList gamesList = JsonConvert.DeserializeObject<GamesList>(jsonText);
        Debug.Log("Loaded Json Game List");

        if (gamesList.inputs.Count > 0)
        {
            // // Assuming each button has the same width or you're okay with using the first button to calculate
            // GameObject firstButton = Instantiate(buttonPrefab, contentPanel);
            // float buttonWidth = 100;
            // //firstButton.GetComponent<RectTransform>().rect.width;
            // Destroy(firstButton); // Destroy the instance as it was only used for measurement

            // float totalWidthNeeded = buttonWidth * gamesList.inputs.Count + 30 * (gamesList.inputs.Count - 1);
            // float startPositionX = -(totalWidthNeeded / 2) + (buttonWidth / 2);
            
            if (uiManager != null)
            {
               uiManager.SetGamesList(gamesList.inputs);
            }
            else
            {
                Debug.LogError("UIManager reference not set in GamesListLoader.");
            }
        
            uiManager.UpdateGameUI(gamesList.inputs[0].title, gamesList.inputs[0].description,0);

            for (int i = 0; i < gamesList.inputs.Count; i++)
            {
                Debug.Log($"Loading game: Title = {gamesList.inputs[i].title}");
                CreateGameButton(gamesList.inputs[i], i, 0.8);
            }
            uiManager.SetGameButtons(gameButtons);
        }
    }

    private void CreateGameButton(GameData gameData, int index, double startPositionX)
    {
        if (gameData == null)
        {
            Debug.LogError("gameData is null");
            return;
        }

        GameObject buttonObj = Instantiate(buttonPrefab, contentPanel);
        buttonObj.name = $"{gameData.title} Button";

        GameButton gameButton = buttonObj.AddComponent<GameButton>(); // Add the GameButton component
        gameButton.gameData = gameData;

        gameButton.border = buttonObj.transform.Find("Border").gameObject; // Replace with your actual border object name
        gameButton.SetBorderVisibility(false);
        
        gameButtons.Add(buttonObj);

        // Find and set the TextMeshPro text
        TextMeshPro buttonText = buttonObj.transform.GetComponentInChildren<TextMeshPro>();
        buttonText.text = gameData.title;


        double posX = startPositionX - spacing * index;
        buttonObj.transform.localPosition = new Vector3((float)posX, buttonObj.transform.localPosition.y, buttonObj.transform.localPosition.z);

        CustomSpatialUIButton customButton = buttonObj.GetComponent<CustomSpatialUIButton>();
        if (customButton != null)
        {
            customButton.m_ButtonText = gameData.title; // This sets the text and updates the TextMeshPro
            customButton.pressEvent.AddListener(() => uiManager.UpdateGameUI(gameData.title, gameData.description, index));
        }
        else
        {
            Debug.LogError("CustomSpatialUIButton component not found on the prefab.");
        }

    }

}

[System.Serializable]
public class GamesList
{
    public List<GameData> inputs;
}

[System.Serializable]
public class GameData
{
    public int id;
    public string title;
    public string description;
}

public class GameButton : MonoBehaviour
{
    public GameData gameData;

    public GameObject border; // Assign this in the inspector

    public void SetBorderVisibility(bool isVisible)
    {
        if (border != null)
        {
            border.SetActive(isVisible);
        }
    }
}


