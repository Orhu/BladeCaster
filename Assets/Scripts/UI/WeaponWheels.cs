using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWheels : MonoBehaviour
{
  public Sprite[] wheelSprites; // 0 = sword, 1 = spear, 2 = grapple, 3 = claymore

  private SpriteRenderer spriteRenderer;
  private int weaponState;

  private void Refresh(){
    spriteRenderer.sprite = this.wheelSprites[weaponState];
  }

  void Awake(){
    spriteRenderer = GetComponent<SpriteRenderer>();
  }

  void Start(){
    this.gameObject.SetActive(false);
  }

  public int weaponChange(int Current){
    weaponState = Current;
    this.gameObject.SetActive(true);
    Refresh();
    while(Input.GetKeyDown(KeyCode.C)){
      if(Input.GetKey(KeyCode.LeftArrow)){
        weaponState++;
        if(weaponState < wheelSprites.Length){
          weaponState = 0;
        }
        Refresh();
      }
      else if(Input.GetKey(KeyCode.RightArrow)){
      weaponState--;
      if(weaponState < 0){
        weaponState = wheelSprites.Length;
      }
      Refresh();
      }
    }
    return weaponState;
  }
}
