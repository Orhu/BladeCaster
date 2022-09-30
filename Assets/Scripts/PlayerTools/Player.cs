using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  private float curEnergy;
  public float maxEnergy = 10;

  // 0: Sword, 1: Grapple, 2: Claws, 3: Claymore, 4: Musket, 5: Shield
  public bool[] UnlockWeapon = {true, false, false, false, false, false};

  //Testing Purposes
  public TextMesh EText;

  void Start()
  {
      curEnergy = maxEnergy;
      EText.text = "Energy: " + curEnergy + "/" + maxEnergy;
  }

  public bool UpdateEnergy(float x)
  {
    /*
    Params:
      float x = energy consumption from the weapon being called
    */
    float tempEng = curEnergy; //Pinpoints the energy so it is not updated
    tempEng -= x;
    if(tempEng <= 0) //If the energy is less than zero (empty), then nothing is changed and the execution is negated
    {
      return false;
    }
    curEnergy = tempEng; //Decrement by x if the curEnergy is greater than 0
    EText.text = "Energy: " + curEnergy + "/" + maxEnergy;
    return true;
  }
}
