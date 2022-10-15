using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {
  private void OnTriggerEnter2D(Collider2D other) {
    if (other.tag == "Player") {
        Player _player = other.GetComponent<Player>();
        Vector3 respawn = new Vector3(transform.position.x, transform.position.y, 0f);
        if (_player.respawnPoint != respawn) {
            _player.UpdateRespawnPoint(new Vector3(transform.position.x, transform.position.y, 0f));
            _player.RestoreHealth(12);
        }
    }
  }
}
