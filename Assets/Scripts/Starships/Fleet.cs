using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Fleet : MonoBehaviour, EndTurnObserver {
    public int speed = 8;
    List<GameObject> shipsInFleet;

    bool isMoving = false;
    List<GameObject> planets;
    GameObject newMovementTarget;

    GameObject orbitingPlanet;

    private Color allegiance;

    private int travelRemaining = 100;

    private bool inTransit = false;

    private string fleetPrefix = "Fleet-";

    public string fleetName;
    
    void Awake() {
        TurnHandler.instance().addEndTurnObserver(this);
        shipsInFleet = new List<GameObject>();
        planets = new List<GameObject>();
        orbitingPlanet = null;

        fleetName = createFleetName();
    }

    void Update() {
        if (Input.GetKeyDown("p")) {
            listPlanet();
        }
    }

    /**
     * Sets the allegiance for this particular fleet
     * TODO - replace with Allegiances once that feature has been implemented
     */
    public void setFleetAllegiance(Color c) {
        allegiance = c;
    }

    public string getFleetName() {
        return fleetName;
    }

    /**
     * Returns the current allegiance of the fleet
     */
    public Color getAllegiance() {
        return allegiance;
    }

    /**
     * Adds the ship to the list of ships within this fleet
     * @param Ship - the ship we want to add to this fleet
     */
    public void addShipToFleet(GameObject ship) {
        if (ship == null) { return; }
        shipsInFleet.Add(ship);
        ship.GetComponent<Ship>().attachToFleet(this.gameObject);
        
        // If the new ship is slower than the remaining distance, we slow the rest of the fleet down
        int slowestShip = getSlowestShipDistance();
        if (travelRemaining > slowestShip) {
            travelRemaining = slowestShip;
        }
    }

    /**
     * Adds a list of ships into this fleet
     * @param List<GameObject> ships - The list of ships we want to add to this fleet
     */
    public void addShipsToFleet(List<GameObject> ships) {
        foreach(GameObject ship in ships) {
            addShipToFleet(ship);
        }
    }

    /**
     * Transfer a list of ships into this fleet
     * @param List<GameObject> ships - the list of ships we want to add to this fleet
     */
    public void transferShipsToFleet(List<GameObject> ships) {
        foreach(GameObject ship in ships) {
            Methods.addGameObjectAsChild(gameObject, ship);
            ship.transform.position = gameObject.transform.position;
            addShipToFleet(ship);
        }
    }

    /**
     * Lists off all the ships in this fleet by name
     */
    public List<GameObject> listShipsInFleet() {
        List<GameObject> ships = new List<GameObject>();
        foreach (GameObject ship in shipsInFleet) {
            string name = ship.GetComponent<Ship>().getName();
            Debug.Log(name);
            ships.Add(ship);
        }
        return ships;
    }

    /**
     * Lists the planet that the fleet is currently around
     */
    public void listPlanet() {
        if (orbitingPlanet != null) {
            Debug.Log(orbitingPlanet.GetComponent<Planet>().getName());
        }
    }

    /**
     * Returns a list of all the ships in this current fleet
     */
    public List<GameObject> getShipsInFleet() {
        return shipsInFleet;
    }

    /**
     * Moves the fleet along the quickest path of planets towards the target goal
     */
    public void moveFleet(GameObject[] listOfPlanets) {
        if (!isMoving) {
            isMoving = true;
            planets = listOfPlanets.ToList();
            newMovementTarget = popNextDestination();
            orbitingPlanet = null;
        }
    }

    /**
     * Teleports the fleet to its destination through space (calculating the intended travel time)
     */
    public int getFleetArrivalTime(GameObject[] listOfPlanets) {
        int costToMoveThroughFriendlyPlanets = 1;
        int costToMoveThroughEnemyPlanets = 2;

        planets = listOfPlanets.ToList();

        int arrivalTime = TurnHandler.instance().getCurrentTurnCount();
        // TODO - make this calculation take into account the speed of items in fleet
        Color fleetAllegiance = getAllegiance();
        foreach (GameObject go in listOfPlanets) {
            if (go.GetComponent<Planet>().getAllegiance() == fleetAllegiance) {
                arrivalTime += costToMoveThroughFriendlyPlanets;
            } else {
                arrivalTime += costToMoveThroughEnemyPlanets;
            }
        }
        return arrivalTime;
    }

    /**
     * Removes the fleet from any associated planets, and destroys the game object 
     */
    public void destroyFleet() {
        if (orbitingPlanet != null) {
            orbitingPlanet.GetComponent<Planet>().removeFleet(gameObject);
        }
        shipsInFleet.Clear();
        Destroy(gameObject);
    }

    /**
     * Lands the ship on the new movement target
     */
    public void landOnPlanet() {
        orbitPlanet(newMovementTarget);
        newMovementTarget.GetComponent<Planet>().setFleet(this.gameObject);
    }

    public void teleportToPlanet(GameObject newPlanet) {
        orbitPlanet(newPlanet);
        setFleetPosition(newPlanet.transform.position);
        newPlanet.GetComponent<Planet>().setFleet(this.gameObject);
        setInTransit();
    }

    public void setInTransit() {
        inTransit = true;
        foreach (GameObject ship in shipsInFleet) {
            ship.GetComponent<Ship>().setInTransit(true);
        }
    }

    /**
     * Sets the transit status to false for this fleet and all of its ships
     */
    public void setFinishedTransit() {
        foreach (GameObject ship in shipsInFleet) {
            ship.GetComponent<Ship>().setInTransit(false);
        }
        inTransit = false;
    }

    /**
     * Indicates whether this fleet is currently transiting between planets
     */
    public bool isInTransit() {
        return inTransit;
    }

    public bool hasBuiltAllShips() {
        // shipsInFleet.All(ship => ship.GetCompone)
        return false;
    }

    /**
     * Sets the planet 
     */
    public void orbitPlanet(GameObject planet) {
        orbitingPlanet = planet;
    }

    /**
     * Calculates the strength of this fleet
     * TODO - Add shield factor in here
     */
    private int calculateFleetStrength(Fleet fleet) {
        List<GameObject> ships = fleet.getShipsInFleet();
        int strength = 0;
        foreach (GameObject ship in ships) {
            strength += ship.GetComponent<Ship>().getAttack();
        }
        return strength;
    }

    /**
     * Checks to see if the fleet has arrived at its target destination
     * If it has, add the fleet to the planet, and stop moving
     * Otherwise, continue moving
     */
    private void continueMovingOrStop() {
        travelRemaining--;
        if (planets.Count > 0) {
            newMovementTarget = popNextDestination();
        } else {
            landOnPlanet();
            isMoving = false;
        }
    }

    /**
     * Fetch the fleets over the current planet we are traveling through
     * @return List - List containing all the fleets over planet (can be empty)
     */
    private List<GameObject> getVisitingPlanetFleets() {
        return newMovementTarget.GetComponent<Planet>().getFleetsOverPlanet();
    }

    /**
     * Determines whether there is a fleet already present at the newly arrived planet
     * @return bool - Whether or not there is an enemy fleet at the current planet
     */
    public bool isEnemyFleetAtPlanet() {
        List<GameObject> possibleFleets = getVisitingPlanetFleets();
        if (possibleFleets.Any()) {
            //TODO - Just checking the first one. If there is a concept of alliances, and multiple types on planet, may need more
            Color otherFleetAllegiance = possibleFleets[0].GetComponent<Fleet>().getAllegiance();
            return otherFleetAllegiance != getAllegiance();
        }
        return false;
    }

    /**
     * Sets the position of the fleet to the new value
     */
    private void setFleetPosition(Vector3 position) {
        gameObject.transform.position = position;
    }

    /**
     * Removes the first entry in the list of necessary movements for the fleet to reach its eventual destination
     */
    private GameObject popNextDestination() {
        GameObject removal = planets[0];
        planets.RemoveAt(0);
        return removal;
    }

    /**
     * Finds the lowest distance of the ships within this fleet
     */
    private int getSlowestShipDistance() {
        int lowest = 1000;
        foreach (GameObject ship in shipsInFleet) {
            int shipMovementLimit = ship.GetComponent<Ship>().getDistance();
            if (shipMovementLimit < lowest) {
                lowest = shipMovementLimit;
            }
        }
        return lowest;
    }

    /**
     * Resets the travel remaining after an end of turn
     */
    private void resetTravel() {
        travelRemaining = getSlowestShipDistance();
    }

    public void onEndTurnNotify() {
        resetTravel();
    }

    private string createFleetName() {
        // This needs to be initialized here to ensure each planet creates it's own random generator
        RandomLetters fleetNameGenerator = new RandomLetters(5);
        String fleetName = fleetNameGenerator.generateString();
        return fleetPrefix + fleetName;
    }
}