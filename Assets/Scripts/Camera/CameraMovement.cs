using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMovement : MonoBehaviour
{
  public GameObject virtualCamera;
  public GameObject room;

  private GameObject _room;

  private Vector2 playerSpawn;

  private void OnTriggerEnter2D(Collider2D other){
    if(other.CompareTag("Player") && !other.isTrigger){
      _room = Instantiate(room) as GameObject;
      playerSpawn = new Vector2(other.transform.position.x, other.transform.position.y);
      virtualCamera.SetActive(true);
    }
  }

  private void OnTriggerExit2D(Collider2D other){
    if(other.CompareTag("Player") && !other.isTrigger){
      StartCoroutine(DestroyRoom());
      virtualCamera.SetActive(false);
    }
  }

  private IEnumerator DestroyRoom() {
    yield return new WaitForSeconds (0.07f);
    Destroy(_room);
    _room = null;
  }
}
