using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public interface IBreadthFirstSearchInterface {
    List<GameObject> getConnectedObjects();
    bool hasBeenVisited();
    void setVisited(bool visited);
    void setPrior(GameObject prior);
    GameObject getPrior();
    int getIndexForX();
    int getIndexForY();
    string getName();
    void setIndex(int x, int y);
}