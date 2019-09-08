using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TurnDisplay : MonoBehaviour, Observer {
	public GameObject PlayerTurnDisplay;

	Text txt;
	int turnCount = 0;

	private GameObject[] listOfPlayers;

	private GameObject currentPlayer;
	private int playerIndex = 0;

	void Awake() {
		EndTurnNotifier.instance().addObserver(this);
		txt = gameObject.GetComponent<Text>();
		updateCount();

		listOfPlayers = GameObject.FindGameObjectsWithTag("Player");
		currentPlayer = listOfPlayers[0];
		updatePlayerColor();
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	/**
	 * Increments the turn count 
	 */
	private void updateCount() {
		turnCount++;
		txt.text = "Turn: " + turnCount;
	}

	private void updatePlayerColor() {
		ChangePlayerDisplay changeDisplay = PlayerTurnDisplay.GetComponent<ChangePlayerDisplay>();
		changeDisplay.setAllegianceColor(currentPlayer.GetComponent<Player>().getAllegiance());
	}

	private void changePlayer() {
		playerIndex += 1;

		if (playerIndex >= listOfPlayers.Length) {
			playerIndex = 0;
			updateCount();
		}

		currentPlayer = listOfPlayers[playerIndex];
		updatePlayerColor();
	}

	/**
	 * Implemented as part of Observer, is called when end of turn happens
	 */
	public void onNotify() {
	    changePlayer();
	}
}
