using UnityEngine;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Controls a pointer object that points to an anchor set in the inspector.
    /// </summary>
    public class PointerAnchorController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The prefab of the pointer object to display.")]
        private GameObject m_PointerPrefab;

        /// <summary>
        /// The prefab of the pointer object to display.
        /// </summary>
        public GameObject pointerPrefab
        {
            get => m_PointerPrefab;
            set => m_PointerPrefab = value;
        }

        [SerializeField]
        [Tooltip("The anchor that the pointer will point to.")]
        private Transform m_Anchor;

        /// <summary>
        /// The anchor that the pointer will point to.
        /// </summary>
        public Transform anchor
        {
            get => m_Anchor;
            set
            {
                m_Anchor = value;
                UpdatePointer();
            }
        }

        [SerializeField]
        [Tooltip("The distance from the anchor at which the pointer will be placed.")]
        private float m_PointerDistance = 0.3f;

        /// <summary>
        /// The distance from the anchor at which the pointer will be placed.
        /// </summary>
        public float pointerDistance
        {
            get => m_PointerDistance;
            set
            {
                m_PointerDistance = value;
                UpdatePointer();
            }
        }

        private Transform m_PointerInstance;

        private void OnEnable()
        {
            UpdatePointer();
        }

        private void Start()
        {
            UpdatePointer();
        }

        private void UpdatePointer()
        {
            // Destroy existing pointer instance if it exists.
            if (m_PointerInstance != null)
            {
                Destroy(m_PointerInstance.gameObject);
                m_PointerInstance = null;
            }

            // Check if anchor and prefab are set.
            if (m_Anchor == null || m_PointerPrefab == null)
                return;

            // Instantiate the pointer prefab.
            m_PointerInstance = Instantiate(m_PointerPrefab).transform;

            // Get camera transform for orientation.
            var cameraTrans = Camera.main.transform;
            var cameraPosition = cameraTrans.position;
            var anchorPosition = m_Anchor.position;

            // Calculate direction from camera to anchor, ignoring depth.
            var directionInScreenSpace = cameraTrans.InverseTransformDirection(anchorPosition - cameraPosition);
            directionInScreenSpace.z = 0f;
            var pointerDirection = cameraTrans.TransformDirection(directionInScreenSpace).normalized;

            // Position and orient the pointer.
            m_PointerInstance.position = anchorPosition - pointerDirection * m_PointerDistance;
            m_PointerInstance.rotation = Quaternion.LookRotation(pointerDirection, -cameraTrans.forward);
        }

        private void OnDisable()
        {
            // Clean up the pointer instance when the component is disabled.
            if (m_PointerInstance != null)
            {
                Destroy(m_PointerInstance.gameObject);
                m_PointerInstance = null;
            }
        }
    }
}
