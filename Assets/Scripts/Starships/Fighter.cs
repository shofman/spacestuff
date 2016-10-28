using UnityEngine;

public class Fighter : Ship {
    new void Update() {
        if (fleet != null) {
            transform.RotateAround(fleet.transform.position, Vector3.up, 40 * Time.deltaTime);
        }
    }
}