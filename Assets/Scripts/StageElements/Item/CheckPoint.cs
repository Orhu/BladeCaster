using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
  private Vector2 playerSpawn;

  private void OnTriggerEnter2D(Collider2D other){
    if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
      playerSpawn = new Vector2(other.transform.position.x, other.transform.position.y);
      other.gameObject.GetComponent<Player>().refillEnergy(Player.maxEnergy);
    }
  }
}
