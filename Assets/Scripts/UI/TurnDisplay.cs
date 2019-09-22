using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnDisplay : MonoBehaviour, ChangePlayerObserver {
	public GameObject PlayerTurnDisplay;

	Text txt;

	float baseTimeInTurn = 36;
	int speedModifier = 1; // Can adjust this if we want to decouple the Time.timeScale to avoid having animations look poor
	float currentTurnTime;

	void Awake() {
		currentTurnTime = baseTimeInTurn;

		CurrentPlayer.instance().addPlayerChangeObserver(this);
		txt = gameObject.GetComponent<Text>();
		updateCount();

		updatePlayerColor();
	}
		    
  void Update() {
      currentTurnTime -= Time.deltaTime * 1;
      if(currentTurnTime < 0) {
		      EndTurnNotifier.instance().notify();
          updateCount();
          currentTurnTime = baseTimeInTurn;
      }
  }

	/**
	 * Increments the turn count 
	 */
	private void updateCount() {
		txt.text = "Turn: " + EndTurnNotifier.instance().getCurrentTurnCount();
	}

	private void updatePlayerColor() {
		ChangePlayerDisplay changeDisplay = PlayerTurnDisplay.GetComponent<ChangePlayerDisplay>();
    Player p = CurrentPlayer.instance().getCurrentPlayer();
		changeDisplay.setAllegianceColor(p.getAllegiance());
	}

	public void changeTurnSpeed(Slider speedSlider) {
		speedModifier = (int) speedSlider.value;
	}

	public void onChangePlayerNotify() {
    updatePlayerColor();
	}
}
