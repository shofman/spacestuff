using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class PlanetProduction : MonoBehaviour {
    // GameObject of a fleet of ships
    public GameObject fleetObject;
    public GameObject fighterObject;
    public GameObject seedShipObject;
    public GameObject scoutShipObject;
    public GameObject lightCruiserObject;
    public GameObject heavyCruiserObject;
    public GameObject troopTransportObject;

    const string shipPrefix = "F3-";

    private Planet planet;
    private Color planetAllegiance;
    private GameObject shipTypeToCreate;
    private GameObject[] listOfPlayers;

    void Awake() {
        shipTypeToCreate = fighterObject;
        listOfPlayers = GameObject.FindGameObjectsWithTag("Player");
    }

    void Start() {
        planet = gameObject.GetComponent<Planet>();
        planetAllegiance = planet.getAllegiance();
    }

    void Update() {
        if (Input.GetKeyDown("f")) {
            Debug.Log("Making fighters");
            shipTypeToCreate = fighterObject;
        } else if (Input.GetKeyDown("h")) {
            Debug.Log("Making heavy cruisers");
            shipTypeToCreate = heavyCruiserObject;
        }
    }

    /**
     * Creates a ship
     * Either creates a new fleet if no ships are present, or adds the new ship to a fleet
     *
     */
    public bool createShip() {
        // For now, find the player whose allegiance this planet belongs to. in the future, change this to be passed in as a dependency
        // Check if ship can be created (allegiance differences / too little money)
        bool isMatchingPlayer = false;
        Player matchingPlayer = null;
        int currentWealth = -10000;

        foreach (GameObject player in listOfPlayers)
        {
            Player p = player.GetComponent<Player>();
            if (p.getAllegiance() == planetAllegiance) {
                matchingPlayer = p;
                isMatchingPlayer = true;
                currentWealth = p.getWealth();
            }
        }

        if (!isMatchingPlayer) {
            return false;
        }

        GameObject newShip = (GameObject) Instantiate (shipTypeToCreate);
        Ship ship = newShip.GetComponent<Ship>();
        int shipCost = ship.Cost;

        if (currentWealth < shipCost) {
            Destroy(newShip);
            return false;
        }
        // Deduct the cost
        matchingPlayer.spendMoney(shipCost);

        // Update ship with necessary variables
        ship.setName(getShipName());
        ship.setAllegiance(planetAllegiance);
        
        // Add ship to fleet
        GameObject fleet = findOrCreateFleet(planet.getFleetsOverPlanet());
        Methods.addGameObjectAsChild(fleet, newShip);
        newShip.transform.position = new Vector3(gameObject.transform.position.x + 6, gameObject.transform.position.y, gameObject.transform.position.z);
        fleet.GetComponent<Fleet>().addShipToFleet(newShip);
        

        return true;
    }

    // TODO - move this into the ship class itself
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