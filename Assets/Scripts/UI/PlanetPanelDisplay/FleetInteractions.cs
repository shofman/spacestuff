using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
 
public class FleetInteractions : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
    private GameObject fleet;
    private GameObject shipDisplay;

    public UnityEvent onLongClick;

    private bool pointerDown;
    private float pointerDownTimer;
    private float requiredHoldTime = 0.5f;

    private Image fleetBorder;
    private BoxCollider2D collisionDetector;

    private bool inDraggableMode = false;

    private bool isDragging = false;

    private Vector3 dragPosition;
    private Vector3 lockedPosition;

    private Vector3 screenPoint;
    private Vector3 offset;

    private List<Collider2D> collidedElements;

    public void OnPointerDown(PointerEventData eventData) {
      pointerDown = true;

      screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
      dragPosition = new Vector3(eventData.position.x, eventData.position.y, screenPoint.z);
      offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(dragPosition);

      if (eventData.button == PointerEventData.InputButton.Left) {
          if (inDraggableMode) {
            lockedPosition = gameObject.transform.position;
            dragPosition = new Vector3(eventData.position.x, eventData.position.y, transform.position.z);
            isDragging = true;
          }
      }
      else if (eventData.button == PointerEventData.InputButton.Middle) {
        setViewAsStatic();
      }
      else if (eventData.button == PointerEventData.InputButton.Right) {
        setViewAsStatic();
      }

    }

    public void OnDrag(PointerEventData eventData) {
      // Do nothing - interface needed due to bug within Unity
      // https://answers.unity.com/questions/1082179/mouse-drag-element-inside-scrollrect-throws-pointe.html
    }

    private bool canTransferFleet(Fleet currentFleet, Fleet fleetToMergeInto) {
      return fleetToMergeInto.getAllegiance() == currentFleet.getAllegiance() && !fleetToMergeInto.isInTransit();
    }

    public void OnPointerUp(PointerEventData eventData) {
      string fleetName = gameObject.GetComponentsInChildren<Text>()[0].text;
      Fleet currentFleet = this.fleet.GetComponent<Fleet>();

      if (collidedElements.Count > 0 && !currentFleet.isInTransit()) {
        float smallestSize = float.PositiveInfinity;
        Collider2D smallestCollider = null;

        foreach ( Collider2D collider in collidedElements) {
          float distanceBetweenObjects = Vector3.Distance(gameObject.transform.position, collider.gameObject.transform.position);

          if (distanceBetweenObjects < smallestSize) {
            smallestSize = distanceBetweenObjects;
            smallestCollider = collider;
          }
        }

        Fleet fleetToMergeInto = smallestCollider.gameObject.GetComponent<FleetInteractions>().getFleet().GetComponent<Fleet>();

        if (canTransferFleet(currentFleet, fleetToMergeInto)) {
          fleetToMergeInto.transferShipsToFleet(currentFleet.getShipsInFleet());
          currentFleet.destroyFleet();
          Destroy(gameObject);
          this.shipDisplay.GetComponent<ShipDisplay>().toggleDisplayShipsOnPlanet(true);
        }
      }

      if (isDragging) {
        if (lockedPosition != null && transform.position != lockedPosition) {
          transform.position = lockedPosition;
        }
      }
      reset();

    }

    private void Start() {
      collidedElements = new List<Collider2D>();
      fleetBorder = gameObject.GetComponent<Image>();
      IEnumerator setSizeCoroutine  = setGridWidth();
      StartCoroutine(setSizeCoroutine);
    }

    private void updateBorderTransparency(float alpha) {
      Color currentColor = fleetBorder.color;
      currentColor.a = alpha;
      fleetBorder.color = currentColor;
    }

    private void setViewAsDraggable() {
      float visibleBorder = 1f;
      updateBorderTransparency(visibleBorder);
      gameObject.GetComponentInParent<ScrollRect>().enabled = false;
      inDraggableMode = true;
    }

    private void setViewAsStatic() {
      float invisible = 0f;
      updateBorderTransparency(invisible);
      gameObject.GetComponentInParent<ScrollRect>().enabled = true;
      inDraggableMode = false;
    }

    private void Update() {
      if (pointerDown) {
        pointerDownTimer += Time.deltaTime;
        if (pointerDownTimer >= requiredHoldTime) {
          if (onLongClick != null) {
            onLongClick.Invoke();
          }
          inDraggableMode = true;

          if (!isDragging) {
            setViewAsDraggable();
            lockedPosition = gameObject.transform.position;
            isDragging = true;
          }
        }
      }

      if (isDragging) {
        Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        transform.position = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
      }
    }

    private void reset() {
      setViewAsStatic();
      isDragging = false;
      pointerDown = false;
      pointerDownTimer = 0;
    }

    private void OnTriggerEnter2D(Collider2D other) {
      if (!isDragging) return;
      collidedElements.Add(other);
      Text fleetText = gameObject.GetComponentsInChildren<Text>()[0];
    }

    private void OnTriggerExit2D(Collider2D other) {
      if (collidedElements.Contains(other)) {
        collidedElements.Remove(other);
      }
    }

    public IEnumerator setGridWidth() {
        yield return new WaitForEndOfFrame();
        collisionDetector = gameObject.GetComponent<BoxCollider2D>();
        Rect currentSize = gameObject.GetComponent<RectTransform>().rect;
        Vector2 collisionSize = new Vector2(currentSize.width, currentSize.height);
        collisionDetector.size = collisionSize;
    }

    public GameObject getFleet() {
      return this.fleet;
    }

    /**
     * Associates a fleet GameObject and the display with the displayed fleet
     */
    public void setParentDisplay(GameObject shipDisplay, GameObject fleet) {
        this.shipDisplay = shipDisplay;
        this.fleet = fleet;
    }
}