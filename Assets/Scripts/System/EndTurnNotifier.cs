using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Invokes the notificaton method
public class EndTurnNotifier
{
    /**
     * Singleton pattern - we want only one instance of a end turn notifier per game
     */
    private static EndTurnNotifier _instance;

    /**
     * A list with observers that are waiting for something to happen
     */
    List<EndTurnObserver> observers = new List<EndTurnObserver>();

    /**
     * Statically access the end of turn notifier
     */
    public static EndTurnNotifier instance() {
        if (_instance == null) {
            _instance = new EndTurnNotifier();
        }
        return _instance;
    }

    /**
     * Constructor - set to private to prevent accidental use
     */
    private EndTurnNotifier() {

    }

    //Send notifications if something has happened
    public void notify()
    {
        for (int i = 0; i < observers.Count; i++)
        {
            //Notify all observers even though some may not be interested in what has happened
            //Each observer should check if it is interested in this event
            observers[i].onEndTurnNotify();
        }
    }

    //Add observer to the list
    public void addEndTurnObserver(EndTurnObserver observer)
    {
        observers.Add(observer);
    }

    //Remove observer from the list
    public void removeObserver(EndTurnObserver observer)
    {
        Debug.Log("NOT DONE");
    }
}