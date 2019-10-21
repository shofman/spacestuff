using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class FleetCombat {
    /**
     * Creates a fight between multiple fleets.
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

    public void resolveFleetBattleAbovePlanet(List<GameObject> fleetsAbovePlanet) {
        if (fleetsAbovePlanet == null) {
            return;
        }

        try {
            var winningFleet = fleetsAbovePlanet
                .Where(fleet => !fleet.GetComponent<Fleet>().isInTransit())
                .GroupBy(fleet => fleet.GetComponent<Fleet>().getAllegiance())
                .Select(fleet => new {
                    Strength = fleet.Sum(f => calculateFleetStrength(f.GetComponent<Fleet>())),
                    Allegiance = fleet.Key
                })
                .OrderByDescending(fleetStrengths => fleetStrengths.Strength)
                .First();

            Color winningAllegiance = winningFleet.Allegiance;

            List<GameObject> losingFleets = fleetsAbovePlanet.Where(
                fleet => !fleet.GetComponent<Fleet>().isInTransit() && fleet.GetComponent<Fleet>().getAllegiance() != winningAllegiance
            ).ToList();

            // We iterate this in reverse while destroying 
            // (since we will skip over entries due to jumping indexes if we iterate positively)
            for(int i=losingFleets.Count-1; i>=0; i--) {
                GameObject lostFleet = losingFleets[i];
                string name = lostFleet.GetComponent<Fleet>().getFleetName();
                Debug.Log("fleet destroyed is " + name);
                // TODO - Handle this better than just destroying. Calculate 'damage' and assign, and only destroy if health is gone
                lostFleet.GetComponent<Fleet>().destroyFleet();
            }
        }
        catch (InvalidOperationException) {}
        catch (ArgumentNullException) {}
    }
}