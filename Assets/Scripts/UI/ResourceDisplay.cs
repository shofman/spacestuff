﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResourceDisplay : MonoBehaviour {
    private Text txt;

    const string resourcePrefix = "Money: ";

    void Awake() {
        txt = gameObject.GetComponent<Text>();
    }

    /**
     * Increments the turn count 
     */
    public void setPlayerDisplay(int amount) {
        txt.text = resourcePrefix + amount;
    }
}
