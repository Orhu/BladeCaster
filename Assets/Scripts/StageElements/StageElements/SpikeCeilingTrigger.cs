using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCeilingTrigger : MonoBehaviour {
    [SerializeField] SpikeCeiling spikeCeiling;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player" && !spikeCeiling.active) {
            spikeCeiling.Activate();
        }
    }
}
