using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CurrentPlayer : EndTurnObserver
{
    /**
     * Singleton pattern - we want only one instance of a end turn notifier per game
     */
    private static CurrentPlayer _instance;

    private GameObject[] listOfPlayers;
    private GameObject currentPlayer;
    private int playerIndex = 0;

    /**
     * A list with observers that are waiting for something to happen
     */
    List<ChangePlayerObserver> observers = new List<ChangePlayerObserver>();

    /**
     * Statically access the end of turn notifier
     */
    public static CurrentPlayer instance() {
        if (_instance == null) {
            _instance = new CurrentPlayer();
        }
        return _instance;
    }

    /**
     * Constructor - set to private to prevent accidental use
     */
    private CurrentPlayer() {
        EndTurnNotifier.instance().addEndTurnObserver(this);
        listOfPlayers = GameObject.FindGameObjectsWithTag("Player").OrderBy(gameObject => gameObject.GetComponent<Player>().TurnOrder).ToArray();

        currentPlayer = listOfPlayers[0];
    }

    //Send notifications if something has happened
    public void notifyPlayerChange()
    {
        for (int i = 0; i < observers.Count; i++)
        {
            //Notify all observers even though some may not be interested in what has happened
            //Each observer should check if it is interested in this event
            observers[i].onChangePlayerNotify();
        }
    }

    private void changePlayer() {
        triggerPlayerCommandsBeforeSwitching();

        playerIndex += 1;

        if (playerIndex >= listOfPlayers.Length) {
            playerIndex = 0;
        }

        currentPlayer = listOfPlayers[playerIndex];
        notifyPlayerChange();
    }

    private void triggerPlayerCommandsBeforeSwitching() {
        foreach (GameObject player in listOfPlayers) {
            Player playerScript = player.GetComponent<Player>();
            playerScript.processUnfinishedCommands();
        }
    }

    public void onEndTurnNotify() {
        changePlayer();
    }

    public Player getCurrentPlayer() {
        return currentPlayer.GetComponent<Player>();
    }

    public GameObject getCurrentPlayerGameObject() {
        return currentPlayer;
    }

    //Add observer to the list
    public void addPlayerChangeObserver(ChangePlayerObserver observer)
    {
        observers.Add(observer);
    }

    //Remove observer from the list
    public void removePlayerChangeObserver(ChangePlayerObserver observer)
    {
        Debug.Log("NOT DONE");
    }
}