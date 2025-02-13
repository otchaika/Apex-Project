using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Teleportation : MonoBehaviour
{
    FadeIn_Out fadeIn_Out;
    [Header("Teleportation Settings")]
    public TeleportationAnchor TeleportAnchor; // Точка телепортации
    public TeleportationProvider TeleportProvider; // Провайдер телепортации
    public Transform XROrigin; // XR Origin для управления позицией и ротацией
    public CanvasGroup CanvasGroup;

    [Header("Planes")]
    public Transform CurrentPlane; // Текущая плоскость, на которой находится игрок
    public Transform TargetPlane;  // Целевая плоскость для перемещения Anchor

    [Header("Anchor Settings")]
    public Transform Anchor; // Anchor, который нужно перемещать

    [Header("Options")]
    public bool EnableRotation = true; // Включить поворот при телепортации

    [Header("Input Settings")]
    public InputActionReference TeleportActivationButton; // Ссылка на Input Action для активации телепортации



    private void OnEnable()
    {
        TeleportActivationButton.action.started += OnTeleportButtonPressed;
    }

    private void OnDisable()
    {
        TeleportActivationButton.action.started -= OnTeleportButtonPressed;
    }

    private void OnTeleportButtonPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Teleport button pressed");

        if (TeleportAnchor != null && TeleportProvider != null && XROrigin != null && CurrentPlane != null && TargetPlane != null && Anchor != null)
        {
            FadeInThenTeleport();
            //StartCoroutine(FadeInThenTeleport());
        }
        else
        {
            Debug.LogWarning("Не все необходимые объекты назначены!");
        }
    }

    private void FadeInThenTeleport()
    {
        // Запуск плавного появления
       // yield return StartCoroutine(FadeIn());
       
        // После завершения FadeIn продолжаем телепортацию
        Debug.Log("FadeIn завершён, начинаем телепортацию...");

        // Сохраняем текущую позицию игрока на текущей плоскости
        Vector3 localPlayerPosition = CurrentPlane.InverseTransformPoint(XROrigin.position);

        // Переводим позицию игрока на целевую плоскость
        Vector3 worldPositionOnTargetPlane = TargetPlane.TransformPoint(localPlayerPosition);

        // Устанавливаем Anchor в соответствующую точку на целевой плоскости
        Anchor.position = worldPositionOnTargetPlane;

        Debug.Log($"Anchor перемещён в позицию {Anchor.position} на целевой плоскости.");
        StartCoroutine(fadeIn_Out.FadeIn());

        // Вычисляем ротацию игрока относительно текущей плоскости
        float currentPlayerRotationY = GetRelativeRotationY(XROrigin, CurrentPlane);

        // Вычисляем новую ротацию на целевой плоскости
        float targetRotationY = TargetPlane.eulerAngles.y + currentPlayerRotationY;

        // Перемещаем игрока (XR Origin)
        Vector3 destinationPosition = TeleportAnchor.transform.position;

        TeleportRequest teleportRequest = new TeleportRequest
        {
            destinationPosition = destinationPosition,
            requestTime = Time.time
        };
        TeleportProvider.QueueTeleportRequest(teleportRequest);
        // Применяем новую ротацию
        if (EnableRotation)
        {
            ApplyRotation(targetRotationY);
        }

        // Запускаем плавное исчезновение после телепортации
        //yield return StartCoroutine(FadeOut());
        StartCoroutine(fadeIn_Out.FadeOut());
        Debug.Log($"Телепортация выполнена в позицию {destinationPosition}" +
                  (EnableRotation ? $" с ротацией {targetRotationY}°" : ""));
    }

    private float GetRelativeRotationY(Transform xrOrigin, Transform plane)
    {
        // Вычисляем разницу ротации XR Origin относительно текущей плоскости по оси Y
        float relativeRotationY = xrOrigin.eulerAngles.y - plane.eulerAngles.y;
        return relativeRotationY;
    }

    private void ApplyRotation(float targetRotationY)
    {
        // Устанавливаем новую ротацию XR Origin
        Vector3 currentEulerAngles = XROrigin.eulerAngles;
        XROrigin.rotation = Quaternion.Euler(currentEulerAngles.x, targetRotationY, currentEulerAngles.z);
    }

    private IEnumerator FadeIn()
    {
        while (CanvasGroup.alpha < 1)
        {
            CanvasGroup.alpha += Time.deltaTime; // Скорость затухания
            yield return null; // Ждать следующий кадр
        }
    }

    private IEnumerator FadeOut()
    {
        while (CanvasGroup.alpha > 0)
        {
            CanvasGroup.alpha -= Time.deltaTime; // Скорость затухания
            yield return null; // Ждать следующий кадр
        }
    }
}
