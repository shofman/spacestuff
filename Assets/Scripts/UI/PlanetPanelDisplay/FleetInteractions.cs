using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
 
public class FleetInteractions : MonoBehaviour, IPointerClickHandler {
    private GameObject fleet;
    private GameObject shipDisplay;
 
    /**
     * Detect click events on the fleet display
     */
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Debug.Log("Left click");
        else if (eventData.button == PointerEventData.InputButton.Middle)
            Debug.Log("Middle click");
        else if (eventData.button == PointerEventData.InputButton.Right)
            Debug.Log("Right click");
    }

    /**
     * Event that fires on a drag event for this UI element
     * Toggles the ship display to move the ships
     */
    public void OnDrag() {
        if (fleet != null && shipDisplay != null) {
            ShipDisplay display = shipDisplay.GetComponent<ShipDisplay>();
            display.toggleMouseForShipMovement();
            display.setChosenFleet(fleet);
        }
    }

    /**
     * Associates a fleet GameObject and the display with the displayed fleet
     */
    public void setParentDisplay(GameObject shipDisplay, GameObject fleet) {
        this.shipDisplay = shipDisplay;
        this.fleet = fleet;
    }
}