using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnDisplay : MonoBehaviour, ChangePlayerObserver {
	public GameObject PlayerTurnDisplay;

	Text txt;
	int turnCount = 0;

	float baseTimeInTurn = 36;
	int speedModifier = 1;
	float currentTurnTime;

	void Awake() {
		currentTurnTime = baseTimeInTurn;
		CurrentPlayer.instance().addPlayerChangeObserver(this);
		txt = gameObject.GetComponent<Text>();
		updateCount();

		updatePlayerColor();
	}
		    
  void Update() {
      currentTurnTime -= Time.deltaTime * speedModifier;

      if(currentTurnTime < 0) {
          updateCount();
          currentTurnTime = baseTimeInTurn;
      }
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
		changeDisplay.setAllegianceColor(CurrentPlayer.instance().getCurrentPlayer().getAllegiance());
	}

	/**
	 * Implemented as part of EndTurnObserver, is called when end of turn happens
	 */
	public void onChangePlayerNotify() {
    updatePlayerColor();
	}
}
