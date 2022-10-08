using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWheel : MonoBehaviour {
  private int weaponsUnlocked = 0;
  [SerializeField] Sprite[] wheel0Sprites; // 0 = nothing
  [SerializeField] Sprite[] wheel1Sprites; // 0 = sword
  [SerializeField] Sprite[] wheel2Sprites; // 0 = sword, 1 = spear
  [SerializeField] Sprite[] wheel3Sprites; // 0 = sword, 1 = spear, 2 = grapple
  [SerializeField] Sprite[] wheel4Sprites; // 0 = sword, 1 = spear, 2 = grapple, 3 = claymore

  private Sprite[] wheelSprites;

  private Image _sprite;
  private int weaponState = 0;

  void Awake(){
    _sprite = GetComponent<Image>();
    wheelSprites = wheel0Sprites;
    _sprite.sprite = wheelSprites[weaponState];
  }

  void Start(){
    this.gameObject.SetActive(false);
  }

  public int weaponChange(){
    if(Input.GetKeyDown(KeyCode.LeftArrow)){
      weaponState++;
      if(weaponState > wheelSprites.Length - 1){
        weaponState = 0;
      }
      _sprite.sprite = this.wheelSprites[weaponState];
    }
    else if(Input.GetKeyDown(KeyCode.RightArrow)){
      weaponState--;
      if(weaponState < 0){
        weaponState = wheelSprites.Length - 1;
      }
      _sprite.sprite = this.wheelSprites[weaponState];
    }
    return weaponState;
  }

  public void IncrementWeaponsUnlocked() {
    weaponsUnlocked++;
    switch (weaponsUnlocked) {
      case 0:
        wheelSprites = wheel0Sprites;
        break;
      case 1:
        wheelSprites = wheel1Sprites;
        break;
      case 2:
        wheelSprites = wheel2Sprites;
        break;
      case 3: 
        wheelSprites = wheel3Sprites;
        break;
      case 4:
        wheelSprites = wheel4Sprites;
        break;
      default:
        Debug.LogError($"Invalid number of weapons unlocked: {weaponsUnlocked}");
        break;
    }
    _sprite.sprite = wheelSprites[weaponState];
  }
}
