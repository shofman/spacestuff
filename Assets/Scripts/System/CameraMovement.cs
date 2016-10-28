using UnityEngine;
/**
 * Class allows the camera to move around the screen upon a click and drag
 * Also allows the user to zoom in with a right click (and drag)
 */
public class CameraMovement : MonoBehaviour {
    public float dragSpeedXY = 5;
    private Vector3 dragOrigin;

    const int zoomSizeMin = 13;
    const int zoomSizeMax = 130;

    public int boundary = 100; // distance from edge scrolling starts
    private int screenWidth;
    private int screenHeight;

    void Start() {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    void Update() {
        // Prevent the scrolling when the mouse has left
        if (!isMouseWithinScreenBoundaries()) {
            return;
        }

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
        
        // Top speed should be 4.3, lowest speed should be .7. Solving simultaneous equations - 13x + y = .7 and 130x + y = 4.3 gives 2/65 and 3/10
        float cameraZoomSpeedAdjustment = (Camera.main.fieldOfView * (2.0f / 65.0f)) + (3.0f/10.0f);

        // Shift the camera if the mouse is within 'boundary' distance of the corners
        if (isMouseWithinXBoundary()) {
            Vector3 pos = getCameraRelativePos(getCenterOfCamera());
            Vector3 move = new Vector3(pos.x * dragSpeedXY * cameraZoomSpeedAdjustment, 0, 0);
            if (isMouseWithinScreenBoundaries()) {
                transform.Translate(move, Space.World);
            }
        }
        if (isMouseWithinYBoundary()) {
            Vector3 pos = getCameraRelativePos(getCenterOfCamera());
            Vector3 move = new Vector3(0, pos.y * dragSpeedXY * cameraZoomSpeedAdjustment, 0);
            if (isMouseWithinScreenBoundaries()) {
                transform.Translate(move, Space.World);
            }
        }
    }

    /**
     * Whether the mouse is within the x boundary along the left and right sides of the screen
     */
    private bool isMouseWithinXBoundary() {
        return Input.mousePosition.x > screenWidth - boundary || Input.mousePosition.x < 0 + boundary;
    }

    /**
     * Whether the mouse is within the y boundary along the top and bottom sides of the screen
     */
    private bool isMouseWithinYBoundary() {
        return Input.mousePosition.y > screenHeight - boundary || Input.mousePosition.y < 0 + boundary;
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

    /**
     * Determines whether the mouse is within the screen
     */
    private bool isMouseWithinScreenBoundaries() {
        return (Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height && Input.mousePosition.x > 0 && Input.mousePosition.x < Screen.width);
    }
}