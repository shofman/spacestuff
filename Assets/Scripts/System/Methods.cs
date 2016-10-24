using UnityEngine;

public class Methods {
    public static void addGameObjectAsChild(GameObject parent, GameObject child) {
        child.transform.parent = parent.transform;
    }

    /**
     * Finds a game object of the children with a particular name, or creates it
     * @param name - The name of the parent gameObject we are wanting to find the game object
     * @return GameObject - created or found game object with the given name
     */
    public static GameObject createEmptyGameObject(GameObject currentObject, string name) {
        GameObject generic = null;
        bool found = false;

        Transform[] transforms = currentObject.GetComponentsInChildren<Transform>();
        foreach (Transform t in transforms) {
            if (t.gameObject.name == name) {
                generic = t.gameObject;
                found = true;
                break;
            }
        }
        if (!found) {
            generic = new GameObject(name);
            generic.transform.parent = currentObject.transform;
        }
        return generic;
    }

}