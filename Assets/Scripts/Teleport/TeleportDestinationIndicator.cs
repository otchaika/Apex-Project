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

        [SerializeField]
        [Tooltip("The minimum dot product threshold to show the pointer (1 means looking directly at the anchor).")]
        private float m_LookThreshold = 0.9f; // Чем ближе к 1, тем точнее взгляд должен быть направлен на anchor

        private Transform m_PointerInstance;

        private void Update()
        {
            UpdatePointer();
        }

        private void UpdatePointer()
        {
            if (m_Anchor == null || m_PointerPrefab == null || Camera.main == null)
                return;

            var cameraTrans = Camera.main.transform;
            var cameraPosition = cameraTrans.position;
            var cameraForward = cameraTrans.forward;
            var anchorPosition = m_Anchor.position;

            // Вектор от камеры к anchor
            var directionToAnchor = (anchorPosition - cameraPosition).normalized;

            // Проверяем, достаточно ли близко камера смотрит на anchor
            bool isLookingAtAnchor = Vector3.Dot(cameraForward, directionToAnchor) >= m_LookThreshold;

            if (isLookingAtAnchor)
            {
                if (m_PointerInstance == null)
                {
                    // Создаем pointer, если он еще не создан
                    m_PointerInstance = Instantiate(m_PointerPrefab).transform;
                }

                // Вычисляем направление pointer
                var directionInScreenSpace = cameraTrans.InverseTransformDirection(directionToAnchor);
                directionInScreenSpace.z = 0f;
                var pointerDirection = cameraTrans.TransformDirection(directionInScreenSpace).normalized;

                // Размещаем pointer
                m_PointerInstance.position = anchorPosition - pointerDirection * m_PointerDistance;
                m_PointerInstance.rotation = Quaternion.LookRotation(pointerDirection, -cameraTrans.forward);
            }
            else if (m_PointerInstance != null)
            {
                // Удаляем pointer, если он есть, но камера не смотрит на anchor
                Destroy(m_PointerInstance.gameObject);
                m_PointerInstance = null;
            }
        }
    }
}
