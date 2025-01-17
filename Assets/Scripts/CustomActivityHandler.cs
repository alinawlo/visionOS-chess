using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomActivityHandler : MonoBehaviour
{
    public bool hideOnAwake = false;
    void Awake()
    {
        if(hideOnAwake)
            gameObject.SetActive(false);
    }

    public void ToggleActivity(){
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
