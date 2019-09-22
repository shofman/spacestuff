using UnityEngine;

public class MoveShipsCommand : ICommand {
  public virtual bool isCompleted { get; set; }

  private GameObject originPlanet;
  private GameObject destinationPlanet;
  private GameObject fleetToMove;
  private GameObject player;

  private int arrivalTime;

  public MoveShipsCommand(GameObject fleet, GameObject originPlanet, GameObject destinationPlanet, GameObject player) {
    // TODO - Don't add entry if the origin and destination planet are the same gameobject

    isCompleted = false;
    this.originPlanet = originPlanet;
    this.destinationPlanet = destinationPlanet;
    this.fleetToMove = fleet;
    this.player = player;

    Planet origin =  originPlanet.GetComponent<Planet>();
    GameObject[] planetsToMoveThrough = origin.getPlanetsToMoveThrough(destinationPlanet);
    this.arrivalTime = fleet.GetComponent<Fleet>().getFleetArrivalTime(planetsToMoveThrough);
    fleetToMove.GetComponent<Fleet>().teleportToPlanet(destinationPlanet);

    // TODO - Make this not be fixed to only be the first entry
    origin.removeFleet(0);
  }

  public void execute() {
    if (EndTurnNotifier.instance().getCurrentTurnCount() >= arrivalTime) {
      fleetToMove.GetComponent<Fleet>().setFinishedTransit();

      // Planet origin =  originPlanet.GetComponent<Planet>();
      // Planet destination = destinationPlanet.GetComponent<Planet>();



      // Debug.Log("We are moving ships from " + origin.getName() + " to " + destination.getName());

      // // TODO - We should pass along the fleet here, but it's throwing a NULL pointer exception
      // origin.moveFleet(destinationPlanet);
      isCompleted = true;
    }
  }

  public void cancel() {
    isCompleted = false;
  }

}