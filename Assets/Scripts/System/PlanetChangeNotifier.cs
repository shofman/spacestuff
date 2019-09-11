using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Invokes the notificaton method
public class PlanetChangeNotifier
{
    /**
     * Singleton pattern - we want only one instance of a end turn notifier per game
     */
    private static PlanetChangeNotifier _instance;

    /**
     * A list with observers that are waiting for something to happen
     */
    List<PlanetObserver> observers = new List<PlanetObserver>();

    /**
     * Statically access the end of turn notifier
     */
    public static PlanetChangeNotifier instance() {
        if (_instance == null) {
            _instance = new PlanetChangeNotifier();
        }
        return _instance;
    }

    /**
     * Constructor - set to private to prevent accidental use
     */
    private PlanetChangeNotifier() {

    }

    //Send notifications if something has happened
    public void notify(GameObject newPlanet)
    {
        Debug.Log("calling notify with " + observers.Count);
        for (int i = 0; i < observers.Count; i++)
        {
            //Notify all observers even though some may not be interested in what has happened
            //Each observer should check if it is interested in this event
            observers[i].onPlanetChange(newPlanet);
        }
    }

    //Add observer to the list
    public void addObserver(PlanetObserver observer)
    {
        observers.Add(observer);
    }

    //Remove observer from the list
    public void removeObserver(PlanetObserver observer)
    {
        Debug.Log("NOT DONE");
    }
}