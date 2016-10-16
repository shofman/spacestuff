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

    private Planet planet;
    private Color planetAllegiance;

    void Awake() {
    }

    void Start() {
        planet = gameObject.GetComponent<Planet>();
        planetAllegiance = planet.getPlanetColor();
    }

    void Update() {

    }

    /**
     * Creates a ship
     * Either creates a new fleet if no ships are present, or adds the new ship to a fleet
     *
     */
    public void createShip() {
        GameObject fleet = findOrCreateFleet(planet.getFleetsOverPlanet());
        GameObject newShip = (GameObject) Instantiate (shipObject);
        
        // Add ship to fleet
        Methods.addGameObjectAsChild(fleet, newShip);
        newShip.transform.position = new Vector3(gameObject.transform.position.x + 6, gameObject.transform.position.y, gameObject.transform.position.z);
        fleet.GetComponent<Fleet>().addShipToFleet(newShip);
        
        // Update ship with necessary variables
        Ship ship = newShip.GetComponent<Ship>();
        ship.setName(getShipName());
        ship.setAllegiance(planetAllegiance);
    }

    private string getShipName() {
        // This needs to be initialized here to ensure each planet creates it's own random generator
        RandomLetters shipNameGenerator = new RandomLetters(3);
        String shipName = shipNameGenerator.generateString();
        return shipPrefix + shipName;
    }

    private GameObject findOrCreateFleet(List<GameObject> fleets) {
        if (!fleets.Any()) {
            GameObject fleet = (GameObject) Instantiate(fleetObject);
            fleet.transform.position = gameObject.transform.position;
            fleet.GetComponent<Fleet>().setFleetAllegiance(planetAllegiance);
            fleet.GetComponent<Fleet>().orbitPlanet(gameObject);
            planet.setFleet(fleet);
            return fleet;
        } else {
            return fleets[0];
        }
    }
}