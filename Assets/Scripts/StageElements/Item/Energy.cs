using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : MonoBehaviour
{
  private void OnTriggerEnter2D(Collider2D other){
    if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
      this.gameObject.SetActive(false);
      other.gameObject.GetComponent<Player>().refillEnergy(Player.maxEnergy);
    }
  }
}
