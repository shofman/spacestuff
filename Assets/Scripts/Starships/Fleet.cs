using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Fleet : MonoBehaviour {
    public int speed = 8;
    List<GameObject> shipsInFleet;

    bool isMoving = false;
    List<GameObject> planets;
    GameObject newMovementTarget;
    
    void Awake() {
        shipsInFleet = new List<GameObject>();
        planets = new List<GameObject>();
    }

    void Update() {
        moveShips();
    }

    /**
     * Adds the ship to the list of ships within this fleet
     * @param Ship - the ship we want to add to this fleet
     */
    public void addShipToFleet(GameObject ship) {
        shipsInFleet.Add(ship);
        ship.GetComponent<Ship>().attachToFleet(this.gameObject);
    }

    /**
     * Lists off all the ships in this fleet by name
     */
    public void listShipsInFleet() {
        foreach (GameObject ship in shipsInFleet) {
            string name = ship.GetComponent<Ship>().getName();
            Debug.Log(name);
        }
    }

    /**
     * Moves the fleet along the quickest path of planets towards the target goal
     */
    public void moveFleet(GameObject[] listOfPlanets) {
        if (!isMoving) {
            isMoving = true;
            planets = listOfPlanets.ToList();
            newMovementTarget = popNextDestination();
        }
    }

    /**
     * Responsible for adjusting the position of the ship. Moves towards target destination
     * Once there, checks to see if list of destinations is empty. If so, finish. Otherwise, pop entry
     * and continue
     */
    private void moveShips() {
        if (isMoving) {
            Vector3 position = newMovementTarget.transform.position;
            if (gameObject.transform.position != position) {
                float step = speed * Time.deltaTime;
                setFleetPosition(Vector3.MoveTowards(gameObject.transform.position, position, step));
            } else {
                if (planets.Count > 0) {
                    newMovementTarget = popNextDestination();
                } else {
                    newMovementTarget.GetComponent<Planet>().setFleet(this.gameObject);
                    isMoving = false;
                }
            }
        }
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


}