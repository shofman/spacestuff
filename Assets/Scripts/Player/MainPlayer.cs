using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainPlayer : Player {
    public GameObject resourceDisplay;
    private ResourceDisplay resources;

    void Awake() {
        allegiance = Color.blue;
        resources = resourceDisplay.GetComponent<ResourceDisplay>();
    }

    void Start() {
        wealth = calculateWealth();
        resources.setPlayerDisplay(wealth);
    }

    void Update() {

    }

    public override void spendMoney(int moneyToSpend) {
        base.spendMoney(moneyToSpend);
        resources.setPlayerDisplay(wealth);
    }

    public override int TurnOrder {
        get { return 3; }
    }
}
