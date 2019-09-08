using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ChangePlayerDisplay : MonoBehaviour {
  Text txt;

  void Awake() {
    txt = gameObject.GetComponent<Text>();
  }

  public void setAllegianceColor(Color c) {
    txt.color = c;
  }
}
