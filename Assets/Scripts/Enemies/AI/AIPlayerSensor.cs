using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerSensor : MonoBehaviour {
    [field: SerializeField]
    public bool PlayerDetected {get; private set;}
    public Vector2 DirectionToTarget => target.transform.position - sensorOrigin.position;

    [Header("OverlapBox Parameters")]
    [SerializeField]
    private Transform sensorOrigin;
    public Vector2 sensorSize = Vector2.one; // tune
    public Vector2 sensorOffset = Vector2.zero;

    public float detectionDelay = 0.2f; // tune

    public LayerMask sensorLayerMask;
    public LayerMask raycastLayerMask; // for making sure there aint walls between the player and the AI

    [Header("Gizmo Parameters")]
    public Color gizmoIdleColor = new Color(0f, 1f, 0f, 0.3f);
    public Color gizmoDetectedColor = new Color(1f, 0f, 0f, 0.3f);
    public bool showGizmos = true;

    private GameObject target;

    public GameObject Target {
        get => target;
        private set {
            target = value;
            PlayerDetected = target != null;
        }
    }

    void Start() {
        StartCoroutine(DetectionUpdate());        
    }

    private IEnumerator DetectionUpdate() {
        yield return new WaitForSeconds(detectionDelay);
        Detect();
        StartCoroutine(DetectionUpdate());
    }

    public void Detect() {
        Collider2D col = Physics2D.OverlapBox((Vector2)sensorOrigin.position + sensorOffset, sensorSize, 0, sensorLayerMask);
        
        if (col != null) {
            Target = col.gameObject;
            // now raycast to see if the player can ACTUALLY be seen (no walls in the way)
            Vector3 destination = Target.transform.position;
            Vector3 direction = destination - sensorOrigin.position;
            RaycastHit2D hit = Physics2D.Raycast((Vector3)sensorOrigin.position, direction, 0.75f, raycastLayerMask);
            if (hit.collider != null) {
                if (hit.collider.tag != "Player") { // if a wall or something is in the way 
                    Target = null;
                }
            }
        } else {
            Target = null;
        }
    }

    private void OnDrawGizmos() {
        if (showGizmos && sensorOrigin != null) {
            Gizmos.color = gizmoIdleColor;
            if (PlayerDetected)
                Gizmos.color = gizmoDetectedColor;
            Gizmos.DrawCube((Vector2)sensorOrigin.position + sensorOffset, sensorSize);
        }
    }
}
