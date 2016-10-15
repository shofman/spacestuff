using UnityEngine;

public class Methods {
    public static void addGameObjectAsChild(GameObject parent, GameObject child) {
        child.transform.parent = parent.transform;
    }
}