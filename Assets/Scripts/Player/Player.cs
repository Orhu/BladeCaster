using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  private float curEnergy;
  public static float maxEnergy = 10;

  public IWeapon currentWeapon;

  // 0: Sword, 1: Grapple, 2: Spear, 3: Claymore, 4: Claws, 5: Musket, 6: Shield
  public static bool[] weaponUnlocks = {false, false, false, false, false, false, false};

  //Testing Purposes
  public TextMesh EText;

  void Start()
  {
      curEnergy = maxEnergy;
      EText.text = "Energy: " + curEnergy + "/" + maxEnergy;
  }

  void Update() {
    // press [button] to open weapon wheel theoretically
    if (Input.GetKeyDown("1")) { // sword
      currentWeapon = WeaponsMaster.sword;
    } else if (Input.GetKeyDown("2")) { // grapple
      currentWeapon = WeaponsMaster.grapple;
    } else if (Input.GetKeyDown("3")) { // spear
      currentWeapon = WeaponsMaster.spear;
    } else if (Input.GetKeyDown("4")) { // claymore
      currentWeapon = WeaponsMaster.claymore;
    } else if (Input.GetKeyDown("5")) { // claws
      currentWeapon = WeaponsMaster.claws;
    } else if (Input.GetKeyDown("6")) { // musket
      currentWeapon = WeaponsMaster.musket;
    } else if (Input.GetKeyDown("7")) { // shield
      currentWeapon = WeaponsMaster.shield;
    }
  }

  public void UnlockWeapon(int weaponNum) {
    weaponUnlocks[weaponNum] = true;
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
