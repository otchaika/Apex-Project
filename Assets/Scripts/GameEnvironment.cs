using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameEnvironment : MonoBehaviour
{
    private List<GameObject> waypoints = new List<GameObject>();
    public List<int> IdleWaypoints = new List<int>(); // Индексы точек для Idle

    public List<GameObject> Waypoints { get { return waypoints; } }

    public static GameEnvironment Instance { set; get; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            RefreshWaypoints(); // Обновляем waypoints при создании
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void RefreshWaypoints()
    {
        waypoints.Clear();
        waypoints.AddRange(GameObject.FindGameObjectsWithTag("Waypoint"));
        waypoints = waypoints.OrderBy(waypoint => waypoint.name).ToList();

        Debug.Log("Waypoints refreshed:");
        for (int i = 0; i < waypoints.Count; i++)
        {
            Debug.Log($"Waypoint {i}: {waypoints[i].name} (Position: {waypoints[i].transform.position})");
        }

        // Пример: Настраиваем точки Idle
        IdleWaypoints.Clear();
        IdleWaypoints.Add(2); // Указываем, что 2-й waypoint вызывает Idle
        IdleWaypoints.Add(5); // Указываем, что 5-й waypoint вызывает Idle
        Debug.Log($"Idle waypoints set: {string.Join(", ", IdleWaypoints)}");
    }

}