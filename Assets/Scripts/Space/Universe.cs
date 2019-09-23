using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Universe : MonoBehaviour {
    public GameObject galaxy;
    public int numberOfGalaxies;

    GameObject[,] listOfGalaxies;
    System.Random random;
    Queue<string> availableNames;

    GameObject universeUI;
    GameObject planetMenuDisplay;

    private int galaxyIndex;

    void Awake() {
        // 5000 seems to be alright
        NameGenerator nameGenerator = new NameGenerator(numberOfGalaxies * 220);
        availableNames = nameGenerator.generatePlanetNamesAsQueue();

        universeUI = GameObject.Find("/UniverseDisplay");
        planetMenuDisplay = GameObject.Find("/PlanetMenu");

        listOfGalaxies = new GameObject[numberOfGalaxies,1];

        for (int i=0; i<listOfGalaxies.GetLength(0); i++) {
            GameObject galaxyCreated = (GameObject)Instantiate(galaxy);
            int yPos = 0;
            if (i%2==1) {
                yPos = -100;
            }
            galaxyCreated.transform.Translate(i*220, yPos, 0);
            Galaxy galaxyScript = galaxyCreated.GetComponent<Galaxy>();

            // Each planet needs a name, plus the galaxy should be named
            int totalNamesNeeded = (galaxyScript.planetRows * galaxyScript.planetColumns) + 1;
            galaxyScript.setIndex(i, 0);
            // Transfer over the total needed amount to a new list
            List<string> galaxyNames = new List<string>();
            for (int j=0; j<totalNamesNeeded; j++) {
                galaxyNames.Add(availableNames.Dequeue());
            }

            bool validGalaxy = galaxyScript.createGalaxy(galaxyNames);
            if (!validGalaxy) {
                Debug.Log("WE HAVE CREATED NOT A VALID GALAXY WITH " + galaxyScript.getName());
            }

            // galaxyScript.createGalaxyUIElements(universeUI);
            listOfGalaxies[i,0] = galaxyCreated;
        }

        // Ensure that the galaxies do not overlap with each other
        for (int i=0; i<listOfGalaxies.GetLength(0); i++) {
            for (int j=i+1; j<listOfGalaxies.GetLength(0); j++) {
                Vector4 galaxyABoundingBox = listOfGalaxies[i,0].GetComponent<Galaxy>().getBoundingBox();
                Vector4 galaxyBBoundingBox = listOfGalaxies[j,0].GetComponent<Galaxy>().getBoundingBox();
                bool intersection = checkIfBoundingBoxesIntersect(galaxyABoundingBox, galaxyBBoundingBox);

                if (intersection) {
                    listOfGalaxies[j,0].transform.Translate(galaxyABoundingBox.y - galaxyBBoundingBox.x, galaxyABoundingBox.w -  galaxyBBoundingBox.z, 0);
                    listOfGalaxies[j,0].GetComponent<Galaxy>().displayConnectedPlanets();
                }
            }
        }

        galaxyIndex = 0;
    }

    // Use this for initialization
    void Start () {
        connectGalaxies();

        // Disable the planet menu
        planetMenuDisplay.SetActive(false);
    }

    void connectGalaxies() {
        for (int i=1; i<listOfGalaxies.GetLength(0); i++) {
            addGalaxy(listOfGalaxies[i,0], listOfGalaxies[i-1,0]);
        }
    }

    void addGalaxy(GameObject galaxyToAdd, GameObject addedGalaxy) {
        galaxyToAdd.GetComponent<Galaxy>().addGalacticRoute(addedGalaxy);
        addedGalaxy.GetComponent<Galaxy>().addGalacticRoute(galaxyToAdd);
    }

    // Update is called once per frame
    void Update () {
        if(Input.GetKeyDown("b")) {
            for (int i=0; i<listOfGalaxies.GetLength(0); i++) {
                listOfGalaxies[i,0].GetComponent<Galaxy>().findCrossingEdges();
            }
        }
        if (Input.GetKeyDown("g")) {
            galaxyIndex += 1;
            galaxyIndex %= listOfGalaxies.GetLength(0);
            GameObject currentGalaxy = listOfGalaxies[galaxyIndex, 0];
            Debug.Log("Cycling to galaxy " + currentGalaxy.GetComponent<Galaxy>().getName());
            Camera.main.transform.position = new Vector3(currentGalaxy.transform.position.x, currentGalaxy.transform.position.y, Camera.main.transform.position.z);
        }
    }

    /**
     * Returns whether two bounding boxes (represented by Vector4s) intersect
     * The vector4s should be in the form of (xMin, xMax, yMin, yMax)
     * if (RectA.Left < RectB.Right && RectA.Right > RectB.Left &&
     RectA.Top < RectB.Bottom && RectA.Bottom > RectB.Top ) 
     */
    private bool checkIfBoundingBoxesIntersect(Vector4 rectA, Vector4 rectB) {
        return (rectA.x < rectB.y && rectA.y > rectB.x && rectA.z < rectB.w && rectA.w > rectB.z);
    }

    /**
     * Returns a list of all planets belonging to a particular allegiance
     * TODO - this breaks Law of Demeter - should move planet fetching code into galaxy
     */
    public List<GameObject> findAllPlanetsBelongingTo(Color c) {
        List<GameObject> planetsBelonging = new List<GameObject>();
        for (int i=0; i<listOfGalaxies.GetLength(0); i++) {
            List<GameObject> planets = listOfGalaxies[i,0].GetComponent<Galaxy>().getListOfPlanets();
            foreach (GameObject planet in planets) {
                if (planet.GetComponent<Planet>().getAllegiance() == c) {
                    planetsBelonging.Add(planet);
                }
            }
        }
        return planetsBelonging;
    }
}
