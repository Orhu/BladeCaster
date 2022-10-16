using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

  public AudioSource checkPointDing;

  private void OnTriggerEnter2D(Collider2D other) {
    if (other.tag == "Player") {
        checkPointDing.Play();
        Player _player = other.GetComponent<Player>();
        Vector3 respawn = new Vector3(transform.position.x, transform.position.y, 0f);
        if (_player.respawnPoint != respawn) {
            _player.UpdateRespawnPoint(new Vector3(transform.position.x, transform.position.y, 0f));
            _player.RestoreHealth(12);
        }
    }
  }
}
