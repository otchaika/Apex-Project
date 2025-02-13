using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using System.Collections;

public class PlayAreaManager : MonoBehaviour
{
    private Vector2 playAreaSize = Vector2.zero;
    private Quaternion playAreaRotation = Quaternion.identity; // Real-world rotation
    [SerializeField] private Transform playerTransform; // Assign XR Origin or Player
    private const float requiredSize = 3.0f;
    private Quaternion targetGameRotation = Quaternion.Euler(0f, -90f, 0f); // Desired in-game rotation

    void Start()
    {
        StartCoroutine(InitializePlayArea());
    }

    private IEnumerator InitializePlayArea()
    {
        yield return new WaitForSeconds(1.0f); // Ensure XR is initialized

        GetPlayAreaSizeAndRotation();

        if (HasEnoughSpace())
        {
            SetPlayerPositionAndRotation();
        }
        else
        {
            Debug.LogWarning($"Not enough space for a 3x3m area. Available: {playAreaSize.x}m x {playAreaSize.y}m");
        }
    }

    private void GetPlayAreaSizeAndRotation()
    {
        XRInputSubsystem xrInputSubsystem = GetXRInputSubsystem();
        if (xrInputSubsystem == null)
        {
            Debug.LogWarning("No XRInputSubsystem found.");
            return;
        }

        List<Vector3> boundaryPoints = new List<Vector3>();
        if (xrInputSubsystem.TryGetBoundaryPoints(boundaryPoints) && boundaryPoints.Count > 0)
        {
            float minX = float.MaxValue, maxX = float.MinValue;
            float minZ = float.MaxValue, maxZ = float.MinValue;

            foreach (var point in boundaryPoints)
            {
                minX = Mathf.Min(minX, point.x);
                maxX = Mathf.Max(maxX, point.x);
                minZ = Mathf.Min(minZ, point.z);
                maxZ = Mathf.Max(maxZ, point.z);
            }

            playAreaSize = new Vector2(maxX - minX, maxZ - minZ);

            // Calculate the real-world rotation using boundary points
            Vector3 forwardDirection = (boundaryPoints[boundaryPoints.Count - 1] - boundaryPoints[0]).normalized;
            playAreaRotation = Quaternion.LookRotation(new Vector3(forwardDirection.x, 0, forwardDirection.z));

            Debug.Log($"Play Area Size: {playAreaSize.x}m x {playAreaSize.y}m, Real-World Rotation: {playAreaRotation.eulerAngles.y}°");
        }
        else
        {
            Debug.LogWarning("Unable to retrieve boundary points.");
        }
    }

    private XRInputSubsystem GetXRInputSubsystem()
    {
        List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances(subsystems);
        return subsystems.Count > 0 ? subsystems[0] : null;
    }

    private bool HasEnoughSpace()
    {
        return playAreaSize.x >= requiredSize && playAreaSize.y >= requiredSize;
    }

    private void SetPlayerPositionAndRotation()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform not assigned!");
            return;
        }

        // Compute the center of a 3x3m play area inside the available space
        float posX = Mathf.Min(requiredSize / 2, playAreaSize.x / 2);
        float posZ = Mathf.Min(requiredSize / 2, playAreaSize.y / 2);
        Vector3 localSpawnPosition = new Vector3(posX, 0f, posZ);

        // Rotate the position to align with the play area rotation
        Vector3 worldSpawnPosition = playAreaRotation * localSpawnPosition;

        // Apply target game rotation relative to real-world rotation
        Quaternion adjustedRotation = playAreaRotation * targetGameRotation;

        // Set final position & rotation
        playerTransform.position = worldSpawnPosition;
        playerTransform.rotation = adjustedRotation;

        Debug.Log($"Player placed at: {worldSpawnPosition}, Adjusted Rotation: {adjustedRotation.eulerAngles.y}°");
    }
}
