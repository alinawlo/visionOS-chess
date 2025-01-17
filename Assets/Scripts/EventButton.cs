using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolySpatial.Samples;
using UnityEngine.Events;

public class EventButton : HubButton
{
    public UnityEvent onClick;
    public override void Press()
    {
        base.Press();
        onClick.Invoke();
    }
}
