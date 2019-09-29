using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShipDisplay : Display, ChangePlayerObserver, PlanetObserver, EndTurnObserver {
    public GameObject createShipButton;
    public GameObject moveShipButton;
    public GameObject shipDisplayScrollbar;

    public GameObject mouse;

    // Fleet and ship display objects
    public GameObject fleetHolder;
    public GameObject fleetNameDisplay;
    public GameObject shipNameDisplay;

    private bool showingShipScrollbar;
    private GameObject chosenFleet;

    private Player currentPlayer;

    protected override void Awake() {
        base.Awake();

        CurrentPlayer.instance().addPlayerChangeObserver(this);
        PlanetChangeNotifier.instance().addObserver(this);
        TurnHandler.instance().addEndTurnObserver(this);

        shipDisplayScrollbar.SetActive(false);
        showingShipScrollbar = false;
        chosenFleet = null;

        currentPlayer = CurrentPlayer.instance().getCurrentPlayer();
    }

    /**
     * Create a single ship from a button
     */
    public void createShipFromButton() {
        if (getPlanet() != null) {
            bool hasCreatedShip = getPlanet().GetComponent<PlanetProduction>().createShip();
            if (hasCreatedShip) {
                validateShipButtons();
                updateShipListingForPlanet();
            }
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
     * Setups up the mouse display to show 
     */
    public void moveShips() {
        if (areMultipleFleetsOverPlanet()) {
            toggleDisplayShipsOnPlanet(true);
            Debug.Log("Which fleet?");
        } else {
            toggleMouseForShipMovement();
        }
    }

    /**
     * Toggles the mouse into a targeting reticule
     */
    public void toggleMouseForShipMovement() {
        disablePlanetOverviewMenu();
        mouse.GetComponent<TargetMouseTexture>().toggleTargetedMouse();
    }

    /**
     * Sets the chosen fleet for moving (in the case of multiple)
     */
    public void setChosenFleet(GameObject fleet) {
        chosenFleet = fleet;
    }

    /**
     * Clears out chosen fleet after use
     */
    public void clearChosenFleet() {
        chosenFleet = null;
    }

    /**
     * Retrieves the chosen fleet from the display
     */
    public GameObject getChosenFleet() {
        return chosenFleet;
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
    public void validateShipButtons() {
        if (getPlanet() != null) {
            List<GameObject> fleets = getFleets();
            Planet currentPlanet = getPlanet().GetComponent<Planet>();
            bool isBlockaded = currentPlanet.isBlockaded();
            bool hasFriendlyFleet = fleets.Any(fleet => fleet.GetComponent<Fleet>().getAllegiance() == currentPlanet.getAllegiance());
            if ((hasFriendlyFleet && !isBlockaded) || (!hasFriendlyFleet && isBlockaded)) {
                bool hasAMovableFleet = fleets.Any(fleet => !fleet.GetComponent<Fleet>().isInTransit());
                moveShipButton.GetComponent<Button>().interactable = hasAMovableFleet;
            } else {
                moveShipButton.GetComponent<Button>().interactable = false;
            }

            createShipButton.GetComponent<Button>().interactable = !isBlockaded;
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
        Text fleetText = fleetDisplay.GetComponentInChildren<Text>();

        Fleet fleetScript = fleet.GetComponent<Fleet>();
        fleetText.text = "Fleet " + fleetIndex;
        fleetText.color = fleetScript.isInTransit() ? Color.white : fleetScript.getAllegiance();
        fleetDisplay.GetComponent<FleetInteractions>().setParentDisplay(gameObject, fleet);

        // Attach list of ships to fleet display
        List<GameObject> ships = fleetScript.listShipsInFleet();
        foreach (var ship in ships) {
            GameObject shipName = (GameObject) Instantiate (shipNameDisplay);
            setUIParent(shipName, fleetDisplay);
            Text shipText = shipName.GetComponent<Text>();
            Ship shipScript = ship.GetComponent<Ship>();
            shipText.text = shipScript.getName();
            shipText.color = shipScript.isInTransit() ? Color.white : fleetScript.getAllegiance();
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

    private void updatePlanetDisplay() {
        GameObject possiblePlanet = getPlanet();
        if (possiblePlanet == null) {
            return;
        }

        Planet currentPlanet = possiblePlanet.GetComponent<Planet>();
        currentPlayer = CurrentPlayer.instance().getCurrentPlayer();

        bool isPlayerOwned = currentPlayer.getAllegiance() == currentPlanet.getAllegiance();
        bool isBlockaded = currentPlanet.isBlockaded();

        if (isPlayerOwned && !isBlockaded) {
            createShipButton.SetActive(true);
            moveShipButton.SetActive(true);
        } else {
            createShipButton.SetActive(false);

            if (isBlockaded && !isPlayerOwned) { // We are blockading an enemy planet - we can still move
                moveShipButton.SetActive(true);
            } else {
                moveShipButton.SetActive(false);
            }
        }

        validateShipButtons();
    }

    public void onEndTurnNotify() {
        updatePlanetDisplay();
        updateShipListingForPlanet();
    }

    public void onChangePlayerNotify() {
        updatePlanetDisplay();
    }

    public override void onPlanetChange(GameObject newPlanet) {
        base.onPlanetChange(newPlanet);
        updatePlanetDisplay();
    }
}
