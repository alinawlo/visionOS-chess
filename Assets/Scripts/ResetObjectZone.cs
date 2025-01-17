using UnityEngine;

namespace PolySpatial.Samples
{
    class ResetObjectZone : MonoBehaviour
    {
        [SerializeField]
        Transform m_RespawnPosition;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out PieceSelectionBehavior piece))
            {
                var pieceTransform = piece.transform;
                var pieceRigidbody = pieceTransform.GetComponent<Rigidbody>();
                pieceRigidbody.isKinematic = true;
                pieceTransform.position = m_RespawnPosition.position;
                pieceTransform.rotation = m_RespawnPosition.rotation;
                pieceRigidbody.isKinematic = false;
            }
        }
    }
}
