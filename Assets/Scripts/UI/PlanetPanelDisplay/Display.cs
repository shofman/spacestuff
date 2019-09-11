using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/**
 * Generic class holding the logic shared between display panels on the planet menu
 */
public class Display : MonoBehaviour, ITabDisplayInterface, PlanetObserver {
    public GameObject planetPanel;
    /**
     * The planet we are currently viewing
     */
    GameObject planetToDisplay;

    protected virtual void Awake() {
        planetToDisplay = null;
        PlanetChangeNotifier.instance().addObserver(this);
    }

    /**
     * Returns the planet we are currently displaying
     * @return {[type]} GameObject - The planet we are viewing
     */
    public GameObject getPlanet() {
        return planetToDisplay;
    }

    /**
     * Enables the current display
     */
    public void enableDisplay() {
        gameObject.SetActive(true);
    }

    /**
     * Disables the current display
     */
    public void disableDisplay() {
        gameObject.SetActive(false);
    }

    /**
     * Disables the display for the planet
     */
    public void disablePlanetOverviewMenu() {
        planetPanel.GetComponent<TabManager>().setOpen(false);
        GameObject planet = getPlanet();
        if (planet != null) {
            planet.GetComponent<Planet>().deactivatePlanetMenu();
        }
    }

    /**
    * Triggered when the selected planet changes
    */
    public void onPlanetChange(GameObject newPlanet) {
        planetToDisplay = newPlanet;
    }
}
