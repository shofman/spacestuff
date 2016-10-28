using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;


public class Galaxy : MonoBehaviour, IBreadthFirstSearchInterface {
    // The planet object
    public GameObject planet;

    //UI element that will display the name of the galaxy
    public GameObject galaxyNameUI;

    //Number of planets in terms of rows and columns
    public int planetRows;
    public int planetColumns;

    // The launcher for transportation between galaxies
    public GameObject launcherObject;

    //Name of the object that houses the trade routes for this galaxy
    private const string TRADE_ROUTE_HOLDER = "TradeRouterHolder";

    // List of planets within this galaxy
    GameObject[,] listOfPlanets;

    // Empty game object to hold collections of gameObject
    GameObject planetsHolder;
    GameObject tradeRouteHolder;

    // List of potential planet names for this galaxy
    List<string> planetNames;

    // Random number generator
    System.Random random;

    // List of all connected galaxies to this particular galaxy
    List<GameObject> connectedGalaxies;

    // Whether we have visited this galaxy before (for searches)
    bool hasVisited = false;

    // What the previous galaxy we visited before this one (for identifying distances in bfs)
    GameObject priorGameObject;

    // The name of this galaxy
    public string galaxyName = "";

    //Initial position in global array holding position
    int universePositionX = -1;
    int universePositionY = -1;

    //List of Launchers (transports between galaxies) in this galaxy
    private List<GameObject> launchersList;

    private int frameWidth = 20;
    private int frameHeight = 20;
    /**
     * On intialization, create necessary child gameObject, setup random generator, and initialize required variables
     */
    void Awake() {
        planetsHolder = Methods.createEmptyGameObject(this.gameObject, "PlanetsHolder");
        tradeRouteHolder = Methods.createEmptyGameObject(this.gameObject, TRADE_ROUTE_HOLDER);
        random = new System.Random();
        connectedGalaxies = new List<GameObject>();
        launchersList = new List<GameObject>();

        planetRows = 5; //random.Next(2,7);
        planetColumns = 5; //random.Next(2,7);
    }


    /**
     * Responsible for setting up and creating the planets in a particular galaxy
     *     * Will position them in grid
     *     * Sets colors to either red or blue
     *     * Creates random connected graph based on available nodes
     * @param newPlanetNames - List of randomly generated planet names
     */
    public void createGalaxy(List<string> newPlanetNames) {
        planetNames = newPlanetNames;

        // Set galaxy name first, and remove it from list (no duplicates)
        galaxyName = planetNames[0];
        planetNames.RemoveAt(0);

        listOfPlanets = new GameObject[planetColumns,planetRows];

        int totalBluePlanets = planetRows*planetColumns/2;
        int totalRedPlanets = totalBluePlanets;

        bool hasCreatedLauncher = false;
        
        for (int i=0; i<planetRows*planetColumns; i++) {
            GameObject planetCreated = (GameObject)Instantiate(planet);
            planetCreated.transform.SetParent(planetsHolder.transform);

            int xPos = ((i/planetRows)*20)-20 + (int) gameObject.transform.position.x;
            int yPos = ((i%planetRows)*20)-20 + (int) gameObject.transform.position.y;
            Planet newPlanet = planetCreated.GetComponent<Planet>();
            newPlanet.setPosition(xPos, yPos);
            newPlanet.setGalaxy(this.gameObject);
            newPlanet.setName(planetNames[i]);
            newPlanet.setIndex(i/planetRows, i%planetRows);
            listOfPlanets[i/planetRows,i%planetRows] = planetCreated;

            int shouldCreatelauncher = random.Next(0,100);
            if ((shouldCreatelauncher > 85 || i == (planetRows*planetColumns)-1) && !hasCreatedLauncher) {
                hasCreatedLauncher = true;
                GameObject newLauncher = (GameObject) Instantiate(launcherObject);
                newLauncher.transform.SetParent(planetCreated.transform);
                newLauncher.transform.position = planetCreated.transform.position;
                newLauncher.transform.position += new Vector3(4,4,0);
                newLauncher.transform.eulerAngles = new Vector3(0,0,320);

                newPlanet.setLauncher(true);
                launchersList.Add(planetCreated);
            }

            if (totalBluePlanets > 0 && totalRedPlanets > 0) {
                int randomValue = random.Next(0,100);
                if (randomValue < 50) {
                    newPlanet.changeColorAndSetUnits(Color.blue, 2);
                    totalBluePlanets--;
                } else {
                    newPlanet.changeColorAndSetUnits(Color.red, 2);
                    totalRedPlanets--;
                }
            } else if (totalBluePlanets > 0) {
                newPlanet.changeColorAndSetUnits(Color.blue, 2);
                totalBluePlanets--;
            } else if (totalRedPlanets > 0) {
                newPlanet.changeColorAndSetUnits(Color.red, 2);
                totalRedPlanets--;
            }

            newPlanet.createRandomTexture();
        }
        connectAllPlanets();
        removeConnections();
        for (int i=0; i<listOfPlanets.GetLength(0); i++) {
            for (int j=0; j<listOfPlanets.GetLength(1); j++) {
                removeEdgelessPlanets(listOfPlanets[i,j]);
            }
        }
        randomlyMovePlanets();
        for (int i=0; i<50; i++) {
            temperatureForcedDirectedGraph();
        }
        // TODO - check for edge crossings here - if so, delete object and return false
        displayConnectedPlanets();
    }

    /**
     * Create the UI display for the particular galaxy 
     * @param  GameObject - The canvas that houses the UI elements for displaying the galaxy info
     */
    public void createGalaxyUIElements(GameObject canvasDisplay) {
        GameObject galaxyUI = (GameObject) Instantiate(galaxyNameUI);
        galaxyUI.transform.SetParent(canvasDisplay.transform, false);
        galaxyUI.transform.Translate(gameObject.transform.position.x, gameObject.transform.position.y, 0);
        galaxyUI.SetActive(false);
        canvasDisplay.GetComponent<GalaxyDisplay>().addGalaxyDisplay(galaxyUI, getName());
    }

    // Use this for initialization
    void Start () {
    }

    /**
     * Connects every planet within a galaxy to its neighbors
     */
    void connectAllPlanets() {
        for (int i=0; i<planetRows; i++) {
            for (int j=0; j<planetColumns; j++) {
                if (j+1 < planetColumns) {
                    addPlanet(listOfPlanets[j,i], listOfPlanets[j+1,i]);
                }
                if (i+1 < planetRows) {
                    for (int k=-1; k<2; k++) {
                        if (j+k > -1 && j+k < planetColumns) {
                            addPlanet(listOfPlanets[j+k,i+1], listOfPlanets[j,i]);
                        }
                    }
                }
            }
        }
    }

    /**
     * Randomly remove the trade routes between planets
     */
    void removeConnections() {
        for (int i=0; i<planetRows; i++) {
            for (int j=0; j<planetColumns; j++) {
                List<GameObject> connectedPlanets = new List<GameObject>(listOfPlanets[j,i].GetComponent<Planet>().getConnectedObjects());
                foreach (GameObject planet in connectedPlanets) {
                    int randomValue = random.Next(0,100);
                    if (randomValue < 70) {
                        //Remove the planet from the connections for the current planet
                        //Find the removed planet in our galaxy list, and remove the current planet from it
                        listOfPlanets[j,i].GetComponent<Planet>().removeConnectedPlanet(planet);
                        for (int x=0; x<planetRows; x++) {
                            for (int y=0; y<planetColumns; y++) {
                                if (listOfPlanets[y,x].GetComponent<Planet>().getName() == planet.GetComponent<Planet>().getName()) {
                                    listOfPlanets[y,x].GetComponent<Planet>().removeConnectedPlanet(listOfPlanets[j,i]);
                                }
                            }
                        }

                        //Check to see if we are still connected
                        gameObject.GetComponent<BreadthFirstSearch>().breadthFirstSearchPlanets<Planet>(listOfPlanets[j,i], listOfPlanets, false);

                        for (int a=0; a<listOfPlanets.GetLength(0); a++) {
                            for (int b=0; b<listOfPlanets.GetLength(1); b++) {
                                if (!listOfPlanets[a,b].GetComponent<Planet>().hasBeenRemoved() && !listOfPlanets[a,b].GetComponent<Planet>().hasBeenVisited()) {
                                    //The planet is still there, and hasn't been visited, so we've broken the chain. Readd the planets
                                    listOfPlanets[j,i].GetComponent<Planet>().addTradeRoute(planet);
                                    for (int x=0; x<planetRows; x++) {
                                        for (int y=0; y<planetColumns; y++) {
                                            if (listOfPlanets[y,x].GetComponent<Planet>().getName() == planet.GetComponent<Planet>().getName()) {
                                                listOfPlanets[y,x].GetComponent<Planet>().addTradeRoute(listOfPlanets[j,i]);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //Remove planets whose edges we've removed all of
                        for (int x=0; x<listOfPlanets.GetLength(0); x++) {
                            for (int y=0; y<listOfPlanets.GetLength(1); y++) {
                                removeEdgelessPlanets(listOfPlanets[x,y]);
                            }
                        }
                    }
                }
            }
        }
    }

    /**
     * Removes all planets that do not have any trade routes leading to them
     * @param  GameObject planet - the current planet we are investigating
     */
    void removeEdgelessPlanets(GameObject planet) {
        List<GameObject> connectedPlanets = new List<GameObject>(planet.GetComponent<Planet>().getConnectedObjects());
        if (connectedPlanets.Count == 0) {
            int xPos = planet.GetComponent<Planet>().getIndexForX();
            int yPos = planet.GetComponent<Planet>().getIndexForY();
            listOfPlanets[xPos,yPos].GetComponent<Planet>().setRemoval();
            Destroy(planet);
        }
    }

    /**
     * Display the connections between the planets as line renderers
     */
    public void displayConnectedPlanets() {
        for (int i=0; i<planetRows; i++) {
            for (int j=0; j<planetColumns; j++) {
                listOfPlanets[j,i].GetComponent<Planet>().displayTradeRoutes(tradeRouteHolder);
            }
        }
    }

    /**
     * Add the planetary connection between the two planets at the planet level (two-way link)
     * @param The first planet we are connecting to 
     * @param The second planet we are connecting to
     */
    void addPlanet(GameObject planetToAdd, GameObject addedPlanet) {
        Planet planetScript = planetToAdd.GetComponent<Planet>();
        planetScript.addTradeRoute(addedPlanet);
        Planet addedPlanetScript = addedPlanet.GetComponent<Planet>();
        addedPlanetScript.addTradeRoute(planetToAdd);
    }
    
    // Update is called once per frame
    void Update () {

    }

    /**
     * Add which galaxies this particular galaxy is connected to
     * @param GameObject galaxy - the galaxy we want to connect this to
     */
    public void addGalacticRoute(GameObject galaxy) {
        this.connectedGalaxies.Add(galaxy);
    }

    // BREADTH FIRST SEARCH VARIABLES
    /**
     * Whether or not we have been visited during a breadth first search
     * @return If we have already visited this galaxy during a search
     */
    public bool hasBeenVisited() {
        return hasVisited;
    }

    /**
     * Sets whether we have visited the planet or not
     * @param bool visited - whether we have visited the galaxy before
     */
    public void setVisited(bool visited) {
        hasVisited = visited;
    }

    public void setPrior(GameObject prior) {
        priorGameObject = prior;
    }

    public GameObject getPrior() {
        return priorGameObject;
    }

    /**
     * Finds where in this galaxy that the trade routes are crossing
     */
    public List<PlanetLine> findCrossingEdges() {
        // Get all the current trade routes
        List<PlanetLine> allTradeRoutes = new List<PlanetLine>();
        for (int i=0; i<planetRows; i++) {
            for (int j=0; j<planetColumns; j++) {
                GameObject currentPlanet = listOfPlanets[j,i];
                List<GameObject> connectedPlanets = new List<GameObject>(currentPlanet.GetComponent<Planet>().getConnectedObjects());
                foreach (GameObject planet in connectedPlanets) {
                    allTradeRoutes.Add(new PlanetLine(currentPlanet, planet));
                }
            }
        }
        bool foundIntersection = false;

        List<PlanetLine> intersectionLines = new List<PlanetLine>();
        foreach (PlanetLine firstLine in allTradeRoutes) {
            foreach (PlanetLine secondLine in allTradeRoutes) {
                if (!firstLine.containsSamePoint(secondLine) && !intersectionAlreadyFound(firstLine, secondLine, intersectionLines)) {
                    bool otherIntersection = lineIntersects(firstLine.planetOne, firstLine.planetTwo, secondLine.planetOne, secondLine.planetTwo);
                    if (otherIntersection) {
                        foundIntersection = true;
                        intersectionLines.Add(firstLine);
                        intersectionLines.Add(secondLine);
                        Debug.Log(firstLine.firstPlanet.GetComponent<Planet>().getName() + " " + firstLine.secondPlanet.GetComponent<Planet>().getName() + " " + secondLine.firstPlanet.GetComponent<Planet>().getName() + " " + secondLine.secondPlanet.GetComponent<Planet>().getName());
                    }
                }
            }
        }
        return intersectionLines;
    }

    /**
     * Sets the indices within the parent array (in this case, universe) for easy access during search
     * @param int xPos - The first parameter in the two dimensional array this value is stored within in the parent array
     * @param int yPos - The second parameter in the two dimensional array this value is stored within in the parent array
     */
    public void setIndex(int xPos, int yPos) {
        universePositionX = xPos;
        universePositionY = yPos;
    }

    /**
     * Returns the first index for accessing this value in the storage two-dimensional array in the parent
     * @return int - the position this value is in within the parent array
     */
    public int getIndexForX() {
        return universePositionX;
    }

    /**
     * Returns the second index for accessing this value in the storage two-dimensional array in the parent
     * @return int - the position this value is in within the parent array
     */
    public int getIndexForY() {
        return universePositionY;
    }

    /**
     * Returns the name of this galaxy
     * @return string 
     */
    public string getName() {
        return galaxyName;
    }

    /**
     * Returns the number of galaxies this function is connected to 
     * @return List<GameObject> a list of all connected galaxies to this galaxy
     */
    public List<GameObject> getConnectedObjects() {
        return this.connectedGalaxies;
    }

    /**
     * Returns the list of launchers this galaxy has
     */
    public List<GameObject> getLaunchers() {
        return launchersList;
    }

    /**
     * Returns a list of all the planets within this galaxy
     */
    public List<GameObject> getListOfPlanets() {
        List<GameObject> planetList = new List<GameObject>();
        for (int i=0; i<listOfPlanets.GetLength(0); i++) {
            for (int j=0; j<listOfPlanets.GetLength(1); j++) {
                planetList.Add(listOfPlanets[i,j]);
            }
        }
        return planetList;
    }

    /** 
     * Performs a bfs on this galaxy
    */
    public GameObject[] bfsGalaxy(GameObject startNode, GameObject targetNode) {
        if (startNode == null) {
            Debug.Log("START NODE IS NULL IN bfsGalaxy");
        } else if (targetNode == null) {
            Debug.Log("TARGET NODE IS NULL IN bfsGalaxy");
        }
        return gameObject.GetComponent<BreadthFirstSearch>().breadthFirstSearchPath<Planet>(listOfPlanets, startNode, targetNode);
    }

    public class PlanetLine : IEquatable<PlanetLine>{
        public GameObject firstPlanet;
        public GameObject secondPlanet;
        public Vector3 planetOne;
        public Vector3 planetTwo;
        public PlanetLine(GameObject firstPlanet, GameObject secondPlanet) {
            this.firstPlanet = firstPlanet;
            this.secondPlanet = secondPlanet;
            this.planetOne = firstPlanet.transform.position;
            this.planetTwo = secondPlanet.transform.position;
        }

        public bool Equals( PlanetLine other ) {
            if (other != null ) {
                return (this.firstPlanet.GetInstanceID() == other.firstPlanet.GetInstanceID() &&
                        this.secondPlanet.GetInstanceID() == other.secondPlanet.GetInstanceID()) ||
                        (this.firstPlanet.GetInstanceID() == other.secondPlanet.GetInstanceID() && 
                         this.secondPlanet.GetInstanceID() == other.firstPlanet.GetInstanceID());
            }
            return false;
        }

        public bool containsSamePoint(PlanetLine otherLine) {
            return this.planetOne == otherLine.planetTwo || 
                   this.planetTwo == otherLine.planetOne ||
                   this.planetOne == otherLine.planetOne ||
                   this.planetTwo == otherLine.planetTwo;
        }
    }

    /**
     * Randomly moves planets 
     */
    private void randomlyMovePlanets() {
        for (int i=0; i<planetRows; i++) {
            for (int j=0; j<planetColumns; j++) {
                GameObject currentPlanet = listOfPlanets[j,i];

                if (isInBottomLeftQuadrant(i, j)) {
                    movePlanetToRandomPosition(currentPlanet, false, false);
                }

                if (isInBottomRightQuadrant(i,j)) {
                    movePlanetToRandomPosition(currentPlanet, true, false);
                }

                if (isInTopRightQuadrant(i,j)) {
                    movePlanetToRandomPosition(currentPlanet, true, true);
                }

                if (isInTopLeftQuadrant(i,j)) {
                    movePlanetToRandomPosition(currentPlanet, false, true);
                }
            }
        }
    }

    /**
     * Moves the target planet to a random position
     * @param planet - the current planet we want to move
     * @shouldAddX - Whether we want to add the random x value or subtract it from our current position
     * @shouldAddY - Whether we want to add the random y value or subtract it from our current position
     */
    private void movePlanetToRandomPosition(GameObject planet, bool shouldAddX, bool shouldAddY) {
        Vector3 currentPosition = planet.transform.position;
        Vector2 randomMovement = new Vector2(random.Next(0,30), random.Next(0,30));
        float newXPos = currentPosition.x + (shouldAddX ? randomMovement.x : -randomMovement.x);
        float newYPos = currentPosition.y + (shouldAddY ? randomMovement.y : -randomMovement.y);
        planet.transform.position = new Vector3(newXPos, newYPos, currentPosition.z);
    }

    /**
     * Calculates the force that pulls the planets together
     */
    private float calcAttractiveForce(float optimalDistance, float actualDistance) {
        return (float) Math.Pow(actualDistance, 2) / optimalDistance;
    }

    /**
     * Calculates the force that pushes the planets apart
     */
    private float calcRepulsiveForce(float optimalDistance, float actualDistance) {
        return (float) Math.Pow(optimalDistance, 2) / actualDistance;
    }

    /**
     * Creates a forced directed graph from https://cs.brown.edu/~rt/gdhandbook/chapters/force-directed.pdf
     */
    private void temperatureForcedDirectedGraph() {
        int area = frameWidth * frameHeight;
        List<GameObject> allPlanets = getListOfPlanets();
        float optimalDistance = (float) (3.0f * Math.Sqrt(area/allPlanets.Count));

        /**
         * For each planet, push them apart from the others 
         */
        foreach (GameObject currentPlanet in allPlanets) {
            currentPlanet.GetComponent<Planet>().setDisplacement(new Vector3(0,0,0));
            foreach (GameObject otherPlanet in allPlanets) {
                if (currentPlanet.GetInstanceID() != otherPlanet.GetInstanceID()) {
                    Vector3 differenceVector = currentPlanet.transform.position - otherPlanet.transform.position;
                    Vector3 currentDisplacement = currentPlanet.GetComponent<Planet>().getDisplacement();
                    Vector3 newDisplacement = currentDisplacement + (differenceVector.normalized * calcRepulsiveForce(optimalDistance, differenceVector.magnitude));
                    currentPlanet.GetComponent<Planet>().setDisplacement(newDisplacement);
                }
            }
        }

        /**
         * For each edge, pull their two planets closer towards each other
         */
        List<PlanetLine> graphEdges = new List<PlanetLine>();
        foreach (GameObject currentPlanet in allPlanets) {
            List<GameObject> connections = currentPlanet.GetComponent<Planet>().getConnectedObjects();
            foreach(GameObject otherPlanet in connections) {
                PlanetLine edge = new PlanetLine(currentPlanet, otherPlanet);
                if (!graphEdges.Contains(edge)) {
                    graphEdges.Add(edge);
                    Vector3 differenceVector = currentPlanet.transform.position - otherPlanet.transform.position;
                    Vector3 currentDisplacement = currentPlanet.GetComponent<Planet>().getDisplacement();
                    Vector3 otherDisplacement = otherPlanet.GetComponent<Planet>().getDisplacement();
                    Vector3 newCurrentDisplacement = currentDisplacement - (differenceVector.normalized * calcAttractiveForce(optimalDistance, differenceVector.magnitude));
                    Vector3 newOtherDisplacement = otherDisplacement + (differenceVector.normalized * calcAttractiveForce(optimalDistance, differenceVector.magnitude));
                    currentPlanet.GetComponent<Planet>().setDisplacement(newCurrentDisplacement);
                    otherPlanet.GetComponent<Planet>().setDisplacement(newOtherDisplacement);
                }
            }
        }

        // Move the planets by their calculated forces
        foreach (GameObject currentPlanet in allPlanets) {
            Vector3 currentPos = currentPlanet.transform.position;
            Vector3 displacementVector = currentPlanet.GetComponent<Planet>().getDisplacement();
            currentPlanet.transform.position = currentPos + ((displacementVector.normalized) * Math.Min(displacementVector.magnitude, getTemp()));
        }
    }

    /**
     * Returns a minimum distance that a planet can move
     */
    private float getTemp() {
        return (float) frameWidth / 10;
    }

    /**
     * Returns whether the planet is in the bottom left of the galaxy
     */
    private bool isInBottomLeftQuadrant(int rowPos, int columnPos) {
        return (rowPos <= (planetRows-1)/2) && (columnPos <= (planetColumns-1)/2);
    }

    /**
     * Returns whether the planet is in the bottom right part of the galaxy
     * In the case where there are an odd amount of columns 
     */
    private bool isInBottomRightQuadrant(int rowPos, int columnPos) {
        if (planetColumns % 2 == 1) {
            return (rowPos <= (planetRows-1)/2) && (columnPos >= (planetColumns-1)/2);
        } else {
            return (rowPos <= (planetRows-1)/2) && (columnPos > (planetColumns-1)/2);        
        }
    }

    /**
     * Returns whether the planet is in the top right part of the galaxy
     * If there is an odd amount of rows or columns, we include the remaining one when dividing by two in both 
     * the top and the right quadrants
     */
    private bool isInTopRightQuadrant(int rowPos, int columnPos) {
        bool isInRow = (planetRows % 2 == 1) ? (rowPos >= (planetRows-1)/2) : (rowPos > (planetRows-1)/2);
        bool isInColumn  = (planetColumns % 2 == 1) ? (columnPos >= (planetColumns-1)/2) : (columnPos > (planetColumns-1)/2);
        return isInRow && isInColumn;
    }

    /**
     * Returns whether the planet is in the top left of the galaxy
     * If there are an odd amount of rows, there's one that won't belong to either top or bottom
     * In that case, include it in both top and bottom
     */
    private bool isInTopLeftQuadrant(int rowPos, int columnPos) {
        if (planetRows % 2 == 1) { 
            return (rowPos >= (planetRows-1)/2) && (columnPos <= (planetColumns-1)/2);
        } else {
            return (rowPos > (planetRows-1)/2) && (columnPos <= (planetColumns-1)/2);
        }
    }

    /**
     * Returns whether or not the intersection has already been identified (since each trade route is two-way, there are duplicates)
     */
    private bool intersectionAlreadyFound(PlanetLine firstLine, PlanetLine secondLine, List<PlanetLine> intersectionsList) {
        return (intersectionsList.Contains(firstLine) && intersectionsList.Contains(secondLine));
    }

    /**
     * Determines whether a line intersects another based off the anchor points
     */
    private bool lineIntersects(Vector3 pointOne, Vector3 pointTwo, Vector3 pointThree, Vector3 pointFour) {

        float lineOneXSlope, lineOneYSlope, lineTwoXSlope, lineTwoYSlope;
        lineOneXSlope = pointTwo.x - pointOne.x;
        lineOneYSlope = pointTwo.y - pointOne.y;
        lineTwoXSlope = pointFour.x - pointThree.x;
        lineTwoYSlope = pointFour.y - pointThree.y;

        float s, t;
        s = (-lineOneYSlope * (pointOne.x - pointThree.x) + lineOneXSlope * (pointOne.y - pointThree.y)) / (-lineTwoXSlope * lineOneYSlope + lineOneXSlope * lineTwoYSlope);
        t = ( lineTwoXSlope * (pointOne.y - pointThree.y) - lineTwoYSlope * (pointOne.x - pointThree.x)) / (-lineTwoXSlope * lineOneYSlope + lineOneXSlope * lineTwoYSlope);

        return (s >= 0 && s <= 1 && t >= 0 && t <= 1);
    }
}
