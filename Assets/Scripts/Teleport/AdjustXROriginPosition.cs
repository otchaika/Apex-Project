using UnityEngine;

public class AdjustXROriginPosition : MonoBehaviour
{
    public Transform XROrigin;  // XR Origin (родительская трансформация для VR-системы)
    public Transform MainCamera; // Main Camera (камера игрока)

    public void Update()
    {
        AdjustPositionToCamera();
    }

    public void AdjustPositionToCamera()
    {
        if (XROrigin != null && MainCamera != null)
        {
            // Получаем локальную позицию камеры относительно XR Origin
            Vector3 cameraLocalPosition = XROrigin.InverseTransformPoint(MainCamera.position);

            // Рассчитываем смещение XR Origin по x и z, оставляя y без изменений
            Vector3 originPosition = XROrigin.position;
            originPosition.x += cameraLocalPosition.x;
            originPosition.z += cameraLocalPosition.z;

            // Обновляем позицию XR Origin
            XROrigin.position = originPosition;

            // Сбрасываем локальную позицию камеры относительно XR Origin
            MainCamera.localPosition = new Vector3(0, MainCamera.localPosition.y, 0);

            Debug.Log($"XR Origin перемещён на новую позицию: {XROrigin.position}");
        }
        else
        {
            Debug.LogWarning("Не назначены XR Origin или Main Camera!");
        }
    }
}