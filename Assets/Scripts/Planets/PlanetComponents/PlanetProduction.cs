using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class PlanetProduction : MonoBehaviour {
    // GameObject of a fleet of ships
    public GameObject fleetObject;
    public GameObject shipObject;

    const string shipPrefix = "F3-";
    RandomLetters shipNameGenerator;

    void Awake() {
        shipNameGenerator = new RandomLetters(3);
    }

    void Start() {

    }

    void Update() {

    }

    /**
     * Creates a ship
     * Either creates a new fleet if no ships are present, or adds the new ship to a fleet
     *
     */
    public void createShip() {
        List<GameObject> fleets = gameObject.GetComponent<Planet>().getFleetsOverPlanet();
        GameObject fleet;
        if (!fleets.Any()) {
            fleet = (GameObject) Instantiate(fleetObject);
            fleet.transform.position = gameObject.transform.position;
            gameObject.GetComponent<Planet>().setFleet(fleet);
        } else {
            fleet = fleets[0];
        }
        GameObject ship = (GameObject) Instantiate (shipObject);
        Methods.addGameObjectAsChild(fleet, ship);
        ship.transform.position = new Vector3(gameObject.transform.position.x + 6, gameObject.transform.position.y, gameObject.transform.position.z);
        String shipName = shipNameGenerator.generateString();
        ship.GetComponent<Ship>().setName(shipPrefix + shipName);
        fleet.GetComponent<Fleet>().addShipToFleet(ship);
    }
}