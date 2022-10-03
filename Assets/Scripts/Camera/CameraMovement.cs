using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
  public GameObject virtualCamera;
  public GameObject room;

  private void OnTriggerEnter2D(Collider2D other){
    if(other.CompareTag("Player") && !other.isTrigger){
      room.SetActive(true);
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
