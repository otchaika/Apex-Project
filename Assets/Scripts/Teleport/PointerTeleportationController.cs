using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationWithPointer : MonoBehaviour
{
    [Header("Pointer Settings")]
    [SerializeField] private GameObject pointerPrefab;
    [SerializeField] private Transform targetAnchor;
    [SerializeField] private float pointerDistance = 0.3f;
    [SerializeField] private float lookThreshold = 0.9f;

    [Header("Teleportation Settings")]
    [SerializeField] private TeleportationAnchor teleportAnchor;
    [SerializeField] private Transform xrOrigin;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] public float fadeDuration = 1.0f;

    [Header("Planes")]
    [SerializeField] private Transform currentPlane;
    [SerializeField] private Transform targetPlane;
    [SerializeField] private Transform anchor;

    [Header("Options")]
    [SerializeField] private bool enableRotation = true;

    [Header("Input Settings")]
    [SerializeField] private InputActionReference teleportActivationButton;

    private Transform pointerInstance;
    private bool isCameraInTrigger = false;

    private void Awake()
    {
        StartCoroutine(FadeOut());
    }

    private void OnEnable()
    {
        teleportActivationButton.action.started += OnTeleportButtonPressed;
    }

    private void OnDisable()
    {
        teleportActivationButton.action.started -= OnTeleportButtonPressed;
    }

    private void Update()
    {
        if (isCameraInTrigger)
        {
            UpdatePointer();
        }
        else
        {
            DestroyPointer();
        }
    }

    private void UpdatePointer()
    {
        if (targetAnchor == null || pointerPrefab == null || Camera.main == null)
            return;

        Transform cameraTrans = Camera.main.transform;
        Vector3 cameraPosition = cameraTrans.position;
        Vector3 cameraForward = cameraTrans.forward;
        Vector3 targetPosition = targetAnchor.position;

        Vector3 directionToTarget = (targetPosition - cameraPosition).normalized;
        bool isLookingAtTarget = Vector3.Dot(cameraForward, directionToTarget) >= lookThreshold;

        if (isLookingAtTarget)
        {
            if (pointerInstance == null)
            {
                pointerInstance = Instantiate(pointerPrefab).transform;
            }

            Vector3 directionInScreenSpace = cameraTrans.InverseTransformDirection(directionToTarget);
            directionInScreenSpace.z = 0f;
            Vector3 pointerDirection = cameraTrans.TransformDirection(directionInScreenSpace).normalized;

            pointerInstance.position = targetPosition - pointerDirection * pointerDistance;
            pointerInstance.rotation = Quaternion.LookRotation(pointerDirection, -cameraTrans.forward);
        }
        else
        {
            DestroyPointer();
        }
    }

    private void DestroyPointer()
    {
        if (pointerInstance != null)
        {
            Destroy(pointerInstance.gameObject);
            pointerInstance = null;
        }
    }

    private void OnTeleportButtonPressed(InputAction.CallbackContext context)
    {
        if (pointerInstance != null && isCameraInTrigger)
        {
            StartCoroutine(PerformTeleport());
        }
    }

    private IEnumerator PerformTeleport()
    {
        Debug.Log("Teleport started");
        yield return StartCoroutine(FadeIn());
        
        yield return new WaitForSeconds(0.5f);
        
        Vector3 localPlayerPositionBefore = currentPlane.InverseTransformPoint(xrOrigin.position);
        Vector3 worldPositionOnTargetPlane = targetPlane.TransformPoint(localPlayerPositionBefore);
        anchor.position = worldPositionOnTargetPlane;

        float currentPlayerRotationY = GetRelativeRotationY(xrOrigin, currentPlane);
        float targetRotationY = targetPlane.eulerAngles.y + currentPlayerRotationY;

        Vector3 destinationPosition = teleportAnchor.transform.position;

        CharacterController controller = xrOrigin.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        xrOrigin.position = destinationPosition;
        if (enableRotation) ApplyRotation(targetRotationY);

        if (controller != null) controller.enabled = true;

        yield return StartCoroutine(FadeOut());
    }

    private float GetRelativeRotationY(Transform xrOrigin, Transform plane)
    {
        return xrOrigin.eulerAngles.y - plane.eulerAngles.y;
    }

    private void ApplyRotation(float targetRotationY)
    {
        Vector3 currentEulerAngles = xrOrigin.eulerAngles;
        xrOrigin.rotation = Quaternion.Euler(currentEulerAngles.x, targetRotationY, currentEulerAngles.z);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            isCameraInTrigger = true;
            Debug.Log("Camera entered trigger zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            isCameraInTrigger = false;
            Debug.Log("Camera exited trigger zone");
            DestroyPointer();
        }
    }
    
    public IEnumerator FadeIn()
    {
        Debug.Log("Запуск FadeIn");
        float elapsedTime = 0;
        canvasGroup.blocksRaycasts = true; 
        while (canvasGroup.alpha < 1)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        Debug.Log("FadeIn завершён");
    }

    public IEnumerator FadeOut()
    {
        Debug.Log("Запуск FadeOut");
        float elapsedTime = 0;
        while (canvasGroup.alpha > 0)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1 - elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.blocksRaycasts = false; 
        Debug.Log("FadeOut завершён");
    }
}