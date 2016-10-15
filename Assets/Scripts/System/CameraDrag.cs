using UnityEngine;
/**
 * Class allows the camera to move around the screen upon a click and drag
 * Also allows the user to zoom in with a right click (and drag)
 */
public class CameraDrag : MonoBehaviour {
    public float dragSpeedXY = 5;
    private Vector3 dragOrigin;

    const int zoomSizeMin = 13;
    const int zoomSizeMax = 100;

    void Update() {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButton(0)) {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(pos.x * dragSpeedXY, pos.y * dragSpeedXY, 0);

            transform.Translate(move, Space.World);
            return;
        }


         if (Input.GetAxis("Mouse ScrollWheel") < 0) { // forward
            Camera.main.fieldOfView = Camera.main.fieldOfView + 5;
         }
         if (Input.GetAxis("Mouse ScrollWheel") > 0) {// back
            Camera.main.fieldOfView = Camera.main.fieldOfView - 5;
         }
         Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, zoomSizeMin, zoomSizeMax );
    }
}