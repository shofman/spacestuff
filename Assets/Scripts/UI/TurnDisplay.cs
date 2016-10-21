using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TurnDisplay : MonoBehaviour, Observer {
	Text txt;
	int turnCount = 0;

	void Awake() {
		EndTurnNotifier.instance().addObserver(this);
		txt = gameObject.GetComponent<Text>();
		updateCount();
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

	/**
	 * Implemented as part of Observer, is called when end of turn happens
	 */
	public void onNotify() {
	    updateCount();
	}
}
