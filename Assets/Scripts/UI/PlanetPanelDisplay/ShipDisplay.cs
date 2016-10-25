using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShipDisplay : Display {
    public GameObject moveShipButton;
    public GameObject shipDisplayScrollbar;

    // Fleet and ship display objects
    public GameObject fleetHolder;
    public GameObject fleetNameDisplay;
    public GameObject shipNameDisplay;

    private bool showingShipScrollbar;

    void Awake() {
        shipDisplayScrollbar.SetActive(false);
        showingShipScrollbar = false;
    }

    /**
     * Create a single ship from a button
     */
    public void createShipFromButton() {
        if (getPlanet() != null) {
            getPlanet().GetComponent<PlanetProduction>().createShip();
            validateMoveShipButton();
            updateShipListingForPlanet();
        }
    }

    /**
     * Unity cannot handle a nullable as part of the click - expose public method here without parameters to send default value in
     */
    public void displayShipsButtonClick() {
        toggleDisplayShipsOnPlanet();
    }

    /**
     * List all the ships currently on the planet
     */
    public void toggleDisplayShipsOnPlanet(bool? shouldShowDisplay = null) {
        if (getPlanet() != null) {
            List<GameObject> fleets = getFleets();

            if (fleets.Any()) {
                if (shouldShowDisplay != null) {
                    showingShipScrollbar = shouldShowDisplay.GetValueOrDefault();
                } else {
                    showingShipScrollbar = !showingShipScrollbar;
                }

                shipDisplayScrollbar.SetActive(showingShipScrollbar);
                removeShipsFromDisplay();
                if (showingShipScrollbar) {
                    int fleetIndex = 0;
                    foreach (var fleet in fleets) {
                        fleetIndex++;
                        createFleetAndShipViews(fleet, fleetIndex);
                    }
                }
            } else if (showingShipScrollbar) {
                // No fleets and still showing scrollbar - hide it
                deactivateShipDisplay();
            }
        }
    }

    /**
     * Updates the scrollbar displaying the list of ships
     */
    public void updateShipListingForPlanet() {
        if (showingShipScrollbar) {
            removeShipsFromDisplay();
            toggleDisplayShipsOnPlanet(true);
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

    /**
     * @returns bool - If there are multiple fleets over the planet
     */
    public bool areMultipleFleetsOverPlanet() {
        return getFleets().Count > 1;
    }

    /**
     * Turns off the scrollbar for displaying ships
     */
    public void deactivateShipDisplay() {
        shipDisplayScrollbar.SetActive(false);
        showingShipScrollbar = false;
        removeShipsFromDisplay();
    }

    /**
     * Removes all ship entries from the fleet holder (so they don't appear on other planet displays)
     */
    private void removeShipsFromDisplay() {
        if (fleetHolder != null) {
            foreach (Transform child in fleetHolder.transform) {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    /**
     * Returns whether or not we are currently showing the 
     */
    private bool currentlyShowingShipScrollbar() {
        return showingShipScrollbar;
    }

    /**
     * Returns a list of all the fleets over a planet
     */
    private List<GameObject> getFleets() {
        return getPlanet().GetComponent<Planet>().getFleetsOverPlanet();
    }

    /**
     * Creates game objects for displaying the fleets and ships on a planet
     */
    private void createFleetAndShipViews(GameObject fleet, int fleetIndex) {
        // Create fleet display
        GameObject fleetDisplay = (GameObject) Instantiate (fleetNameDisplay);
        setUIParent(fleetDisplay, fleetHolder);
        fleetDisplay.GetComponent<Text>().text = "Fleet " + fleetIndex;

        // Attach list of ships to fleet display
        List<GameObject> ships = fleet.GetComponent<Fleet>().listShipsInFleet();
        foreach (var ship in ships) {
            GameObject shipName = (GameObject) Instantiate (shipNameDisplay);
            setUIParent(shipName, fleetDisplay);
            shipName.GetComponent<Text>().text = ship.GetComponent<Ship>().getName();
        }
    }

    /**
     * Sets the child for a parent, and resets values to standard values
     */
    private void setUIParent(GameObject child, GameObject parent) {
        child.transform.SetParent(parent.transform);

        // Scale and pos Z values refuse to behave - set to large numbers (possibly due to zoom)
        // Reset to original values to compensate
        RectTransform rect = child.GetComponent<RectTransform>();
        rect.localScale = Vector3.one;
        rect.localPosition = Vector3.zero;
    }
}
