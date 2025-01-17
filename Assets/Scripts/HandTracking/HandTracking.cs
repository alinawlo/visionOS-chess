// Take data from a string reference and transform it into hand tracking values

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UDPReceiver))]
public class HandTracking : MonoBehaviour
{
    public StringReference trackingData;

    public bool isTracking = false;

    public float pinchThreshold = 0.05f;

    public Transform handOrigin;
    
    [Range(0f, 1f)]
    public float smoothingFactor = 0.5f;
    
    public Vector3 offset;
    public Vector3 orientation;
    public float handSize;
    public int handPointReferenceAIndex;
    public int handPointReferenceBIndex;

    private Vector3[] handPointsRaw = new Vector3[21];
    public GameObject[] handPoints;

    private UDPHandTrackingDevice handTracker;
    private void Start() {
        handTracker = InputSystem.AddDevice<UDPHandTrackingDevice>();
    }

    private void OnDisable() {
        InputSystem.RemoveDevice(handTracker);
    }

    // Update is called once per frame
    void Update()
    {
        string data = trackingData.value;
        if(data == "") {
            isTracking = false;
            return;
        }
        isTracking = true;

        // Remove brackets
        data = data.Remove(0, 1);
        data = data.Remove(data.Length-1, 1);
        
        // Split point values
        string[] points = data.Split(",");

        // Assign position of points
        for(int i = 0; i<21; i++){
            float x = float.Parse(points[i*3]) / 1000;
            float y = float.Parse(points[i*3 + 1]) / 1000;
            float z = float.Parse(points[i*3 + 2]) / 1000;

            handPointsRaw[i] = Vector3.Scale(new Vector3(x,y,z) + offset, orientation);
        }
        for(int i = 0; i<21; i++){
            Vector3 calculatedPosition = fixedHandSize(handPointsRaw[i], handSize, handPointsRaw[handPointReferenceAIndex], handPointsRaw[handPointReferenceBIndex]);
            handPoints[i].transform.localPosition = Vector3.Lerp(handPoints[i].transform.localPosition, calculatedPosition, smoothingFactor);
            
        }
    }

    private Vector3 fixedHandSize(Vector3 targetPoint, float fixedSize, Vector3 referencePointA, Vector3 referencePointB){
        float referenceDistance = Vector3.Distance(referencePointA, referencePointB);
        
        if(referenceDistance == 0) return targetPoint;

        float referenceScaling = fixedSize/referenceDistance;
        return referencePointA + (targetPoint - referencePointA) * referenceScaling + new Vector3(0,0,1-referenceScaling*2);
    }
}
