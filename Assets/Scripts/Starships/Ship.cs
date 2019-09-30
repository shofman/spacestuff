using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Ship : MonoBehaviour {
    public int health = 10;
    public int shields = 1;
    public int attack = 1;
    public int distance = 3;
    public string shipName = "";

    const string shipPrefix = "F3-";

    protected GameObject fleet;
    protected Color allegiance;
    protected bool isTransiting;

    void Awake() {
        isTransiting = false;
        setName(getShipName());
    }

    protected virtual void Start() {}

    protected virtual void Update() {
        if (fleet != null && !isTransiting) {
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

    /**
     * Returns the distance of the ship
     */
    public int getDistance() {
        return distance;
    }

    public void setInTransit(bool inTransit) {
        isTransiting = inTransit;
        if (inTransit) {
            GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
        } else {
            GetComponent<Renderer>().material.SetColor("_Color", allegiance);
        }
    }

    public bool isInTransit() {
        return isTransiting;
    }

    /**
     * Returns the cost to build the ship
     */
    public virtual int Cost
    {
        get { return 100; }
    }

    protected string getShipName() {
        // This needs to be initialized here to ensure each planet creates it's own random generator
        RandomLetters shipNameGenerator = new RandomLetters(3);
        String shipName = shipNameGenerator.generateString();
        return shipPrefix + shipName;
    }
}