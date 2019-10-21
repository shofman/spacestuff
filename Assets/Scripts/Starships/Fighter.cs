using UnityEngine;

public class Fighter : Ship {
    public override int Cost {
      get { return 100; }
    }

    public override int ConstructionDuration {
      get { return 1; }
    }

    protected override string getShipPrefix() {
      return "F3-";
    }
}