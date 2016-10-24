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

    void Awake() {
        // 5000 seems to be alright
        NameGenerator nameGenerator = new NameGenerator(numberOfGalaxies * 22);
        availableNames = nameGenerator.generatePlanetNamesAsQueue();

        universeUI = GameObject.Find("/UniverseDisplay");
        planetMenuDisplay = GameObject.Find("/PlanetMenu");

        listOfGalaxies = new GameObject[numberOfGalaxies,1];

        for (int i=0; i<listOfGalaxies.GetLength(0); i++) {
            GameObject galaxyCreated = (GameObject)Instantiate(galaxy);
            galaxyCreated.transform.Translate(i*220, 0, 0);
            Galaxy galaxyScript = galaxyCreated.GetComponent<Galaxy>();

            // Each planet needs a name, plus the galaxy should be named
            int totalNamesNeeded = (galaxyScript.planetRows * galaxyScript.planetColumns) + 1;
            galaxyScript.setIndex(i, 0);
            // Transfer over the total needed amount to a new list
            List<string> galaxyNames = new List<string>();
            for (int j=0; j<totalNamesNeeded; j++) {
                galaxyNames.Add(availableNames.Dequeue());
            }

            galaxyScript.createGalaxy(galaxyNames);
            // galaxyScript.createGalaxyUIElements(universeUI);
            listOfGalaxies[i,0] = galaxyCreated;
        }
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
            listOfGalaxies[0,0].GetComponent<Galaxy>().findCrossingEdges();
        }
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
