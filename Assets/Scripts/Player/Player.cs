using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  private float curEnergy;
  public static float maxEnergy = 10;
  public bool debugWeapon = false;

  public IWeapon currentWeapon;
  public WeaponWheel wheel;

  private float fixedDeltaTime;
  private PlayerMovement _movement;

  // 0: Sword, 1: Grapple, 2: Spear, 3: Claymore, 4: Claws, 5: Musket, 6: Shield
  public static bool[] weaponUnlocks = {false, false, false, false, false, false, false};

  private Animator _anim;

  void Awake(){
    this.fixedDeltaTime = Time.fixedDeltaTime;
  }

  void Start() {
    _anim = GetComponent<Animator>();
    _movement = GetComponent<PlayerMovement>();

    if(debugWeapon){
      for (int i = 0; i < 4; i++) {
        UnlockWeapon(i);
        Debug.Log("Weapon " + i + " Unlocked");
      }
    }
  }

  void Update() {
    // Weapon Swap
    // press [button] to open weapon wheel theoretically
    if(Input.GetKey(KeyCode.C)){
      wheel.gameObject.SetActive(true);
      SwitchWeapon(wheel.weaponChange());
      Time.timeScale = 0.1f;
    }
    else if(Input.GetKeyUp(KeyCode.C)){
      wheel.gameObject.SetActive(false);
      Time.timeScale = 1.0f;
    }
    Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;

    if (Input.GetKeyDown("1")) { // sword
      Debug.Log("Switching to Sword");
      SwitchWeapon(0);
    } else if (Input.GetKeyDown("2")) { // spear
      Debug.Log("Switching to Spear");
      SwitchWeapon(1);
    } else if (Input.GetKeyDown("3")) { // grapple
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
    if (_anim.GetBool("swordPogo") && weaponNum!= 0) {
      _anim.SetBool("swordPogo", false);
    }
    switch (weaponNum) {
      case 0:
        if(weaponUnlocks[weaponNum] == false) break;
        currentWeapon = WeaponsMaster.sword;
        Debug.Log("Sword equipped successfully");
        break;
      case 1:
        if(weaponUnlocks[weaponNum] == false) break;
        currentWeapon = WeaponsMaster.spear;
        Debug.Log("Spear equipped successfully");
        break;
      case 2:
        if(weaponUnlocks[weaponNum] == false) break;
        currentWeapon = WeaponsMaster.grapple;
        break;
      case 3:
        if(weaponUnlocks[weaponNum] == false) break;
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
    if(weaponUnlocks[weaponNum] == true){
      _anim.SetInteger("weapon", (weaponNum + 1));
      GetComponent<PlayerMovement>().SwitchWeapon(weaponNum + 1);
    }
  }

  public void UnlockWeapon(int weaponNum) {
    weaponUnlocks[weaponNum] = true;
    wheel.IncrementWeaponsUnlocked();
  }

  public void refillEnergy(float amount){
    curEnergy += amount;
    if(curEnergy > maxEnergy)
    {
      curEnergy = maxEnergy;
    }
    return;
  }

  /*public bool UpdateEnergy(float x) {

    Params:
      float x = energy consumption from the weapon being called

    float tempEng = curEnergy; //Pinpoints the energy so it is not updated
    tempEng -= x;
    if(tempEng <= 0) //If the energy is less than zero (empty), then nothing is changed and the execution is negated
    {
      return false;
    }
    curEnergy = tempEng; //Decrement by x if the curEnergy is greater than 0
    EText.text = "Energy: " + curEnergy + "/" + maxEnergy;
    return true;
  }*/
}
