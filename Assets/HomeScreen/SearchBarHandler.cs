using TMPro;
using UnityEngine;

public class SearchBarHandler : MonoBehaviour
{
    public TextMeshPro searchTextDisplay; // Assign in the inspector
    private string currentSearchText = "Search...";
    private bool isSearchBarActive = false;

    public void ToggleSearchBarActive()
    {
        isSearchBarActive = !isSearchBarActive;
    }

    public bool IsSearchBarActive()
    {
        return isSearchBarActive;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSearchBarActive)
        {
            // Capture keyboard input and update the search text
            foreach (char c in Input.inputString)
            {
                if (c == '\b') // has backspace/delete been pressed?
                {
                    if (currentSearchText.Length != 0)
                    {
                        currentSearchText = currentSearchText.Substring(0, currentSearchText.Length - 1);
                    }
                }
                else if ((c == '\n') || (c == '\r')) // enter/return
                {
                    PerformSearch();
                }
                else
                {
                    currentSearchText += c;
                }
            }
        }

        // Update the TextMeshPro text
        searchTextDisplay.text = currentSearchText;
    }

    public void OnSearchBarClicked()
    {
        if (!isSearchBarActive)
        {
            currentSearchText = ""; // Clear default text when search bar is first activated
        }
        ToggleSearchBarActive();
    }

    public void DeactivateSearchBar()
    {
        isSearchBarActive = false;
        if (string.IsNullOrEmpty(currentSearchText))
        {
            currentSearchText = "Search..."; // Reset to default text if empty
        }
    }


    public void PerformSearch()
    {
        // Implement search logic to filter game data
        // This could call another method in a different script that handles the filtering
        GamesListLoader loader = FindObjectOfType<GamesListLoader>();
        if (loader != null)
        {
            loader.FilterGameList(currentSearchText.ToLower());
            Debug.Log("Perform search for: " + currentSearchText);
        }
        else
        {
            Debug.LogError("No GamesListLoader found in the scene.");
        }        

        DeactivateSearchBar();
        // For example: uiManager.FilterGameList(query);
    }
}
