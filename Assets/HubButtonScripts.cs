using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubButtonScripts : MonoBehaviour
{
    // Start is called before the first frame update
    public void ToggleHand(){
        var toggle = GameObject.FindObjectOfType<UDPReceiver>();
        if (toggle != null) toggle.ToggleConnection();
    }

    public void UnloadScene(){
        var home = GameObject.Find("Home Menu");
        if(home != null) {
            foreach(Transform child in home.transform){
                child.gameObject.SetActive(true);
            }
        }
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
    }
}
