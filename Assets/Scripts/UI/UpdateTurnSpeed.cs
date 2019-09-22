using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct TimeDisplay {
  public string display;
  public int timeValue;

  public TimeDisplay(string show, int value) {
    display = show;
    timeValue = value;
  }
}

public class UpdateTurnSpeed : MonoBehaviour
{
    Text txt;
    Hashtable ht = new Hashtable();

    void Awake() {
      txt = gameObject.GetComponent<Text>();
      ht.Add(0, new TimeDisplay("||", 0));
      ht.Add(1, new TimeDisplay(">", 1));
      ht.Add(2, new TimeDisplay(">>", 5));
      ht.Add(3, new TimeDisplay(">>>", 25));
    }

    public void updateSpeed(Slider speedSlider) {
      int updateValue = (int) speedSlider.value;
      if (ht.ContainsKey(updateValue)) {
        TimeDisplay matchedDisplay = (TimeDisplay) ht[updateValue];
        txt.text = matchedDisplay.display;
        Time.timeScale = matchedDisplay.timeValue;
      } else {
        Debug.Log("Could not find value for updating speed. We will remain at previous value");
      }
    }
}
