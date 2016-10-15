using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Ship : MonoBehaviour {
    public int health = 10;
    public int shields = 1;
    public string shipName = "";

    private GameObject fleet;

    void Awake() {

    }

    void Start() {

    }

    void Update() {
        if (fleet != null) {
            transform.RotateAround(fleet.transform.position, Vector3.up, 20 * Time.deltaTime);
        }
    }

    /**
     * Attaches this ship to an associated fleet
     */
    public void attachToFleet(GameObject fleet) {
        this.fleet = fleet;
    }

    /**
     * Sets the name of this ship
     */
    public void setName(string name) {
        this.shipName = name;
    }

    /**
     * Returns the name of the ship
     * @return String - name of ship
     */
    public string getName() {
        return shipName;
    }
}