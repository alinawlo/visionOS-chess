using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

public struct UDPHandTrackingDeviceState : IInputStateTypeInfo
{
    // Format code "FABI"
    public FourCC format => new FourCC('U', 'D', 'P', 'H');


    // InputControls
    [InputControl(layout = "Button", name = "isTracked", displayName = "Is Tracked")]
    public float isTracked;

    [InputControl(layout = "Button", name = "pinch", displayName = "Pinch")]
    public float pinch;
    [InputControl(layout = "Button", name = "point", displayName = "Point")]
    public float point;

    [InputControl(usage = "handPosition")]
    public Vector3 handPosition;

    [InputControl(usage = "handRotation")]
    public Quaternion handRotation;

    [InputControl(usage = "pinchPosition")]
    public Vector3 pinchPosition;
    [InputControl(usage = "pointPosition")]
    public Vector3 pointPosition;


}
#if UNITY_EDITOR
[InitializeOnLoad] // Call static class constructor in editor.
#endif
[InputControlLayout(stateType = typeof(UDPHandTrackingDeviceState))]
public class UDPHandTrackingDevice : InputDevice, IInputUpdateCallbackReceiver
{
    #if UNITY_EDITOR
    static UDPHandTrackingDevice()
    {
        // Trigger our RegisterLayout code in the editor.
        Initialize();
    }
    #endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // Register our device with the input system.
        InputSystem.RegisterLayout<UDPHandTrackingDevice>(
            matches: new InputDeviceMatcher()
                .WithInterface("UDPHandTracking"));
    }

    // Expose controls
    public ButtonControl isTracked { get; private set; }
    public ButtonControl pinch { get; private set; }
    public ButtonControl point { get; private set; }
    public Vector3Control handPosition { get; private set; }
    public QuaternionControl handRotation { get; private set; }
    public Vector3Control pinchPosition { get; private set; }
    public Vector3Control pointPosition { get; private set; }

    // Finalize device setup
    protected override void FinishSetup()
    {
        base.FinishSetup();

        isTracked = GetChildControl<ButtonControl>("isTracked");
        pinch = GetChildControl<ButtonControl>("pinch");
        point = GetChildControl<ButtonControl>("point");

        handPosition = GetChildControl<Vector3Control>("handPosition");
        handRotation = GetChildControl<QuaternionControl>("handRotation");
        pinchPosition = GetChildControl<Vector3Control>("pinchPosition");
        pointPosition = GetChildControl<Vector3Control>("pointPosition");
    }

    // Expose current
    public static UDPHandTrackingDevice current { get; private set; }
    public override void MakeCurrent()
    {
        base.MakeCurrent();
        current = this;
    }

    // Make sure current is null when removed
    protected override void OnRemoved()
    {
        base.OnRemoved();
        if (current == this)
            current = null;
    }

    public void OnUpdate()
    {
        var state = new UDPHandTrackingDeviceState
        {
            isTracked = 0,
            pinch = 0,
            point = 0,
            handPosition = Vector3.zero,
            handRotation = Quaternion.identity,
            pinchPosition = Vector3.zero,
            pointPosition = Vector3.zero
        };

        HandTracking handTracker = GameObject.FindObjectOfType<HandTracking>();
        if(handTracker != null && handTracker.isTracking) {
            var handPoints = handTracker.handPoints;

            state.isTracked = 1;
            state.handPosition = handPoints[0].transform.position - handTracker.handOrigin.position;
            state.pinchPosition = (handPoints[4].transform.position + handPoints[8].transform.position) / 2f;
            state.pointPosition = handPoints[8].transform.position;

            var forward = handPoints[9].transform.position - handPoints[0].transform.position;
            var sideways = handPoints[5].transform.position - handPoints[0].transform.position;
            state.handRotation = Quaternion.LookRotation(forward, Vector3.Cross(forward, sideways));

            if (Vector3.Distance(handPoints[8].transform.position, handPoints[4].transform.position) <= handTracker.pinchThreshold){
                state.pinch = 1;
            } else if(
                Vector3.Distance(posOf(handPoints[8]), posOf(handPoints[5])) > Vector3.Distance(posOf(handPoints[7]), posOf(handPoints[5])) &&
                Vector3.Distance(posOf(handPoints[7]), posOf(handPoints[5])) > Vector3.Distance(posOf(handPoints[6]), posOf(handPoints[5])) &&
                Vector3.Distance(posOf(handPoints[8]), posOf(handPoints[5])) < Vector3.Distance(posOf(handPoints[8]), posOf(handPoints[12])) &&
                Vector3.Distance(posOf(handPoints[8]), posOf(handPoints[5])) < Vector3.Distance(posOf(handPoints[8]), posOf(handPoints[16])) &&
                Vector3.Distance(posOf(handPoints[8]), posOf(handPoints[5])) < Vector3.Distance(posOf(handPoints[8]), posOf(handPoints[20]))
            ){
                state.point = 1;
            }
        }

        InputSystem.QueueStateEvent(this, state);
    }

    Vector3 posOf(GameObject obj){
        return obj.transform.position;
    }
}