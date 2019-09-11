using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

		listOfPlayers = GameObject.FindGameObjectsWithTag("Player").OrderBy(gameObject => gameObject.GetComponent<Player>().TurnOrder).ToArray();

		currentPlayer = listOfPlayers[0];
		updatePlayerColor();
	}

	private void triggerPlayerCommands() {
		Player player = currentPlayer.GetComponent<Player>();
		player.processUnfinishedCommands();
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
    // Note - triggerPlayerCommands must come before changePlayer is called
    triggerPlayerCommands();
    changePlayer();
	}
}
