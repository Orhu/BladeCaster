using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour {
    [SerializeField] float knockbackStrength = 1.5f;
    void OnCollisionEnter2D(Collision2D col) {
        GameObject colObj = col.collider.gameObject;
        if (colObj.tag == "Player") {
            // kill player
        } else if (colObj.tag == "enemy") {
            colObj.GetComponent<IEnemy>().GetHit(999999, knockbackStrength);
        }
    }
}
