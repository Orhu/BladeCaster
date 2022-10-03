using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  private float curEnergy;
  public static float maxEnergy = 10;
  public bool debugWeapon = false;

  public IWeapon currentWeapon;

  // 0: Sword, 1: Grapple, 2: Spear, 3: Claymore, 4: Claws, 5: Musket, 6: Shield
  public static bool[] weaponUnlocks = {false, false, false, false, false, false, false};

  //Testing Purposes
  public TextMesh EText;

  private Animator _anim;

  void Start() {
    _anim = GetComponent<Animator>();
    curEnergy = maxEnergy;
    EText.text = "Energy: " + curEnergy + "/" + maxEnergy;

    if(debugWeapon){
      for (int i = 0; i < 4; i++) {
        UnlockWeapon(i);
      }
    }
  }

  void Update() {
    // Weapon Swap
    // press [button] to open weapon wheel theoretically
    if (Input.GetKeyDown("1")) { // sword
      SwitchWeapon(0);
    } else if (Input.GetKeyDown("2")) { // grapple
      SwitchWeapon(1);
    } else if (Input.GetKeyDown("3")) { // spear
      SwitchWeapon(2);
    } else if (Input.GetKeyDown("4")) { // claymore
      SwitchWeapon(3);
    } /*else if (Input.GetKeyDown("5")) { // claws
      SwitchWeapon(4);
    } else if (Input.GetKeyDown("6")) { // musket
      SwitchWeapon(5);
    } else if (Input.GetKeyDown("7")) { // shield
      SwitchWeapon(6);
    }*/

    if (currentWeapon != null) {
      // Attacking/Ability
      if (Input.GetKeyDown(KeyCode.X)) {
        _anim.SetTrigger("ability");
        currentWeapon.Ability();
      } else if (Input.GetKeyDown(KeyCode.Z)) {
        _anim.SetTrigger("attack");
        currentWeapon.Attack();
      } else {
        currentWeapon.WeaponUpdate();
      }
    }
  }

  private void SwitchWeapon(int weaponNum) {
    _anim.SetInteger("weapon", (weaponNum + 1));
    switch (weaponNum) {
      case 0:
        currentWeapon = WeaponsMaster.sword;
        break;
      case 1:
        currentWeapon = WeaponsMaster.grapple;
        break;
      case 2:
        currentWeapon = WeaponsMaster.spear;
        break;
      case 3:
        currentWeapon = WeaponsMaster.claymore;
        break;
      /*
      case 4:
        currentWeapon = WeaponsMaster.claws;
        break;
      case 5:
        currentWeapon = WeaponsMaster.musket;
        break;
      case 6:
        currentWeapon = WeaponsMaster.shield;
        break;*/
    }
  }

  public void UnlockWeapon(int weaponNum) {
    weaponUnlocks[weaponNum] = true;
  }

  public bool UpdateEnergy(float x) {
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
