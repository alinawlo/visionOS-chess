using System.Collections;
using System.Collections.Generic;
using PolySpatial.Samples;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandInputManager : MonoBehaviour
{
    [SerializeField]
    InputActionReference handRotation;
    [SerializeField]
    InputActionReference pinch;
    [SerializeField]
    InputActionReference pinchPosition;
    [SerializeField]
    InputActionReference point;
    [SerializeField]
    InputActionReference pointPosition;

    List<PieceSelectionBehavior> pinchedPieces = new List<PieceSelectionBehavior>();
    List<SpatialUI> pointedPieces = new List<SpatialUI>();
    List<SpatialUI> pointedPiecesNew = new List<SpatialUI>();
    List<HubButton> pointedHub = new List<HubButton>();
    List<HubButton> pointedHubNew = new List<HubButton>();

    bool isPinching = false;
    bool isPointing = false;

    public float interactionRadius = 0.1f;

    void OnEnable()
    {
        handRotation.action.Enable();
        pinch.action.Enable();
        pinchPosition.action.Enable();
        point.action.Enable();
        pointPosition.action.Enable();
    }


    // Update is called once per frame
    void Update()
    {
        Pinching();
        Pointing();
    }

    void Pointing(){
        if(point.action.ReadValue<float>() > 0){
            Collider[] hits = Physics.OverlapSphere(pointPosition.action.ReadValue<Vector3>(), interactionRadius);
            pointedPiecesNew = new List<SpatialUI>();
            pointedHubNew = new List<HubButton>();
            foreach (Collider hit in hits){

                // point
                if (hit.TryGetComponent(out SpatialUI point))
                {
                    pointedPiecesNew.Add(point);
                    if(!pointedPieces.Contains(point)){
                        point.Press(pointPosition.action.ReadValue<Vector3>());
                    }
                }
                if (hit.TryGetComponent(out HubButton pointHub))
                {
                    pointedHubNew.Add(pointHub);
                    if(!pointedHub.Contains(pointHub)){
                        pointHub.Press();
                    }
                }
            }
            pointedPieces = pointedPiecesNew;
            pointedHub = pointedHubNew;
        }
        else {
            pointedPieces = new List<SpatialUI>();
            pointedHub = new List<HubButton>();
        }
    }

    void Pinching(){
        if(pinch.action.ReadValue<float>() > 0){
            if(!isPinching){
                isPinching = true;

                Collider[] hits = Physics.OverlapSphere(pinchPosition.action.ReadValue<Vector3>(), interactionRadius);
                foreach (Collider hit in hits){
                    // pinch enter
                    if (hit.TryGetComponent(out PieceSelectionBehavior piece))
                    {
                        if(!pinchedPieces.Contains(piece)){
                            pinchedPieces.Add(piece);
                        }
                    } 
                }
            }
        }
        else {
            isPinching = false;
            foreach(PieceSelectionBehavior piece in pinchedPieces){
                piece.Select(false, pinchPosition.action.ReadValue<Vector3>(),handRotation.action.ReadValue<Quaternion>());
            }
            pinchedPieces = new List<PieceSelectionBehavior>();
        }

        foreach(PieceSelectionBehavior piece in pinchedPieces){
            piece.Select(true, pinchPosition.action.ReadValue<Vector3>(), handRotation.action.ReadValue<Quaternion>());
        }
    }
}
