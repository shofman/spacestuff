using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BuildShipsCommand : ICommand {
  public virtual bool isCompleted { get; set; }

  private GameObject originPlanet;
  private GameObject shipToBuild;
  private GameObject player;

  private int arrivalTime;

  public BuildShipsCommand(GameObject newShip, GameObject originPlanet, GameObject player) {
    isCompleted = false;
    this.originPlanet = originPlanet;
    this.shipToBuild = newShip;
    this.player = player;

    Planet origin =  originPlanet.GetComponent<Planet>();
    this.arrivalTime = 0;
    Debug.Log("We are building a ship at " + origin.getName());
  }

  public void execute() {
    if (TurnHandler.instance().getCurrentTurnCount() >= arrivalTime) {
      isCompleted = true;
    }
  }

  public void cancel() {
    isCompleted = false;
  }

}