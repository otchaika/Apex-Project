/////////////////////////////////////////////////////////////////////////////////////////
///
///   Author: Prof. Dr. Frank Gabler, fbmd, Hochschule Darmstadt
///   ER-T2
///   Created: x.05.19
/// 
////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;

// Scritable Objects you need to create via menu, because you can not attach them to Objects!
[CreateAssetMenu(fileName = "New Event", menuName = "Game Event", order = 52)]

// Watchout: GameEvent it not derived from MonoBehavior! Is's a ScriptableObject
public class GameEvent : ScriptableObject
{
    // create a list of all the objects listening to our event
    private List<EventListener> elisteners = new List<EventListener>();

    // Listeners musst be registered to become informed
    public void Register(EventListener listener)
    {
        elisteners.Add(listener);
    }

    // There must be the possibility to unregister if a listener becomes destroyed or inactive
    public void Unregister(EventListener listener)
    {
        elisteners.Remove(listener);
    }

    // This method informs all the listeners if an event occurres
    public void Occurred(GameObject go)
    {
        // loop over all listeners
        for (int i = 0; i < elisteners.Count; i++)
        {
            // Call the assigned function to inform them
            elisteners[i].OnEventOccurs(go);
        }
    }
}
