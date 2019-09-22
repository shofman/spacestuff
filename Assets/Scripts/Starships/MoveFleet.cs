using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class MoveFleet : MonoBehaviour, PlanetObserver {
    public GameObject player;
    public GameObject shipDisplay;

    private GameObject currentlySelectedPlanet;

    void Awake() {
        PlanetChangeNotifier.instance().addObserver(this);
    }

    /**
     * Called on each game loop
     * We use it to detect mouse clicks when moving a 
     */
    void Update() {
        // Detect left clicks only on planets
        if (Input.GetMouseButtonDown(0) && inTransferingFleetMouseState()) {
            RaycastHit hit = findObjectsOnScreen(); //Will return Vector3(0,0,0) if nothing found
            if (!(hit.point.x == 0 &&
                hit.point.y == 0 &&
                hit.point.z == 0)) {
                //If we hit a planet, we send the fleet there
                GameObject hitObject = hit.transform.gameObject;
                if (hitObject.tag == "Planet" && currentlySelectedPlanet != null) {
                    // Take the fleet at the currently selected planet, and move it to this planet
                    ShipDisplay shipScript = shipDisplay.GetComponent<ShipDisplay>();
                    GameObject chosenFleet = shipScript.getChosenFleet();
                    MoveShipsCommand moveShips;
                    if (chosenFleet != null) {
                        shipScript.clearChosenFleet();

                        moveShips = new MoveShipsCommand(
                            chosenFleet,
                            currentlySelectedPlanet,
                            hitObject,
                            player
                        );
                    } else {
                        // TODO - Figure out how to handle selecting which fleet to go
                        GameObject currentFleet = currentlySelectedPlanet.GetComponent<Planet>().getFleetOverPlanet();
                        moveShips = new MoveShipsCommand(
                            currentFleet,
                            currentlySelectedPlanet,
                            hitObject,
                            player
                        );
                    }
                    Player p = player.GetComponent<Player>();
                    p.addCommand(moveShips);
                    hitObject.GetComponent<Planet>().setTransferingFleet(true); // Don't show the player menu
                }
            }

            // Deactivate the moving fleet cursor regardless if we find something
            deactiveTargetMouse();
        } else if (Input.GetMouseButtonDown(1) && inTransferingFleetMouseState()) {
            // Cancel moving the fleet if a right click is detected
            deactiveTargetMouse();
        }
    }

    /**
     * Function to toggle off the target mouse state, as we have either fulfilled the goal
     * (transfering the fleet) or have entered a different state
     */
    private void deactiveTargetMouse() {
        if (inTransferingFleetMouseState()) {
            this.gameObject.GetComponent<TargetMouseTexture>().toggleTargetedMouse();
        }
    }

    /**
     * Checks to see what state the mouse is
     * @return Boolean - Whether or not we are trying to transfer a ship
     */
    private bool inTransferingFleetMouseState() {
        return MouseState.instance().getCurrentMouseState() == MouseState.State.MoveShip;
    }

    /**
     * Fires off a raycast, and returns the first game object that is hit
     * @return RaycastHit - The raycasthit containing the info of the object we hit
     *                      or an empty Vector3(0,0,0) for the point if nothing
     */
    private RaycastHit findObjectsOnScreen() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Casts the ray and get the first game object hit
        Physics.Raycast(ray, out hit);
        return hit;
    }

    /**
     * Triggers when the displayed planet is changed
     */ 
    public void onPlanetChange(GameObject newPlanet) {
        currentlySelectedPlanet = newPlanet;
    }
}