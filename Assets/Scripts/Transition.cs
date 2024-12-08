using UnityEngine;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace WorldSpaceTransitions
{
    [ExecuteInEditMode]
    public class Transition : MonoBehaviour
    {
        [System.Serializable]
        public class TransitionEvent : UnityEvent<Vector3> { }

        public TransitionEvent OnForwardTransitionTriggered;
        public TransitionEvent OnBackwardTransitionTriggered;

        private Material[] allMats;
        public float radius_max = 3;
        public float radius_editor = 1;
        public float fwdInterval = 1.5f;
        public float bwdInterval = 0.6f;
        private bool coroutineIsRunning = false;

        private Vector3 backCenter;
        private Vector3 frontCenter;

        void Awake()
        {
            Shader.DisableKeyword("FADE_PLANE");
            Shader.DisableKeyword("FADE_SPHERE");

            if (OnForwardTransitionTriggered == null)
                OnForwardTransitionTriggered = new TransitionEvent();
            if (OnBackwardTransitionTriggered == null)
                OnBackwardTransitionTriggered = new TransitionEvent();

            OnForwardTransitionTriggered.AddListener(StartForwardTransition);
            OnBackwardTransitionTriggered.AddListener(StartBackwardTransition);

            SceneManager.sceneLoaded += OnSceneLoaded;
            CalculateTransitionPoints();
        }

        void OnEnable()
        {
            Shader.DisableKeyword("FADE_PLANE");
            Shader.DisableKeyword("FADE_SPHERE");

            allMats = new Material[0];
            Renderer[] allRenderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in allRenderers)
            {
                Material[] mats = r.sharedMaterials;
                allMats = allMats.Union(mats).ToArray();
            }

            foreach (Material m in allMats)
            {
                m.DisableKeyword("FADE_PLANE");
                m.SetInt("_FADE_PLANE", 0);
            }

            if (!Application.isPlaying)
            {
                foreach (Material m in allMats)
                {
                    m.SetVector("_SectionPoint", transform.position);
                    m.SetFloat("_Radius", radius_editor);
                    m.EnableKeyword("FADE_SPHERE");
                    m.SetInt("_FADE_SPHERE", 1);
                }
            }
        }

        void LateUpdate()
        {
            CalculateTransitionPoints();
        }

        void OnDisable()
        {
            foreach (Material m in allMats)
            {
                m.DisableKeyword("FADE_SPHERE");
                m.SetInt("_FADE_SPHERE", 0);
            }
            StopAllCoroutines();
            coroutineIsRunning = false;

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"Scene '{scene.name}' loaded with mode {mode}");

            if (scene.name == "Cave_Cooking") // Replace with your scene name
            {
                Debug.Log("Specific scene loaded. Perform custom actions.");
                StartCoroutine(HandleSceneTransition());
            }
        }

        IEnumerator HandleSceneTransition()
        {
            TriggerForwardTransition();
            yield return new WaitForSeconds(bwdInterval);
            gameObject.SetActive(false);
        }

        private void CalculateTransitionPoints()
        {
            Collider collider = GetComponent<Collider>();
            if (collider == null)
            {
                Debug.LogError("No Collider attached to the object!");
                return;
            }

            Bounds bounds = collider.bounds;
            Vector3 forwardDirection = transform.forward.normalized;

            backCenter = bounds.center - forwardDirection * bounds.extents.z;
            frontCenter = bounds.center + forwardDirection * bounds.extents.z;
        }

        public void TriggerForwardTransition()
        {
            OnForwardTransitionTriggered.Invoke(backCenter);
        }

        public void TriggerBackwardTransition()
        {
            Debug.Log("Triggering backward transition.");
            OnBackwardTransitionTriggered.Invoke(frontCenter);
        }

        private void StartForwardTransition(Vector3 hitPoint)
        {
            if (!coroutineIsRunning)
            {
                StartCoroutine(doTransition(hitPoint, true));
            }
        }

        private void StartBackwardTransition(Vector3 hitPoint)
        {
            if (!coroutineIsRunning)
            {
                StartCoroutine(doTransition(hitPoint, false));
            }
        }

        IEnumerator doTransition(Vector3 initialHitPoint, bool isForward)
        {
            coroutineIsRunning = true;
            float startTime = Time.time;
            float t = 0f;

            while (t < 1)
            {
                Vector3 currentHitPoint = isForward ? backCenter : frontCenter;

                foreach (Material m in allMats)
                {
                    m.SetVector("_SectionPoint", currentHitPoint);
                    m.SetFloat("_Radius", Mathf.SmoothStep(isForward ? 0 : radius_max, isForward ? radius_max : 0, t));
                    m.EnableKeyword("FADE_SPHERE");
                    m.SetInt("_FADE_SPHERE", 1);
                }

                t = (Time.time - startTime) / (isForward ? fwdInterval : bwdInterval);
                yield return null;
            }

            foreach (Material m in allMats)
            {
                m.DisableKeyword("FADE_SPHERE");
                m.SetInt("_FADE_SPHERE", 0);
            }

            coroutineIsRunning = false;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(backCenter, 0.1f); // Back side
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(frontCenter, 0.1f); // Front side
        }
    }
}
