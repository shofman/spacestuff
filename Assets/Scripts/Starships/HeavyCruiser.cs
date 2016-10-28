using UnityEngine;

public class HeavyCruiser : Ship {
    new void Update() {
        base.Update();
        transform.RotateAround(gameObject.transform.position, new Vector3(70,0,0), 10 * Time.deltaTime);
    }
}