using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TroopDisplay : Display {
  public GameObject garrisonAmount;
  Text txt;

  protected override void Awake() {
    base.Awake();
    txt = garrisonAmount.GetComponent<Text>();
  }

  public void setGarrisons(int garrisons) {
    txt.text = "Garrisons: " + garrisons;
  }
}