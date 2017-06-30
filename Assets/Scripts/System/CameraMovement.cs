using UnityEngine;
/**
 * Class allows the camera to move around the screen upon a click and drag
 * Also allows the user to zoom in with a right click (and drag)
 */
public class CameraMovement : MonoBehaviour {
    public float dragSpeedXY = 5;

    const int zoomSizeMin = 13;
    const int zoomSizeMax = 130;

    private int screenWidth;
    private int screenHeight;

    private Vector3 startingPos;

    void Start() {
        startingPos = Camera.main.transform.position;
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    void Update() {
        if (Input.GetKeyDown("space")) {
            Camera.main.transform.position = startingPos;
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


        Vector3 moveHorizonal = new Vector3(Input.GetAxis("Horizontal") * dragSpeedXY * cameraZoomSpeedAdjustment, 0, 0);
        transform.Translate(moveHorizonal, Space.World);
        Vector3 moveVertical = new Vector3(0, Input.GetAxis("Vertical") * dragSpeedXY * cameraZoomSpeedAdjustment, 0);
        transform.Translate(moveVertical, Space.World);
    }
}