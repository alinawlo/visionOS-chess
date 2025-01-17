using System;
using Unity.VisualScripting;
using UnityEngine;

namespace PolySpatial.Samples
{
    [RequireComponent(typeof(Rigidbody))]
    public class PieceSelectionBehavior : MonoBehaviour
    {
        public MeshRenderer m_MeshRenderer;

        public Material m_DefaultMat;

        public Material m_SelectedMat;
        
        [Header("Indicator")]
        public bool enableIndidicator;
        public LayerMask collisionLayer;
        public GameObject indicatorPrefab;
        Transform indicator;
        bool isIndicating;

        Rigidbody m_Rigidbody;
        GameObject m_TempParent;

        Vector3 oldPosition = Vector3.zero;
        Quaternion oldRotation = Quaternion.identity;
        Vector3 linearVelocityChange = Vector3.zero;
        Vector3 angularVelocityChange = Vector3.zero;

        void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();

            m_TempParent = new GameObject("tempParent_" + gameObject.name);
            m_TempParent.transform.position = transform.position;
        }

        void Update(){
            
            if(isIndicating)
            {
                DrawProjection();
            }

            // Calculate linear velocity change
            linearVelocityChange = (transform.position - oldPosition) / Time.deltaTime;

            // Calculate angular velocity change
            Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(oldRotation);
            float theta = 2 * Mathf.Acos(Mathf.Clamp(deltaRotation.w, -1f, 1f));
            angularVelocityChange = new Vector3(deltaRotation.x, deltaRotation.y, deltaRotation.z).normalized * (theta / Time.deltaTime);

            oldRotation = transform.rotation;
            oldPosition = transform.position;
        }

        public void Select(bool selected, Vector3 interactionPosition)
        {
            Select(selected, interactionPosition, Quaternion.identity);
        }
        public void Select(bool selected, Vector3 interactionPosition, Quaternion rotation)
        {
            m_TempParent.transform.position = interactionPosition;
            m_TempParent.transform.rotation = rotation;
            transform.SetParent(selected ? m_TempParent.transform : null);
            m_MeshRenderer.material = selected ? m_SelectedMat : m_DefaultMat;
            m_Rigidbody.isKinematic = selected;
            if(enableIndidicator)
            {
                isIndicating = selected;
                if(!selected && indicator){
                    Destroy(indicator.gameObject);
                }
            }
            if (!selected) {
                ApplyDesiredMotion();
            }
        }

        void DrawProjection() 
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position + transform.up * 0.01f, Vector3.down, out hit, 10, collisionLayer)){
                if(!indicator){
                    indicator = Instantiate(indicatorPrefab).transform;
                }
                indicator.transform.position = hit.point;
            }
            else {
                Destroy(indicator.gameObject);
            }
        }

        void ApplyDesiredMotion()
        {
            // Calculate linear force needed
            Vector3 force = linearVelocityChange * m_Rigidbody.mass;

            // Calculate angular torque needed
            Vector3 torque = angularVelocityChange * m_Rigidbody.inertiaTensor.magnitude;

            // Apply the calculated force and torque to the object
            m_Rigidbody.AddForce(force, ForceMode.Impulse);
            m_Rigidbody.AddTorque(torque, ForceMode.Impulse);
        }
    }
}
