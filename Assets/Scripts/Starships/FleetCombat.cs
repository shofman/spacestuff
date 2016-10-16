using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FleetCombat {
    /**
     * Creates a fight between two fleets.
     * TODO - Handle multiple fleets in a single area
     * TODO - Handle different types of ships
     * TODO - Refine so that ship stat numbers mean something (rather than more ships equals better)
     * TODO - Add concept of retreat?
     */
    public void resolveCombat(GameObject fleetOne, GameObject fleetTwo) {
        Fleet attackFleet = fleetOne.GetComponent<Fleet>();
        Fleet defenseFleet = fleetTwo.GetComponent<Fleet>();

        int attackStrength = calculateFleetStrength(attackFleet);
        int defenseStrength = calculateFleetStrength(defenseFleet);
        
        if (attackStrength > defenseStrength) {
            defenseFleet.destroyFleet();
            attackFleet.landOnPlanet();
        } else {
            attackFleet.destroyFleet();
        }
    }

    /**
     * Calculates a basic value for how strong a fleet is
     * by summing up the attacks
     * TODO - Add shield factor in here
     */
    private int calculateFleetStrength(Fleet fleet) {
        List<GameObject> ships = fleet.getShipsInFleet();
        int strength = 0;
        foreach (GameObject ship in ships) {
            strength += ship.GetComponent<Ship>().getAttack();
        }
        return strength;
    }
}