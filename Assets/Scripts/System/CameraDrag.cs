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

    public int boundary = 50; // distance from edge scrolling starts
    private int screenWidth;
    private int screenHeight;

    void Start() {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButton(0)) {
            Vector3 pos = getCameraRelativePos(dragOrigin);
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
        
        // Shift the camera if the mouse is within 'boundary' distance of the corners
        if (Input.mousePosition.x > screenWidth - boundary || Input.mousePosition.x < 0 + boundary) {
            Vector3 pos = getCameraRelativePos(getCenterOfCamera());
            Vector3 move = new Vector3(pos.x * dragSpeedXY, 0, 0);
            transform.Translate(move, Space.World);
        }
        if (Input.mousePosition.y > screenHeight - boundary || Input.mousePosition.y < 0 + boundary) {
            Vector3 pos = getCameraRelativePos(getCenterOfCamera());
            Vector3 move = new Vector3(0, pos.y * dragSpeedXY, 0);
            transform.Translate(move, Space.World);
        }
    }

    /**
     * Returns the camera position relative to a specific point (usually center of the screen, or the start of a drag)
     */
    private Vector3 getCameraRelativePos(Vector3 relativeTo) {
        return Camera.main.ScreenToViewportPoint(Input.mousePosition - relativeTo);
    }

    /**
     * Returns a Vector3 describing the center of the camera
     */
    private Vector3 getCenterOfCamera() {
        return new Vector3(Screen.width / 2, Screen.height / 2, 0);
    }
}