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

    private Planet planet;
    private Color planetAllegiance;
    private GameObject[] listOfPlayers;

    void Awake() {
        listOfPlayers = GameObject.FindGameObjectsWithTag("Player");
    }

    void Start() {
        planet = gameObject.GetComponent<Planet>();
        planetAllegiance = planet.getAllegiance();
    }

    /**
     * Creates a ship
     * Either creates a new fleet if no ships are present, or adds the new ship to a fleet
     */
    public bool createShip(GameObject shipTypeToCreate) {
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
        ship.setAllegiance(planetAllegiance);
        
        // Add ship to fleet
        GameObject fleet = findOrCreateFleet(planet.getFleetsOverPlanet());
        Methods.addGameObjectAsChild(fleet, newShip);
        newShip.transform.position = new Vector3(gameObject.transform.position.x + 6, gameObject.transform.position.y, gameObject.transform.position.z);
        fleet.GetComponent<Fleet>().addShipToFleet(newShip);

        return true;
    }

    private GameObject createFleet() {
        GameObject fleet = (GameObject) Instantiate(fleetObject);
        fleet.transform.position = gameObject.transform.position;
        fleet.GetComponent<Fleet>().setFleetAllegiance(planetAllegiance);
        fleet.GetComponent<Fleet>().orbitPlanet(gameObject);
        planet.setFleet(fleet);
        return fleet;
    }

    private GameObject findOrCreateFleet(List<GameObject> fleets) {
        try {
            return fleets.First(fleet => !fleet.GetComponent<Fleet>().isInTransit() && fleet.GetComponent<Fleet>().getAllegiance() == planetAllegiance);
        }
        catch (InvalidOperationException) {}
        catch (ArgumentNullException) {} // Do nothing - we will just create a fleet normally
        return createFleet();
    }
}