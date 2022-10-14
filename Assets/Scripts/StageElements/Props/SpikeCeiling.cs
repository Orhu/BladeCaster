using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCeiling : MonoBehaviour, ILevelProp {
    [SerializeField] float timeToFall = 30f;

    [SerializeField] Vector3 destination;
    private Vector3 start;

    private bool active;

    void Start() {
        start = transform.position;
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.collider.tag == "Player") {
            other.collider.gameObject.GetComponent<PlayerMovement>().KillPlayer(); // swap for the proper kill player thing when thats ready
            active = false;
        } if (other.collider.tag == "enemy") {
            Destroy(other.collider.gameObject);
        }
    }

    public void Interact() {
        // nothing
    }

    public void SwitchOperate() {
        active = false;
    }

    public void Activate() {
        active = true;
        StartCoroutine(Fall());
    }

    private IEnumerator Fall() {
        float t = 0f;
        while (active) {
            if (t/timeToFall > 1f){
                t = timeToFall;
            }
            transform.position = new Vector3(start.x, Mathf.Lerp(start.y, destination.y, t/timeToFall));
            t += Time.deltaTime;
        
            yield return null;
        }

        Vector3 endpt = transform.position;

        t = 0f;
        while (t != 1f) { 
            if (t> 1f) {
                t = 1f;
            }
            transform.position = new Vector3(start.x, Mathf.Lerp(endpt.y, start.y, t)); // adjust so that it wont be longer than respawn time.
            t += Time.deltaTime;

            yield return null;
        }
    }
}
