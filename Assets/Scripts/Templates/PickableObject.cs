using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolySpatial.Samples;
using Unity.PolySpatial;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PieceSelectionBehavior))]
[RequireComponent(typeof(PolySpatialHoverEffect))]
[RequireComponent(typeof(SaveableObject))]
public class PickableObject : MonoBehaviour
{
    [SerializeField]
    public Material material;
    public string prefabLocation;
    [ContextMenu("Set Default Values")]
    void SetDefaultValues(){
        if(!material) {
            Debug.LogError("Missing Material");
            return;
        }

        if(prefabLocation == ""){
            Debug.LogError("Missing Prefab Location");
            return;
        }

        GetComponent<MeshCollider>().convex = true;

        PieceSelectionBehavior psb = GetComponent<PieceSelectionBehavior>();
        psb.m_MeshRenderer = GetComponent<MeshRenderer>();

        psb.m_DefaultMat = material;
        psb.m_SelectedMat = material;

        GetComponent<SaveableObject>().prefabLocation = prefabLocation;
    }

    [ContextMenu("Disable Rotaion")]
    void DisableRotaion(){
        Rigidbody rig = GetComponent<Rigidbody>();
        rig.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
    }
    [ContextMenu("Enable Rotaion")]
    void EnableRotaion(){
        Rigidbody rig = GetComponent<Rigidbody>();
        rig.constraints = RigidbodyConstraints.None;
    }
}
