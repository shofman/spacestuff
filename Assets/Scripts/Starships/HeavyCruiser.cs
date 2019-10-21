using UnityEngine;

public class HeavyCruiser : Ship {
    public override int Cost {
      get { return 200; }
    }

    public override int ConstructionDuration {
      get { return 2; }
    }

    protected override string getShipPrefix() {
      return "HC-";
    }
}