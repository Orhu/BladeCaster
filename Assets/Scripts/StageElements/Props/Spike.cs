using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour {
    [SerializeField] float knockbackStrength = 1.5f;
    void OnCollisionEnter2D(Collision2D col) {
        GameObject colObj = col.collider.gameObject;
        if (colObj.tag == "Player") {
            colObj.GetComponent<PlayerMovement>().GetHit(1, 0f, false, true);
            colObj.GetComponent<PlayerMovement>().KillPlayer(false); // temporary, once health is implemented, switch to doing a ton of damage.
        } else if (colObj.tag == "enemy") {
            colObj.GetComponent<IEnemy>().GetHit(999999, knockbackStrength);
        }
    }
}
