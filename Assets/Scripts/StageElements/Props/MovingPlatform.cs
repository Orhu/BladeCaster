using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour, ILevelProp {
    public Vector3 endPoint = Vector3.zero;
    private Vector3 startPoint;
    public float speed = 0.5f;

    private float trackPercent = 0;
    private int direction = 1;

    public bool active = false;

    void Start() {
        startPoint = transform.position;
    }

    void Update() {
        if (active) {
            trackPercent += direction * speed * Time.deltaTime; // is deltatime needed?
            float x = (endPoint.x - startPoint.x) * trackPercent + startPoint.x;
            float y = (endPoint.y - startPoint.y) * trackPercent + startPoint.y;
            transform.position = new Vector3(x, y, startPoint.z);

            if ((direction == 1 && trackPercent > .9f) || (direction == -1 && trackPercent < .1f)) {
                direction *= -1;
            }
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, endPoint);
    }

    public void Interact() {
        // not used by moving platform
    }

    public void SwitchOperate() {
        active = !active;
    }
}
