using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<GamePrefabPair> gamePrefabPairs; // Assign this in the inspector    
    [SerializeField] private Transform objectArea;
    [SerializeField] private TextMeshPro titleText;
    [SerializeField] private TextMeshPro descriptionText;
    [SerializeField] private EventSystem eventSystem; // Assign this in the inspector
    private List<GameData> gamesList; // This will hold your games data
    public int currentIndex = 0; // This will keep track of the current game
    private List<GameObject> gameButtons = new List<GameObject>(); // Declare this in UIManager if needed
    public SearchBarHandler searchBarHandler;

    public void SetGamesList(List<GameData> games)
    {
        gamesList = games;
        UpdateGameUI(); // Update UI with the first game data
    }

    public void OnRightArrowClicked()
    {
        if (gamesList == null)
        {
            Debug.LogError("gamesList is null. Make sure it is initialized before clicking the arrow.");
            return;
        }

        if (searchBarHandler.IsSearchBarActive())
        {
            searchBarHandler.DeactivateSearchBar();
        }
        // Increment the currentIndex, wrap around if at the end
        currentIndex = (currentIndex + 1) % gamesList.Count;
        UpdateGameUI(); // Update UI with the new game data
        SelectGameButton(currentIndex);
    }
    
    public void OnLeftArrowClicked()
    {
        if (gamesList == null || gamesList.Count == 0)
        {
            Debug.LogError("gamesList is not initialized.");
            return;
        }

        // Check if currentIndex is at the first game, wrap around to the last game if true
        if (currentIndex == 0)
        {
            currentIndex = gamesList.Count - 1;
        }
        else
        {
            // Otherwise, just decrement the currentIndex
            currentIndex -= 1;
        }

        if (searchBarHandler.IsSearchBarActive())
        {
            searchBarHandler.DeactivateSearchBar();
        }
        // Update UI with the previous game's title and description
        UpdateGameUI(gamesList[currentIndex].title, gamesList[currentIndex].description, currentIndex);

        // Select the corresponding game button
        SelectGameButton(currentIndex);
    }   


    public void UpdateGameUI()
    {
        // Check if we have any games in the list
        if (gamesList == null || gamesList.Count == 0)
        {
            Debug.LogError("No games data available.");
            return;
        }

        // Update the UI elements
        titleText.text = gamesList[currentIndex].title;
        descriptionText.text = gamesList[currentIndex].description;
        Load3dObject(currentIndex);
    }
    
    public void UpdateGameUI(string newTitle, string newDescription, int index)
    {
        titleText.text = newTitle;         // Update the title UI element
        descriptionText.text = newDescription; // Update the description UI element
        currentIndex = index;

        Load3dObject(index);
    }
    
    
     public void SelectGameButton(int index)
    {
        if (gameButtons == null || gameButtons.Count == 0 || index < 0 || index >= gameButtons.Count)
        {
            Debug.LogError("Invalid gameButtons list or index.");
            return;
        }

        // Clear current selection
        EventSystem.current.SetSelectedGameObject(null);

        // Set new selection
        EventSystem.current.SetSelectedGameObject(gameButtons[index].gameObject);

    }

    // private void InitializeGameButton(GameObject buttonObj, GameData gameData, int index)
    // {
    //     GameButton gameButton = buttonObj.AddComponent<GameButton>();
    //     gameButton.gameData = gameData;

    //     // Set the border based on whether this is the currently selected button
    //     gameButton.SetBorderVisibility(index == currentIndex);
    // }

    public void SetGameButtons(List<GameObject> buttons)
    {
        this.gameButtons = buttons;
        SelectGameButton(currentIndex);
    }


    public void LoadScene()
    {
        StartCoroutine("LoadAsync");
    }

    IEnumerator LoadAsync(){
        yield return SceneManager.LoadSceneAsync(gamesList[currentIndex].title, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
        var home = GameObject.Find("Home Menu");
        if(home != null) {
            foreach(Transform child in home.transform){
                child.gameObject.SetActive(false);
            }
        }
    }

    public void Load3dObject(int index){

        if (searchBarHandler.IsSearchBarActive())
        {
            searchBarHandler.DeactivateSearchBar();
        }

        for (int i = 0; i < gameButtons.Count; i++)
        {
            GameButton gameButton = gameButtons[i].GetComponent<GameButton>();
            if (gameButton != null)
            {
                // Set the border visibility based on whether this is the selected button
                gameButton.SetBorderVisibility(i == index);
            }
        }


        // Clear previous 3D objects
        foreach (Transform child in objectArea)
        {
            Destroy(child.gameObject);
        }

        // Find the prefab for the selected game
        GamePrefabPair pair = gamePrefabPairs.Find(p => p.title == gamesList[index].title);
        if (pair != null)
        {
            // Instantiate the correct prefab at the 3dObjectArea
            GameObject instance = Instantiate(pair.prefab, objectArea);
            instance.transform.localPosition = new Vector3(0, (float)-1.2, (float)2.2);
            instance.transform.localScale = new Vector3(5,5,5);
        }
        else
        {
            Debug.LogWarning($"Prefab for game {gamesList[index].title} not found.");
        }
    }

}

[System.Serializable]
public class GamePrefabPair
{
    public string title;
    public GameObject prefab;
}