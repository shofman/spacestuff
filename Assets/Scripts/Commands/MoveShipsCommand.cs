using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
    Debug.Log("We are moving ships from " + origin.getName() + " to " + destinationPlanet.GetComponent<Planet>().getName());
  }

  private void resolveAnyFleetCombatFromArrivingShips() {
    List<GameObject> fleetsAtPlanet = destinationPlanet.GetComponent<Planet>().getFleetsOverPlanet();
    int numberOfValidFleets = fleetsAtPlanet
      .Where(fleet => !fleet.GetComponent<Fleet>().isInTransit())
      .GroupBy(fleet => fleet.GetComponent<Fleet>().getAllegiance())
      .Distinct()
      .Count();

    if (numberOfValidFleets > 1) {
      FleetCombat combat = new FleetCombat();
      combat.resolveFleetBattleAbovePlanet(fleetsAtPlanet);
    }

  }

  public void execute() {
    if (TurnHandler.instance().getCurrentTurnCount() >= arrivalTime) {
      fleetToMove.GetComponent<Fleet>().setFinishedTransit();
      isCompleted = true;
      resolveAnyFleetCombatFromArrivingShips();
    }
  }

  public void cancel() {
    isCompleted = false;
  }

}