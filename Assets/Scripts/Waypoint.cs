using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint nextWaypoint; // Ссылка на следующий Waypoint
    public bool isIdlePoint;      // Указывает, нужно ли переходить в Idle на этом Waypoint
}