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
            Debug.Log("Player killzone");
            StartCoroutine(RespawnPlayer(other));
        } else if (other.tag == "enemy") {
            Destroy(other.gameObject);
        }
    }

    private IEnumerator RespawnPlayer(Collider2D other) { // look! an impressive mess!
        Debug.Log("Player respawn timer on");
        SpriteRenderer _sprite = other.GetComponent<SpriteRenderer>();
        Rigidbody2D _body = other.GetComponent<Rigidbody2D>();
        _sprite.enabled = false;
        _body.gravityScale = 0;
        _body.velocity = new Vector3(0f, 0f, 0f);
        yield return new WaitForSeconds(2f);
        Debug.Log("Player respawn timer finished");
        other.transform.position = respawnPosition;
        _body.gravityScale = 1;
        _sprite.enabled = true;
    }
}
