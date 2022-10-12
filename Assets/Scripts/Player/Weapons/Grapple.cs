using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour, IWeapon {
    public int damage {get; private set;} = 1; // for weak attack
    public int strongDamage {get; private set;} = 2; // for sweet spot attack

    [Header("Cast Layer Masks")]
    [SerializeField] LayerMask hurtboxLayerMask;
    [SerializeField] LayerMask grappleTargetMask;
    [SerializeField] LayerMask raycastLayerMask;

    [Header("Attack Variables")]
    [SerializeField] float weakAtkKnockback = 1.5f;
    [SerializeField] float strongAtkKnockback = 2.5f;

    [Header("Grapple Variables")]
    [SerializeField] float grappleRange = 1.5f; // roughly the middle third of the screen.
    [SerializeField] float pullSpeed = 3f;
    [SerializeField] float grappleCooldownTime = 0.1f;
    
    [Header("Prefabs")]
    [SerializeField] GameObject hookPrefab;
    [SerializeField] GameObject targetMarkerPrefab;

    private Rigidbody2D _body;
    private BoxCollider2D _box;
    private Animator _anim;

    private IEnumerator activeCR; // needed in case the player is hit while grappling

    private Vector2 lookDirection = Vector2.right; // default to right
    private GameObject markedTarget; // currently targeted game object
    private GameObject _targetMarker = null;
    private GameObject _hook = null;

    private bool canGrapple = true;


    [Header("Debug")]
    [SerializeField] bool showGizmos = true;
    [SerializeField] Color rangeColor = Color.yellow;

    void Start() {
        _body = GetComponent<Rigidbody2D>();
        _box = GetComponent<BoxCollider2D>();
        _anim = GetComponent<Animator>();
    }

    public void WeaponUpdate() {
        if (canGrapple) {
            FindTarget(); // search for and mark target
        }
    }

    private void FindTarget() {
        lookDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); // find the direction the player is looking
        if (lookDirection == Vector2.zero) {
            lookDirection = Vector2.right * transform.localScale.x;
        }

        Collider2D[] targets = Physics2D.OverlapCircleAll(_box.bounds.center, grappleRange, grappleTargetMask);

        GameObject cTarget = null;
        float minAngle = Mathf.Infinity;
        float minDist = Mathf.Infinity;

        foreach (Collider2D target in targets) {
            if (target.gameObject.tag != "grapplePoint" && target.gameObject.tag != "enemy") {
                continue;
            } else if (target.gameObject.tag == "enemy" && target.GetComponent<IGrappleTarget>() == null) {
                continue;
            } else if (target.GetComponent<IGrappleTarget>() != null ) {
                if (!target.GetComponent<IGrappleTarget>().active) {
                    continue;
                }
            }
            Vector3 targetPos = target.transform.position;
            Vector3 direction = targetPos - this.transform.position;

            // make sure target's collider is not obscured
            RaycastHit2D hit = Physics2D.Raycast((Vector3)this.transform.position, direction, Mathf.Infinity, raycastLayerMask);
            if (hit.collider != target) {
                Debug.Log(hit.collider);
                continue; // skip over if target is obscured
            }

            Vector2 targetVector = new Vector2(direction.x, direction.y);

            // Find the angle between target vector and player's look vector            
            float angle = Vector2.Angle(targetVector, lookDirection);
            if (angle <= 90f && angle < minAngle) { // will always be true on the first collision
                cTarget = target.gameObject;
                minAngle = angle;
                minDist = Vector3.Distance(target.transform.position, transform.position);
            } else if (angle == minAngle) { // if target has the same angle as the existing cTarget
                float cTDist = Vector3.Distance(cTarget.transform.position, transform.position);
                float nTDist = Vector3.Distance(target.transform.position, transform.position);
                if (cTDist > nTDist) { // if candidate cTarget distance is less than old cTarget distance, make it the new cTarget
                    cTarget = target.gameObject;
                    minAngle = angle;
                    minDist = nTDist;
                } 
            } else if (angle > 90f) {
                if (minAngle <= 90f) {
                    continue;
                }
                float tDist = Vector3.Distance(target.transform.position, transform.position);
                if (minDist > tDist) {
                    cTarget = target.gameObject;
                    minAngle = angle;
                    minDist = tDist;
                }
            }
        }
       
        // If we found a valid target
        if (cTarget != null) {
            Vector3 dirT = cTarget.transform.position - this.transform.position;
            float signedAngle = Vector2.SignedAngle(new Vector2(dirT.x, dirT.y), Vector2.right);
            // find the direction the target is in relative to the player (the 0-7 direction that is)
            int dir = GetDirection(signedAngle);
            // then place a marker on the target
            SetTarget(cTarget, dir);
        } else {
            SetTarget(null, -1);
        }
    }

    private void SetTarget(GameObject newTarget, int direction) {
        if (newTarget != null) {
            if (newTarget != markedTarget) {
                markedTarget = newTarget;
                
                if (_targetMarker != null) {
                    Destroy(_targetMarker);
                }

                _targetMarker = Instantiate(targetMarkerPrefab) as GameObject;
                _targetMarker.GetComponent<GrappleMarker>().SetUpMarker(markedTarget, direction);
            }
            _targetMarker.GetComponent<GrappleMarker>().UpdateMarker(direction);
        } else {
            markedTarget = null;
            if (_targetMarker != null) {
                    Destroy(_targetMarker);
            }
        }
    }

    private int GetDirection(float angle) {
        Debug.Log(angle);
        switch (angle) {
            case <= -157.5f:
                return 6;
            case > -157.5f and <= -112.5f:
                return 7;
            case > -112.5f and <= -67.5f:
                return 0;
            case > -67.5f and <= -22.5f:
                return 1;
            case > -22.5f and <= 22.5f:
                return 2;
            case > 22.5f and <= 67.5f:
                return 3;
            case > 67.5f and <= 112.5f:
                return 4;
            case > 112.5f and <= 157.5f:
                return 5;
            case > 157.5f:
                return 6;
            default:
                return -1;
        }
    }



    public void Attack() {
        // Unused, replaced by GrappleAttack via Animation Event
    }
    public void GrappleAttack() {
        float direction = transform.localScale.x; // which way are you facing
        Collider2D[] enemiesHitStrong = Physics2D.OverlapBoxAll(_box.bounds.center + new Vector3((_box.bounds.extents.x + 0.39f) * direction, 0.01f, 0f), new Vector3(0.1f, 0.2f, 0f), 0f, hurtboxLayerMask);
        Collider2D[] enemiesHitWeak = Physics2D.OverlapBoxAll(_box.bounds.center + new Vector3((_box.bounds.extents.x + 0.19f) * direction, 0.06f, 0f), new Vector3(0.3f, 0.3f, 0f), 0f, hurtboxLayerMask);

        foreach (Collider2D hit in enemiesHitStrong) {
            Debug.Log($"Strong Hit: {hit}");
            // check for tags for different results of getting hit
            if (hit.tag == "enemy") {
                IEnemy hitEnemy = hit.GetComponent<IEnemy>();
                if (!hitEnemy.IsInvulnerable()) {
                    hitEnemy.GetHit(strongDamage, strongAtkKnockback * direction);
                } else if (hit.tag == "levelProp") {
                    hit.GetComponent<ILevelProp>().Interact();
                }
            }
        }

        foreach (Collider2D hit in enemiesHitWeak) {
            Debug.Log($"Weak Hit: {hit}");
            // check for tags for different results of getting hit
            if (hit.tag == "enemy") {
                IEnemy hitEnemy = hit.GetComponent<IEnemy>();
                if (!hitEnemy.IsInvulnerable()) {
                    hitEnemy.GetHit(damage, weakAtkKnockback * direction);
                } else if (hit.tag == "levelProp") {
                    hit.GetComponent<ILevelProp>().Interact();
                }
            }
        }
    }

    
    public void Ability() {
        if (markedTarget != null) {
            canGrapple = false;
            ShootHook();
        }
    }

    private void ShootHook() {
        if (_hook != null) {
            Destroy(_hook);
        }
        _hook = Instantiate(hookPrefab) as GameObject;
        _hook.GetComponent<GrappleHook>().SetUpHook(transform.position, markedTarget.transform.position, _targetMarker.GetComponent<Animator>().GetInteger("direction"));
        if (activeCR != null) {
            StopCoroutine(activeCR);
        }
        activeCR = DoGrapple();
        StartCoroutine(activeCR);
    }

    private IEnumerator DoGrapple() {
        // silly variables
        float currentVelocity = 1.0f;
        Vector3 start = transform.position;
        Vector3 dest = markedTarget.transform.position;

        GrappleHook _hookComponent = _hook.GetComponent<GrappleHook>();
        GetComponent<PlayerMovement>().StunPlayer(0.01f, false, "grapple");
        _body.gravityScale = 0f;
        _body.velocity = Vector3.zero; // freeze the player
        while(!_hookComponent.hooked) {
            yield return null; // until we're hooked, stay frozen.
        }
        // hooked
        float t = 0.0f;
        float vT = 0.0f;
        
        while(t != 1f) { // while moving to target
            transform.position = new Vector3(Mathf.Lerp(start.x, dest.x, t), Mathf.Lerp(start.y, dest.y, t), 0f);

            t += (currentVelocity * 2f) * Time.deltaTime;
            if (t > 1f) {
                t = 1f;
            }
            currentVelocity = Mathf.Lerp(1.0f, pullSpeed, vT);
            vT += 4f * Time.deltaTime; 
            if (vT > 1f) {
                vT = 1f;
            }
            yield return null;
        }

        // once we reach the target
        Vector3 dir3 = dest - start;
        Vector2 velocityDirection = new Vector2(dir3.x, dir3.y);

        Destroy(_hook);
        SetTarget(null, -1);

        _body.gravityScale = 1f;
        _body.AddForce(Vector3.Normalize(velocityDirection) * currentVelocity, ForceMode2D.Impulse);

        GetComponent<PlayerMovement>().Unstun();
        
        yield return new WaitForSeconds(grappleCooldownTime);
        canGrapple = true;

        Debug.Log("ready to grapple again");
    }

    void OnDrawGizmos() {
        if (showGizmos) {
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, grappleRange);
        }
    }
}