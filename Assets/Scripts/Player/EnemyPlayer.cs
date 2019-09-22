using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyPlayer : Player {
    void Awake() {
        allegiance = Color.red;
    }

    public override int TurnOrder {
        get { return 2; }
    }

    public override string PlayerName {
        get { return "AI-MAN"; }
    }
}
