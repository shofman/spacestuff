using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
    public int wealth;
    public Color allegiance;

    public GameObject universe;

    public GameObject resourceDisplay;

    private ResourceDisplay resources;

    void Awake() {
        allegiance = Color.blue;
        resources = resourceDisplay.GetComponent<ResourceDisplay>();
    }

    void Start() {
        wealth = calculateWealth();
        resources.setPlayerDisplay(wealth);
    }

    void Update() {

    }

    private int calculateWealth() {
        List<GameObject> playersPlanets = universe.GetComponent<Universe>().findAllPlanetsBelongingTo(allegiance);
        int amount = 0;
        foreach (GameObject planet in playersPlanets) {
            amount += planet.GetComponent<Planet>().getSpiceValue();
        }
        return amount;
    }
}
