using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShipDisplay : Display {
    public GameObject moveShipButton;

    /**
     * Create a single ship from a button
     */
    public void createShipFromButton() {
        if (getPlanet() != null) {
            getPlanet().GetComponent<PlanetProduction>().createShip();
            validateMoveShipButton();
        }
    }

    /**
     * List all the ships currently on the planet
     */
    public void displayShipsOnPlanet() {
        if (getPlanet() != null) {
            List<GameObject> fleets = getFleets();
            foreach (var fleet in fleets) {
                fleet.GetComponent<Fleet>().listShipsInFleet(); 
            }
        }
    }

    /**
     * Validates whether or not we should show the move ship button
     * Disables it if we cannot move ships
     */
    public void validateMoveShipButton() {
        if (getPlanet() != null) {
            List<GameObject> fleets = getFleets();
            if (fleets.Any()) {
                moveShipButton.GetComponent<Button>().interactable = true;
            } else {
                moveShipButton.GetComponent<Button>().interactable = false;
            }
        }
    }

    private List<GameObject> getFleets() {
        return getPlanet().GetComponent<Planet>().getFleetsOverPlanet();
    }
}
