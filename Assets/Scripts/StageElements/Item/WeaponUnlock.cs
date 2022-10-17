using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUnlock : MonoBehaviour
{
  public int weaponType = 0;

  private void OnTriggerEnter2D(Collider2D other){
    if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
      other.GetComponent<Player>().UnlockWeapon(weaponType);
      Destroy(this.gameObject);
    }
  }
}
