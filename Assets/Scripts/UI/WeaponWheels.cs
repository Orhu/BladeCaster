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

  void Update()
  {
    if(Input.GetKey("c")){
      this.gameObject.SetActive(true);
      if(Input.GetKeyDown("LeftArrow")){
        weaponState++;
        if(weaponState < wheelSprites.Length){
          weaponState = 0;
        }
        Refresh();
      }
      else if(Input.GetKeyDown("RightArrow")){
        weaponState--;
        if(weaponState < 0){
          weaponState = wheelSprites.Length;
        }
        Refresh();
      }
      this.gameObject.SetActive(false);
    }
  }
}
