using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
  public GameObject virtualCamera;
  public GameObject room;

  private Vector2 playerSpawn;

  private void OnTriggerEnter2D(Collider2D other){
    if(other.CompareTag("Player") && !other.isTrigger){
      room.SetActive(true);
      playerSpawn = new Vector2(other.transform.position.x, other.transform.position.y);
      virtualCamera.SetActive(true);
    }
  }

  private void OnTriggerExit2D(Collider2D other){
    if(other.CompareTag("Player") && !other.isTrigger){
      room.SetActive(false);
      virtualCamera.SetActive(false);
    }
  }
}
