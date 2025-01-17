using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;

public class CustomHandTrackingProvider : XRHandSubsystem
{
    public override UpdateSuccessFlags TryUpdateHands(UpdateType updateType)
    {
        return UpdateSuccessFlags.None;
    }
}