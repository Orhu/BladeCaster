using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWheels : MonoBehaviour
{
  public Sprite[] wheelSprites; // 0 = sword, 1 = spear, 2 = grapple, 3 = claymore

  private SpriteRenderer spriteRenderer;
  private int weaponState = 0;

  void Awake(){
    spriteRenderer = GetComponent<SpriteRenderer>();
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
      spriteRenderer.sprite = this.wheelSprites[weaponState];
    }
    else if(Input.GetKeyDown(KeyCode.RightArrow)){
      weaponState--;
      if(weaponState < 0){
        weaponState = wheelSprites.Length - 1;
      }
      spriteRenderer.sprite = this.wheelSprites[weaponState];
    }
    return weaponState;
  }
}
