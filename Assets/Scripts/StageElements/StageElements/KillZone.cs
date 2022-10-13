using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PLACEHOLDER SCRIPT PLEASE PLAN TO REPLACE THIS WITH A BETTER, MORE FUNCTIONAL SCRIPT AT A LATER DATE
public class KillZone : MonoBehaviour {
    [SerializeField] Vector3 respawnPosition; 

    private BoxCollider2D _box;

    void Start() {
        _box = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            other.gameObject.GetComponent<PlayerMovement>().KillPlayer();
        } else if (other.tag == "enemy") {
            Destroy(other.gameObject);
        }
    }
}
