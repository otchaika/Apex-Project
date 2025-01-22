using UnityEngine;

public class TeleportAnchorToCorrespondingPoint : MonoBehaviour
{
    [Header("Plane Settings")]
    public Transform currentPlane; // Текущая плоскость, на которой находится игрок
    public Transform targetPlane;  // Целевая плоскость, куда нужно переместить Anchor

    [Header("Anchor Settings")]
    public Transform anchor; // Anchor, который нужно перемещать

    [Header("Player Settings")]
    public Transform xrOrigin; // XR Origin (положение игрока)

    public void TeleportAnchor()
    {
        if (currentPlane == null || targetPlane == null || anchor == null || xrOrigin == null)
        {
            Debug.LogWarning("Не все объекты назначены!");
            return;
        }

        // Позиция игрока в локальных координатах текущей плоскости
        Vector3 localPlayerPosition = currentPlane.InverseTransformPoint(xrOrigin.position);

        // Преобразуем локальную позицию игрока на текущей плоскости в мировые координаты
        Vector3 worldPositionOnTargetPlane = targetPlane.TransformPoint(localPlayerPosition);

        // Преобразуем мировые координаты в локальные координаты целевой плоскости
        Vector3 localPositionOnTargetPlane = targetPlane.InverseTransformPoint(worldPositionOnTargetPlane);

        // Перемещаем Anchor в вычисленную локальную позицию на целевой плоскости
        anchor.localPosition = localPositionOnTargetPlane;

        Debug.Log($"Anchor перемещён в точку {localPositionOnTargetPlane} на целевой плоскости.");
    }
}