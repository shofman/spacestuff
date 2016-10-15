using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BreadthFirstSearch : MonoBehaviour {

    /**
     * Perform a breadth first search on the type T
     * T - The gameobject we want to perform the bfs on
     * @type gameObject
     * @param initialNode - starting node for the search
     * @param Array representing the graph we are performing the search throughout
     * @param displayNames - boolean for displaying the name as we perform the search
     */
    public void breadthFirstSearchPlanets <T> (GameObject initialNode, GameObject[,] listOfPlanets, bool displayNames) where T : Component, IBreadthFirstSearchInterface {
        Queue searchQueue = new Queue();
        searchQueue.Enqueue(initialNode);

        for (int i=0; i<listOfPlanets.GetLength(0); i++) {
            for (int j=0; j<listOfPlanets.GetLength(1); j++) {
                listOfPlanets[i,j].GetComponent<T>().setVisited(false);
            }
        }

        while (searchQueue.Count != 0) {
            T currentPlanet = ((GameObject) searchQueue.Dequeue()).GetComponent<T>();
            if (displayNames) {
                Debug.Log(currentPlanet.getName());
            }
            //find the current index, use it to find the corresponding planet in the array, and set its visited status to true
            int xPos = currentPlanet.getIndexForX();
            int yPos = currentPlanet.getIndexForY();
            listOfPlanets[xPos,yPos].GetComponent<T>().setVisited(true);

            List<GameObject> connectedPlanets = new List<GameObject>(currentPlanet.getConnectedObjects());
            for (int i=0; i<connectedPlanets.Count; i++) {
                int x = connectedPlanets[i].GetComponent<T>().getIndexForX();
                int y = connectedPlanets[i].GetComponent<T>().getIndexForY();
                if (!searchQueue.Contains(connectedPlanets[i]) && !listOfPlanets[x,y].GetComponent<T>().hasBeenVisited()) {
                    searchQueue.Enqueue(connectedPlanets[i]);
                }
            }
        }
    }

    /**
     * Perform a breadth first search on the type T and return 
     * T - The gameobject we want to perform the bfs on
     * @type gameObject
     * @param initialNode - starting node for the search
     * @param Array representing the graph we are performing the search throughout
     * @param displayNames - boolean for displaying the name as we perform the search
     */
    public GameObject[] breadthFirstSearchPath <T> (GameObject[,] listOfPlanets, GameObject initialNode, GameObject targetNode) where T : Component, IBreadthFirstSearchInterface {
        if (initialNode.GetInstanceID() == targetNode.GetInstanceID()) {
            return null;
        }
        Queue searchQueue = new Queue();
        searchQueue.Enqueue(initialNode);

        for (int i=0; i<listOfPlanets.GetLength(0); i++) {
            for (int j=0; j<listOfPlanets.GetLength(1); j++) {
                listOfPlanets[i,j].GetComponent<T>().setVisited(false);
                listOfPlanets[i,j].GetComponent<T>().setPrior(null);
            }
        }

        while (searchQueue.Count != 0) {
            GameObject planetObject = (GameObject) searchQueue.Dequeue();
            T currentPlanet = planetObject.GetComponent<T>();
            if (planetObject.GetInstanceID() == targetNode.GetInstanceID()) {
                break;
            }
            //find the current index, use it to find the corresponding planet in the array, and set its visited status to true
            int xPos = currentPlanet.getIndexForX();
            int yPos = currentPlanet.getIndexForY();
            listOfPlanets[xPos,yPos].GetComponent<T>().setVisited(true);

            List<GameObject> connectedPlanets = new List<GameObject>(currentPlanet.getConnectedObjects());
            for (int i=0; i<connectedPlanets.Count; i++) {
                int x = connectedPlanets[i].GetComponent<T>().getIndexForX();
                int y = connectedPlanets[i].GetComponent<T>().getIndexForY();
                if (!searchQueue.Contains(connectedPlanets[i]) && !listOfPlanets[x,y].GetComponent<T>().hasBeenVisited()) {
                    connectedPlanets[i].GetComponent<T>().setPrior(planetObject);
                    searchQueue.Enqueue(connectedPlanets[i]);
                }
            }
        }
        Stack<GameObject> planetsToVisit = new Stack<GameObject>();
        GameObject planetList = targetNode.GetComponent<T>().getPrior();
        planetsToVisit.Push(targetNode);

        while (planetList != null && planetList.GetInstanceID() != initialNode.GetInstanceID()) {
            planetsToVisit.Push(planetList);
            planetList = planetList.GetComponent<T>().getPrior();
        }
        return planetsToVisit.ToArray();
    }

}