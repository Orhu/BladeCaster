using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
  private void OnTriggerEnter2D(Collider2D other){
    if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
      //TO DO LATER; Have Player Spawn Change Here/Fill Energy to Max
    }
  }
}
