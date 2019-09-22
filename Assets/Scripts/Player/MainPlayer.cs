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

    protected override void Start() {
        base.Start();
        resources.setPlayerDisplay(base.wealth);
    }

    void Update() {

    }

    public override void spendMoney(int moneyToSpend) {
        base.spendMoney(moneyToSpend);
        resources.setPlayerDisplay(wealth);
    }

    public override int TurnOrder {
        get { return 1; }
    }

    public override string PlayerName {
        get { return "MainPlayer"; }
    }
}
