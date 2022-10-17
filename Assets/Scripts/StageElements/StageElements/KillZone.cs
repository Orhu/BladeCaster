using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PLACEHOLDER SCRIPT PLEASE PLAN TO REPLACE THIS WITH A BETTER, MORE FUNCTIONAL SCRIPT AT A LATER DATE
public class KillZone : MonoBehaviour {
    [SerializeField] bool trueKill = false;
    private BoxCollider2D _box;
    
    [SerializeField] AudioClip fall;
    private AudioSource _voice;

    void Start() {
        _box = GetComponent<BoxCollider2D>();
        _voice = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            _voice.PlayOneShot(fall);
            other.gameObject.GetComponent<PlayerMovement>().KillPlayer(trueKill);
        } else if (other.tag == "enemy") {
            Destroy(other.gameObject);
        }
    }
}
