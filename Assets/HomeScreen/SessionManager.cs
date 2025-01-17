using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class SessionManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject sessionManagementPanel;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Transform statusArea; // Assign this in the inspector
    [SerializeField] private GameObject textFieldPrefab; // Assign a prefab for the text fields in the inspector

    
    private void Start()
    {
        UpdateStatus(false);
        sessionManagementPanel.SetActive(false);
        // Connect to Photon servers
        PhotonNetwork.ConnectUsingSettings();
    }
    
    // Call this method when the session button is clicked
    public void ToggleSessionPanel()
    {
        // Toggle the active state of the session management panel
        sessionManagementPanel.SetActive(!sessionManagementPanel.activeSelf);
    }
    
    public void UpdateStatus(bool isConnected)
    {
        statusText.text = isConnected ? "Connected" : "Disconnected";
    }
    
    public override void OnConnectedToMaster()
    {
        // Call the base method if needed
        base.OnConnectedToMaster();

        // Log the connection details
        LogConnectionDetails();
        
        UpdateStatus(true);
    }
    
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        UpdateStatus(false);
    }
    
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom(); // Call this method when 'Join' button is clicked
    }

    public void CreateRoom()
    {
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true
        };
        PhotonNetwork.CreateRoom("Session 1", options); // Passing null for a random room name
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(); // Call this method when 'Leave' button is clicked
    }

private void DisplayRoomOptions(RoomOptions options)
{
    // Clear existing fields if necessary
    foreach (Transform child in statusArea)
    {
        Destroy(child.gameObject);
    }

    // Define the starting Y position and the height of each text field (including spacing)
    float startPositionY = -66f;
    float textFieldHeight = 30f; // Adjust this value based on the actual height of your text fields + desired spacing

    // Instantiate new text fields for each option, adjusting the Y position for each
    //InstantiateTextField("Max Players: " + options.Name, ref startPositionY, textFieldHeight);
    InstantiateTextField("Max Players: " + options.MaxPlayers, ref startPositionY, textFieldHeight);
}


private void InstantiateTextField(string text, ref float startPositionY, float textFieldHeight)
{
    GameObject textField = Instantiate(textFieldPrefab, statusArea);
    TextMeshProUGUI buttonText = textField.transform.Find("Text").GetComponent<TextMeshProUGUI>();
    if (buttonText.text != null)
    {
        buttonText.text = text;
    }
    else
    {
        Debug.LogError("TextMeshProUGUI component not found on the prefab.");
        return; // Exit if the text component is not found to avoid null reference on the following lines
    }

    // Adjust the RectTransform to position the text field correctly
    RectTransform rectTransform = textField.GetComponent<RectTransform>();
    if (rectTransform != null)
    {
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startPositionY);
        startPositionY -= textFieldHeight +10f; // Move up the start position for the next text field
    }
    else
    {
        Debug.LogError("RectTransform component not found on the prefab.");
    }
}
    
    private void LogConnectionDetails()
    {
        string message = "Connected to Photon Server. " +
                        "\nRegion: " + PhotonNetwork.CloudRegion +
                        "\nServer Address: " + PhotonNetwork.ServerAddress +
                        "\nNetwork Client State: " + PhotonNetwork.NetworkClientState;

        // Log the message to the Unity Console
        Debug.Log(message);

        // Optionally, update a text field or similar in your UI with the connection details
        if (statusText != null)
        {
            statusText.text += "\n" + message;
        }
    }
    
        public override void OnCreatedRoom()
    {
        base.OnCreatedRoom(); // Call the base method

        // Get the current room
        Room currentRoom = PhotonNetwork.CurrentRoom;

        // Log the room details
        if (currentRoom != null)
        {
            string roomDetails = $"Room Created: Name: {currentRoom.Name}, " +
                                 $"Open: {currentRoom.IsOpen}, " +
                                 $"Max Players: {currentRoom.MaxPlayers}";
            Debug.Log(roomDetails);
        }
        
        float startPositionY = -26f;
        float textFieldHeight = 30f;
        InstantiateTextField("Session Name: " + currentRoom.Name, ref startPositionY, textFieldHeight);
        InstantiateTextField("Max Players: " + currentRoom.MaxPlayers, ref startPositionY, textFieldHeight);
        //DisplayRoomOptions(currentRoom.MaxPlayers); // Call this method to display room options   

    }

    // This function is called when room creation fails
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);

        // Log the failure details
        Debug.LogError($"Room creation failed: {message}");
    }

}