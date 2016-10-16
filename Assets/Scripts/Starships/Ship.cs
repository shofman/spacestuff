using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Ship : MonoBehaviour {
    public int health = 10;
    public int shields = 1;
    public int attack = 1;
    public string shipName = "";

    private GameObject fleet;
    private Color allegiance;

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
     * Sets the allegiance of this ship
     */
    public void setAllegiance(Color c) {
        allegiance = c;
        GetComponent<Renderer>().material.SetColor("_Color", c);
    }

    /**
     * Gets the allegiance of this particular ship
     */
    public Color getAllegiance() {
        return allegiance;
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

    /**
     * Returns the attack of the ship
     */
    public int getAttack() {
        return attack;
    }
}