using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Planet : MonoBehaviour, IPointerClickHandler, IBreadthFirstSearchInterface {
    //Spice provides money, which buys troops every round
    //Defense provides a defense bonus, 
    //Garrisons provides the defense value, which must be beaten to determine victory
    //Trade Routes determine where a planet can connect with
    int spice;
    int defense;
    int garrisons;
    bool isRemoved = false;
    bool hasVisited = false;
    bool hasLauncher = false;
    GameObject priorGameObject;
    bool createdNameObject = false;
    string planetName;
    int galaxyPositionX = -1;
    int galaxyPositionY = -1;
    List<GameObject> connectedPlanets;
    List<GameObject> listOfRoutes;

    public GameObject planetNameDisplay;
    public GameObject tradeRoute;
    public float lineWidth;
    
    GameObject canvasUI;
    GameObject tabManager;
    List<GameObject> fleetOverPlanet;

    // Displays for planet
    GameObject shipDisplay;
    GameObject troopDisplay;
    GameObject peopleDisplay;
    GameObject productionDisplay;
    GameObject planetDisplay;
    System.Random random;

    private GameObject galaxy;

    private Color planetColor;
    private GameObject nameDisplay;

    // Boolean for detecting whether we are trying to transfer a fleet to this planet
    bool isTransferingAFleet = false;

    void Awake() {
        canvasUI = GameObject.Find("/PlanetMenu");
        tabManager = GameObject.Find("/PlanetMenu/Panel");

        // Setup the displays
        shipDisplay = GameObject.Find("/PlanetMenu/Panel/DisplayHolder/ShipDisplay");
        troopDisplay = GameObject.Find("/PlanetMenu/Panel/DisplayHolder/TroopDisplay");
        peopleDisplay = GameObject.Find("/PlanetMenu/Panel/DisplayHolder/PeopleDisplay");
        productionDisplay = GameObject.Find("/PlanetMenu/Panel/DisplayHolder/ProductionDisplay");
        planetDisplay = GameObject.Find("/PlanetMenu/Panel/DisplayHolder/PlanetDisplay");
        
        connectedPlanets = new List<GameObject>();
        listOfRoutes = new List<GameObject>();
        random = new System.Random ();
        fleetOverPlanet = new List<GameObject>();
    }

    // Use this for initialization
    void Start () {
        spice = random.Next (0, 100);
        defense = random.Next (0, 100);
        hasVisited = false;
    }
    
    // Update is called once per frame
    void Update () {

    }

    /**
     * Sets whether we are trying to transfer a fleet to this planet
     * We use this method to get around the limitations of the UI detection
     * (Raycasts do not get detected on the UI elements, so we want to prevent clicks from falling through)
     * @param {[type]} bool isTransfering [description]
     */
    public void setTransferingFleet(bool isTransfering) {
        isTransferingAFleet = isTransfering;
    }

    /**
     * Detects mouse clicks, but only if a UI element does not interfere with the click
     * @param eventData - Information about the event that we don't really care currently about
     */
    public void OnPointerClick(PointerEventData eventData) {
        if (isTransferingAFleet) {
            // Take the fleet at the currently selected planet, and move it to this planet
            GameObject currentlySelectedPlanet = shipDisplay.GetComponent<Display>().getPlanet();
            currentlySelectedPlanet.GetComponent<Planet>().moveFleet(gameObject);
        } else {
            activatePlanetMenu();
        }
        isTransferingAFleet = false;
    }

    /**
     * Deactivates the planet menu
     */
    public void deactivatePlanetMenu() {
        canvasUI.SetActive(false);
    }

    /**
     * Sets whether this planet has a launcher to another galaxy
     */
    public void setLauncher(bool hasLauncher) {
        this.hasLauncher = hasLauncher;
    }

    /**
     * Activates the displays for showing the planet menu 
     * This is what will show the planet statistics, fleet information, etc
     */
    private void activatePlanetMenu() {
        // Initialize the planet for the displays
        shipDisplay.GetComponent<Display>().setPlanet(gameObject);
        troopDisplay.GetComponent<Display>().setPlanet(gameObject);
        peopleDisplay.GetComponent<Display>().setPlanet(gameObject);
        productionDisplay.GetComponent<Display>().setPlanet(gameObject);
        planetDisplay.GetComponent<Display>().setPlanet(gameObject);

        // If the menu is closed, open it and show the currently selected tab
        tabManager.GetComponent<TabManager>().enableDisplayOnOpen();
        
        // Set the name for the planet display tab
        planetDisplay.GetComponent<PlanetDisplay>().setName(planetName);

        // Set the garrison amount for the troop display tab
        troopDisplay.GetComponent<TroopDisplay>().setGarrisons(garrisons);

        // Activate the main menu
        canvasUI.SetActive(true);
    }

    /**
     * Adds a planet to the list of planets we are connected to via trade routes
     * @param GameObject - The planet that we wish to add to our connected list
     */
    public void addTradeRoute(GameObject planet) {
        this.connectedPlanets.Add(planet);
    }

    /**
     * Returns a list of the planets we are connected to via trade routes
     * @return List<GameObjects>
     */
    public List<GameObject> getConnectedObjects() {
        return this.connectedPlanets;
    }

    /**
     * Remove a trade route from the list of connected planets
     * @param  GameObject The planet we wish to remove
     */
    public void removeConnectedPlanet(GameObject planet) {
        connectedPlanets.Remove(planet);
        for (int i=listOfRoutes.Count-1; i >=0; i--) {
            Destroy((GameObject)listOfRoutes[i]);
            listOfRoutes.RemoveAt(i);
        }
    }

    /**
     * Displays the planets that we are currently connected to via trade routes
     * @return string - A list of planets we have trade relations with
     */
    public string printConnectedPlanets() {
        string output = "";
        foreach (GameObject planet in connectedPlanets) {
            output += planet.GetComponent<Planet>().getName() + " - ";
        }
        return output;
    }

    /**
     * Removes a trade route from this planet, based off a probability
     * @param int - the probability that we will remove a trade route
     */
    public void removeTradeRoute(int probability) {
        for (int i = connectedPlanets.Count - 1; i >= 0; i--) {
            int randomValue = random.Next(0,100);
            if (randomValue < probability) {
                connectedPlanets.RemoveAt(i);
            }
        }

        for (int i=listOfRoutes.Count-1; i >=0; i--) {
            Destroy((GameObject)listOfRoutes[i]);
            listOfRoutes.RemoveAt(i);
        }
    }

    /**
     * Creates a number of lineRenderers to represent the trade routes between planets
     * @param  GameObject - an empty gameObject holder to house the created trade route objects
     */
    public void displayTradeRoutes(GameObject tradeRouterHolder) {
        for (int i=listOfRoutes.Count-1; i >=0; i--) {
            Destroy((GameObject)listOfRoutes[i]);
            listOfRoutes.RemoveAt(i);
        }

        foreach(GameObject planet in connectedPlanets) {
            GameObject lineRenderer = (GameObject)Instantiate(tradeRoute);
            Methods.addGameObjectAsChild(tradeRouterHolder, lineRenderer);
            this.listOfRoutes.Add(lineRenderer);
            Vector3 currPosition = gameObject.transform.position;
            Vector3 targetPosition = planet.transform.position;
            LineRenderer line = lineRenderer.GetComponent<LineRenderer>();
            line.SetPosition(0, currPosition);
            line.SetPosition(1, targetPosition);
            line.SetWidth(lineWidth,lineWidth);
        }
    }

    public void createRandomTexture() {
        this.gameObject.GetComponent<RandomTexture>().createTexture();
    }

    /**
     * Sets the color of the trade route between planets
     * @param Color - the color we want the trade route to be represented by
     */
    public void setLineColor(Color c) {
        foreach(GameObject lineRenderer in listOfRoutes) {
            LineRenderer line = lineRenderer.GetComponent<LineRenderer>();
            line.SetColors(c,c);
        }
    }

    /**
     * Sets the name of the planet
     * Creates a game object (using a deprecated function) to display the name underneath the planet as well
     * This will only create a name once
     * @param The new name of the planet
     * 
     */
    public void setName(string name) {
        this.planetName = name;
        if (!createdNameObject) {
            createdNameObject = true;
            nameDisplay = (GameObject) Instantiate(planetNameDisplay);
            nameDisplay.transform.parent = this.transform;
            nameDisplay.transform.position = this.transform.position;
            nameDisplay.transform.position -= new Vector3(2.3f,5,0);
            nameDisplay.GetComponent<TextMesh>().text = name;
            nameDisplay.GetComponent<TextMesh>().fontSize = 20;
        }
    }

    /**
     * Update both the number of units and set the color
     * @param  Color - the color we want to set the planet to
     * @param  numberOfUnits - the number of units we want to place on the planet
     */
    public void changeColorAndSetUnits(Color color, int numberOfUnits) {
        setUnits(numberOfUnits);
        setToColor(color);
    }

    /**
     * Sets the amount of units currently on the planet (only a simple number, currently)
     * @param int - The number of units we want to place on the planet
     */
    public void setUnits(int numberOfUnits) {
        garrisons = numberOfUnits;
    }

    /**
     * Sets the planets color
     * @param Color - the color we want to set the planet to
     */
    public void setToColor(Color c) {
        planetColor = c;
        if (nameDisplay != null) {
            nameDisplay.GetComponent<TextMesh>().color = planetColor;
        }
    }

    /**
     * Gets the color associated with this planet
     */
    public Color getPlanetColor() {
        return planetColor;
    }

    /**
     * Shifts the planet into the position provided by the x and y parameters
     * @param The distance along the x coordinate system we want the planet to move along
     * @param The distance along the y  coordinate system we want the planet to move along
     */
    public void setPosition(int x, int y) {
        gameObject.transform.Translate(x,y,0);
    }

    /**
     * Return the name of this planet
     * @return string - the name of the planet
     */
    public string getName() {
        return this.planetName;
    }

    /**
     * Whether this planet has been removed during the creation of the galaxy (not currently implemented)
     * @return {Boolean} - Whether the planet has been removed
     */
    public bool hasBeenRemoved() {
        return this.isRemoved;
    }

    /**
     * Sets if this planet has been removed by the creation of the galaxy
     */
    public void setRemoval() {
        this.isRemoved = true;
    }

    /**
     * Sets whether this planet has been visited during a search
     * @return {Boolean} - Whether the planet has been visited during a search
     */
    public bool hasBeenVisited() {
        return this.hasVisited;
    }

    /**
     * Sets the status of whether this planet has already been visisted during a search
     * @param {Boolean} - Whether or not the planet has been visisted
     */
    public void setVisited(bool visitedStatus) {
        this.hasVisited = visitedStatus;
    }

    /**
     * Gets the current placeholder for the position within the two-dimensional array in the parent gameObject
     * @return int - The first coordinate for where this planet is located in the parents container
     */
    public int getIndexForX() {
        return this.galaxyPositionX;
    }

    /**
     * Gets the current placeholder for the position within the two-dimensional array in the parent gameObject
     * @return int - The second coordinate for where this planet is located within the parents container
     */
    public int getIndexForY() {
        return this.galaxyPositionY;
    }

    /**
     * Sets the location of where this object is located within the parents two-dimensional array (in this case, the galaxy)
     * @param int - the first coordinate (representing the depth of the array)
     * @param int - the second coordinate (representing the )
     */
    public void setIndex(int positionX, int positionY) {
        this.galaxyPositionX = positionX;
        this.galaxyPositionY = positionY;
    }

    /**
     * Updates the fleet above the planet
     * @param GameObject fleet The fleet object we want to add to our planet's fleet
     */
    public void setFleet(GameObject fleet) {
        fleetOverPlanet.Add(fleet);
        shipDisplay.GetComponent<ShipDisplay>().validateMoveShipButton();
    }

    /**
     * Removes the fleet at the given index
     */
    public void removeFleet(int index) {
        try {
            fleetOverPlanet.RemoveAt(index);
            shipDisplay.GetComponent<ShipDisplay>().validateMoveShipButton();
        } catch (ArgumentOutOfRangeException e) {
            Debug.Log("Trying to remove fleet that does not exist");
            Debug.Log(e.ToString());
        } 
    }

    /**
     * Removes the fleet matching the instance ID of the GameObject provided
     */
    public void removeFleet(GameObject fleetToRemove) {
        int fleetId = fleetToRemove.GetInstanceID();
        var item = fleetOverPlanet.SingleOrDefault(x => x.GetInstanceID() == fleetId);
        if (item != null) {
            fleetOverPlanet.Remove(item);
        }
    } 

    /**
     * Finds the fleet above the planet
     * TODO - Replace with selector to choose which fleet if multiple
     * @return GameObject the fleet currently above the planet
     */
    public GameObject getFleetOverPlanet() {
        if (fleetOverPlanet.Count > 0) {
            return fleetOverPlanet[0];
        } else {
            return null;
        }
    }

    /**
     * Temp object to assist in adding functionality for multiple fleets
     */
    public List<GameObject> getFleetsOverPlanet() {
        return fleetOverPlanet;
    }

    /**
     * Sets the galaxy that this planet belongs to
     */
    public void setGalaxy(GameObject galaxy) {
        this.galaxy = galaxy;
    }

    /**
     * Returns the galaxy that this planet belongs to
     */
    public GameObject getGalaxy() {
        return this.galaxy;
    }

    /**
     * Sets the prior planet visited (for bfs distances)
     */
    public void setPrior(GameObject prior) {
        priorGameObject = prior;
    }

    /**
     * Retrieves the planet visited prior (in bfs)
     */
    public GameObject getPrior() {
        return priorGameObject;
    }

    /**
     * Retrieves the trade routes for a particular planet
     */
    public List<GameObject> getTradeRoutes() {
        return listOfRoutes;
    }

    /**
     * Transfers the fleet to the planet provided
     * @param  GameObject planetToMoveTo - The planet we want to move to 
     * Currently, only moves the first entry on the fleet list
     */
    public void moveFleet(GameObject planetToMoveTo) {
        if (planetToMoveTo.GetInstanceID() != this.gameObject.GetInstanceID()) {
            GameObject fleet = getFleetOverPlanet();
            GameObject[] planetsToMoveThrough = null;
            if (isPlanetInSameGalaxy(planetToMoveTo)) {
                planetsToMoveThrough = galaxy.GetComponent<Galaxy>().bfsGalaxy(this.gameObject, planetToMoveTo);
            } else {
                planetsToMoveThrough = multiGalaxyMoveFleet(planetToMoveTo);
            }
            
            // TODO - make this not always the first entry
            removeFleet(0);
            
            if (fleet != null && planetsToMoveThrough != null) {
                fleet.GetComponent<Fleet>().moveFleet(planetsToMoveThrough);
            }
        }
    }

    /**
     * Since the planet belongs to another galaxy, we move the ships to the launcher for the current galaxy
     * We then do a universe bfs search for the fastest route to the galaxy we want to go towards from our current galaxy
     * Then we do another galaxy bfs from that launcher to the target planet
     */
    private GameObject[] multiGalaxyMoveFleet(GameObject planetToMoveTo) {
        GameObject[] planetsToMoveThrough = null;
        Galaxy currentGalaxy = galaxy.GetComponent<Galaxy>();
        // TODO - make the launchers a dynamic option (bfs each, and see whichever list is shorter)
        GameObject closestCurrentLauncher = currentGalaxy.getLaunchers()[0];

        // Move the fleet to the launcher - initialized to empty in the case where the launcher and starting planet are the same (returns null)
        GameObject[] currentGalaxyMovementList = new GameObject[0];
        if (this.gameObject.GetInstanceID() != closestCurrentLauncher.GetInstanceID()) {
            currentGalaxyMovementList = currentGalaxy.bfsGalaxy(this.gameObject, currentGalaxy.getLaunchers()[0]);
        }

        // Get the list of movements between the target galaxy launcher and destination planet
        Galaxy targetGalaxy = planetToMoveTo.GetComponent<Planet>().getGalaxy().GetComponent<Galaxy>();

        GameObject closestTargetLauncher = targetGalaxy.getLaunchers()[0];

        GameObject[] targetGalaxyMovementList = new GameObject[0];
        if (closestTargetLauncher.GetInstanceID() != planetToMoveTo.GetInstanceID()) {
            targetGalaxyMovementList = targetGalaxy.bfsGalaxy(closestTargetLauncher, planetToMoveTo);
        }
        
        // Combine the values
        // Because the implementation of the bfs assumes that the ship is on the starting node,
        // we need to add the target galaxy launcher planet to our list of movements
        GameObject[] targetLauncher = new GameObject[] { closestTargetLauncher };
        GameObject[] fullMovement = currentGalaxyMovementList.Concat(targetLauncher).Concat(targetGalaxyMovementList).ToArray();
        return fullMovement;
    }

    private bool isPlanetInSameGalaxy(GameObject otherPlanet) {
        return this.getGalaxy().GetInstanceID() == otherPlanet.GetComponent<Planet>().getGalaxy().GetInstanceID();
    }
}
