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
    
    private SFXHandler _voice;

    [SerializeField] bool lockAtEnd = false;

    void Start() {
        _voice = GetComponent<SFXHandler>();

        startPoint = transform.position;
    }

    void Update() {
        if (active) {
            trackPercent += direction * speed * Time.deltaTime; // is deltatime needed?
            float x = (endPoint.x - startPoint.x) * trackPercent + startPoint.x;
            float y = (endPoint.y - startPoint.y) * trackPercent + startPoint.y;
            transform.position = new Vector3(x, y, startPoint.z);

            if ((direction == 1 && trackPercent > 1f) || (direction == -1 && trackPercent < 0f)) {
                if (lockAtEnd) {
                    if (_voice != null) {
                        _voice.PlaySFX("Sounds/SFX/elevatorArrive");
                    }
                    active = false;
                    gameObject.tag = "levelProp";
                    Player _player = transform.GetComponentInChildren<Player>();
                    if (_player != null) {
                        _player.gameObject.transform.parent = null;
                    }
                    

                } else {
                    direction *= -1;
                }
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
        if (_voice != null) { // elevator only basically
            _voice.PlaySFX("Sounds/SFX/elevatorActivate");
        }
    }
}
