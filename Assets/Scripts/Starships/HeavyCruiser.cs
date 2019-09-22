using UnityEngine;

public class HeavyCruiser : Ship {
    new void Start() {

    }

    public override int Cost {
      get { return 20; }
    }
}