using UnityEngine;

public class Fighter : Ship {
    new void Start() {

    }

    public override int Cost {
      get { return 100; }
    }
}