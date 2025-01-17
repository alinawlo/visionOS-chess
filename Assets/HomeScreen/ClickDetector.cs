using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDetector : MonoBehaviour
{
    public SearchBarHandler searchBarHandler; // Assign this in the inspector

    private Camera mainCamera;
    private Collider searchBarCollider;

    void Start()
    {
        mainCamera = Camera.main;
        searchBarCollider = searchBarHandler.GetComponent<Collider>(); // Ensure this is the collider for the search bar
    }

    void Update()
    {
        // Detect mouse click
        if (Input.GetMouseButtonDown(0))
        {
            DetectClickOrTouch(Input.mousePosition);
        }

        // Detect touch input
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            DetectClickOrTouch(Input.GetTouch(0).position);
        }
    }

    private void DetectClickOrTouch(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // If the hit object is the search bar, toggle its active state
            if (hit.collider == searchBarCollider)
            {
                searchBarHandler.OnSearchBarClicked();
            }
        }
        else
        {
            // If we didn't hit anything, we clicked/touched outside the search bar
            if (searchBarHandler.IsSearchBarActive())
            {
                searchBarHandler.DeactivateSearchBar();
            }
        }
    }
}

