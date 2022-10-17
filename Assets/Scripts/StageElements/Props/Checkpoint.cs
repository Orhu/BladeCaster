using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {
  [SerializeField] Vector3 respawnPosition;
  private SFXHandler _voice;

  void Start() {
    _voice = GetComponent<SFXHandler>();
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (other.tag == "Player") {
        Player _player = other.GetComponent<Player>();
        if (_player.respawnPoint != respawnPosition) {
            _voice.PlaySFX("Sounds/SFX/checkpoint");
            _player.UpdateRespawnPoint(respawnPosition);
            _player.RestoreHealth(12);
        }
    }
  }
}
