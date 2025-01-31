/////////////////////////////////////////////////////////////////////////////////////////
///
///   Author: Prof. Dr. Frank Gabler, fbmd, Hochschule Darmstadt
///   ER-T2
///   Created: x.05.19
/// 
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.Events;

// we need to create our own event to allow to pass the <GameObject> with it
// (e.g. needed in the Radar to read the position)
[System.Serializable]
public class UnityGameObjectEvent: UnityEvent<GameObject> { }
// this one is derived from momobehavoir
public class EventListener : MonoBehaviour
{
    // this is our assigned event
    public GameEvent gEvent;
    // what is the response to the event. We need to instantiate the event here!
    public UnityGameObjectEvent response = new UnityGameObjectEvent();

    private void OnEnable()
    {
        gEvent.Register(this);
    }

    private void OnDisable()
    {
        gEvent.Unregister(this);
    }

    public void OnEventOccurs(GameObject go)
    {
        // fire the UnityEvent via the attached response
        response.Invoke(go);
    }
}
