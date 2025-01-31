using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EventTrigger : MonoBehaviour
{
     public GameEvent @event;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Campfire")||other.CompareTag("CaveWoman")) // Проверяем тег у объекта, который вошел в триггер
        {
            //Debug.Log("Fire");
            @event.Occurred(this.gameObject);
        }
        
    }


}