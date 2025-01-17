using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolySpatial.Samples;
using Unity.PolySpatial;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolySpatialHoverEffect))]
[RequireComponent(typeof(BoxCollider))]
public class CustomSpatialUIButton : SpatialUIButton
{
    public TMPro.TextMeshPro text;
    public UnityEvent pressEvent;

    // private string buttonText;

    // public string ButtonText
    // {
    //     get { return buttonText; }
    //     set
    //     {
    //         if (text != null && buttonText != value)
    //         {
    //             buttonText = value;
    //             text.text = buttonText;
    //         }
    //     }
    // }

    // Update is called once per frame
    void Update()
    {
        if(text && ButtonText != ""){
            text.text = ButtonText;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        WasPressed += InvokeEvent;
    }

    void OnDisable(){
        WasPressed -= InvokeEvent;
    }

    void InvokeEvent(string buttonText, MeshRenderer meshrenderer)
    {
        pressEvent.Invoke();
    }
}
