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

    protected string shipPrefix = "S-";

    protected GameObject fleet;
    protected Color allegiance;
    protected bool isTransiting;
    protected bool isBuilding;

    void Awake() {
        isTransiting = false;
        isBuilding = true;
        setName(getShipName());
    }

    protected virtual void Start() {}

    protected virtual void Update() {
        if (fleet != null && !isTransiting && !isBuilding) {
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
    private void setName(string name) {
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

    public bool isUnderConstruction() {
        return isBuilding;
    }

    public void finishConstruction() {
        isBuilding = false;
        GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
    }

    /**
     * Returns the cost to build the ship
     */
    public virtual int Cost {
        get { return 100; }
    }

    public virtual int ConstructionDuration {
        get { return 1000; }
    }

    protected string getShipName() {
        // This needs to be initialized here to ensure each planet creates it's own random generator
        RandomLetters shipNameGenerator = new RandomLetters(3);
        String shipName = shipNameGenerator.generateString();
        return getShipPrefix() + shipName;
    }

    protected virtual string getShipPrefix() {
        return this.shipPrefix;
    }
}