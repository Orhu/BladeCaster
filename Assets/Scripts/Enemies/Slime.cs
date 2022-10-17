using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour, IEnemy { // basic AI for the slime enemy (right now it just sits still.
    public int health {get; [SerializeField] set;} = 2;
    public bool invulnerable {get; private set;} = false;

    [Header("Slimy Parameters")]
    [SerializeField] float iFrames = 1f;
    [SerializeField] float knockbackStrength = 1.5f;
    [SerializeField] float bounce = 1.5f;

    private BoxCollider2D _box;
    private Rigidbody2D _body;
    private Animator _anim;
    private AIPlayerSensor _playerSensor;
    private SFXHandler _voice;

    [Header("AI Parameters")]
    //[SerializeField] float followTime = 0.5f;
    [SerializeField] float updateDelay = 0.2f;
    [SerializeField] float thinkTime = 0.5f;
    [SerializeField] float wanderSpeed = 0.3f; // speed of slime in wander action
    [SerializeField] float wanderDelay = 2.125f;
    [SerializeField] float approachSpeed = 1f; // speed of slime in approach action
    [SerializeField] float approachTimeDelay = 2f;
    [SerializeField] int movesToLose = 3; // moves to forget about player
    [SerializeField] float heightDifferenceTolerance = 0.2f; // How far the slime is willing to fall.

    // behavioral variables
    private float lastX; // last x value the player was spotted at or wander destinations.
    private bool passive = true; // true if player not detected as of last slime update
    private bool acting = false; // if the slime is acting, dont do slime update
    private Vector3 homePosition; // set in start

    private GameObject parentObj = null;

    private IEnumerator currentAction;

    private bool justStarted = true;

    void Start() {
        _box = GetComponent<BoxCollider2D>();
        _body = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _playerSensor = GetComponent<AIPlayerSensor>();
        _voice = GetComponent<SFXHandler>();

        _anim.SetInteger("health", 2);

        homePosition = transform.position;
        if (transform.parent != null) {
            parentObj = transform.parent.gameObject;
        }

        StartCoroutine(SlimeUpdate());
    }

    void Update() {
        // animator stuff
        if (!IsGrounded()) {
            _anim.SetBool("inAir", true);
        } else {
            _anim.SetBool("inAir", false);
        }

        // die zone
        if (health <= 0 && IsGrounded()) {
            StartCoroutine(Die());
            health = -1;
        }
    }

    void OnDisable() {
        if (currentAction != null) {
            StopCoroutine(currentAction);
        }
    }

    // SLIME AI ACTION ZONE

    private IEnumerator SlimeUpdate() {
        if (justStarted) {
            justStarted = false;
            yield return new WaitForSeconds(2f);
        }
        yield return new WaitForSeconds(updateDelay);

        // when target detected
        if (_playerSensor.Target != null && passive) {
            passive = false;
            if (currentAction != null) {
                StopCoroutine(currentAction);
            }
            currentAction = Think();
            StartCoroutine(currentAction);
        }

        // when player lost
        if (_playerSensor.Target == null && !passive) {

            passive = true;
            if (currentAction != null) {
                StopCoroutine(currentAction);
            }
            currentAction = ApproachLastSeen();
            StartCoroutine(currentAction);
        }

        if (!acting) {
            if (_playerSensor.Target != null) { // if player has been spotted
                lastX = _playerSensor.Target.transform.position.x;
                float decisionVal = Random.Range(0f, 1f);
                 if (decisionVal <= GenerateAtkThreshold(_playerSensor.Target.transform.position)) {
                    if (currentAction != null) {
                        StopCoroutine(currentAction);
                    }
                    currentAction = Attack();
                    StartCoroutine(currentAction);
                } else {
                    if (currentAction != null) {
                        StopCoroutine(currentAction);
                    }
                    currentAction = Approach();
                    StartCoroutine(currentAction);
                }
            } else {
                passive = true;
                if (currentAction != null) {
                    StopCoroutine(currentAction);
                }
                currentAction = Wander();
                StartCoroutine(currentAction);
            }
        }

        StartCoroutine(SlimeUpdate());
    }


    private IEnumerator Wander() {
        // Generate a random int between 0 and 7.
        // If number == 0, move left after safety check (need to implement)
        // If number == 7, move right after safety check
        //
        // Do this by setting current destination to be whatever the position of the slime is + the move distance.
        // End the coroutine immediately if the player is detected.
        Debug.Log("Wander");
        acting = true;
        int actionVal = Random.Range(0,7);
        bool move = false;
        float dir = 1;
        switch (actionVal) {
            case 0:
            case 1:
                move = true;
                dir = -1;
                break;
            case 7:
            case 6:
                move = true;
                dir = 1;
                break;
            default:
                break;
        }

        if (move) {
            float realMove = 0.031062f * dir;
            if (SafeToMove(realMove, 0)) {
                for (int i = 0; i < 3; i++) {
                    float deltaX = dir * wanderSpeed;
                    _body.velocity = new Vector2(deltaX, _body.velocity.y);
                    ChangeLookDirection();
                    yield return new WaitForSeconds(0.05f);
                }
            }
            yield return new WaitForSeconds(wanderDelay);
        } else {
            yield return new WaitForSeconds(thinkTime);
        }
        currentAction = null;
        acting = false;
    }


    private IEnumerator Approach() {
        Debug.Log("Approach");

        acting = true;

        float dir = Mathf.Sign(lastX - transform.position.x);

        float realMove = 0.2531857f * dir;
        if (SafeToMove(realMove, 1)) {
            float deltaX = dir * approachSpeed;
            _body.velocity = new Vector2(deltaX, _body.velocity.y + bounce);
            _voice.PlaySFX("Sounds/SFX/slimeBounce");
            ChangeLookDirection();
            yield return new WaitForSeconds(approachTimeDelay);
        }

        while (!IsGrounded()) {
            yield return null;
        }

        currentAction = null;
        acting = false;
    }


    private IEnumerator ApproachLastSeen() {
        Debug.Log("Approach Last Seen");

        acting = true;

        float dir = Mathf.Sign(lastX - transform.position.x);

        for (int i = 0; i < movesToLose; i++) {
            float realMove = 0.2531857f * dir;
                if (SafeToMove(realMove, 1)) {
                    float deltaX = dir * approachSpeed;
                    _body.velocity = new Vector2(deltaX, _body.velocity.y + bounce);
                    _voice.PlaySFX("Sounds/SFX/slimeBounce");
                    ChangeLookDirection();
                    yield return new WaitForSeconds(approachTimeDelay);
                } else {
                    break;
                }
            while (!IsGrounded()) {
                yield return null;
            }
        }

        currentAction = null;
        acting = false;
    }


    private IEnumerator Attack() {
        Debug.Log("Attack");

        acting = true;

        yield return new WaitForSeconds(0.75f); // charge up

        float strength = Random.Range(1.5f, 4.0f);
        var dir = _playerSensor.Target.transform.position - transform.position;

        strength *= Mathf.Sign(_playerSensor.Target.transform.position.x - transform.position.x);

        if (Mathf.Abs(_playerSensor.Target.transform.position.y - transform.position.y) >= 0.2f) {
            dir = dir.normalized;
            _body.AddForce(dir * strength, ForceMode2D.Impulse);
        } else { // launch at 45 degree angle
            _body.velocity = Vector2.zero;
            float angle = 45f * Mathf.Deg2Rad;
            Vector2 force = new Vector2(Mathf.Cos(angle) * strength, Mathf.Sin(angle) * Mathf.Abs(strength));
            _body.AddForce(force, ForceMode2D.Impulse);
        }
        _voice.PlaySFX("Sounds/SFX/slimeAttack");
        ChangeLookDirection();

        yield return new WaitForSeconds(1f);

        while (!IsGrounded()) {
            yield return null;
        }
        currentAction = null;
        acting = false;
    }


    private IEnumerator Think() {
        acting = true;
        yield return new WaitForSeconds(thinkTime);
        currentAction = null;
        acting = false;
    }

    //private IEnumerator Retreat() {
        // TO DO
        // Way later, not really important right now
    //}


    private float GenerateAtkThreshold(Vector3 targetPos) {
         float distance = Mathf.Abs(transform.position.x - targetPos.x);

         if (0.5f - distance <= 0f) {
            distance = 0.5f;
         }

         return ((0.5f - distance) * 2);


    }


    private bool SafeToMove(float deltaX, int mode) { // 0 = wander, 1 = approach
        RaycastHit2D hit = Physics2D.Raycast(_box.bounds.center + new Vector3 (deltaX, 0f, 0f), Vector3.down, heightDifferenceTolerance, LayerMask.GetMask("Platform"));
        return (hit.collider != null);
    }






    public bool IsInvulnerable() {
        return invulnerable;
    }

    public void GetHit(int damage, float strength) {
        if (currentAction != null) {
            StopCoroutine(currentAction);
        }
        health -= damage;
        _voice.PlaySFX("Sounds/SFX/slimeHurt");
        _anim.SetInteger("health", health);
        Vector2 knockback = new Vector2(Mathf.Cos(0.6f) * strength, Mathf.Sin(0.6f) * Mathf.Abs(strength));
        _body.AddForce(knockback, ForceMode2D.Impulse);
        ChangeLookDirection();
        _anim.SetTrigger("hit");
        if (health != 0) {
            StartCoroutine(DoIFrames());
        }
    }

    private IEnumerator DoIFrames() {
        acting = true;
        invulnerable = true;
        gameObject.layer = 10;
        yield return new WaitForSeconds(iFrames);
        while (!IsGrounded()) {
            yield return null;
        }
        invulnerable = false;
        gameObject.layer = 7;
        acting = false;
    }

    private IEnumerator Die() {
        if (currentAction != null) {
            StopCoroutine(currentAction);
        }
        Debug.Log("Enemy killed");
        invulnerable = true;
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private bool IsGrounded() {
        float bonusHeight = 0.05f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(_box.bounds.center, _box.bounds.size, 0f, Vector2.down, bonusHeight, LayerMask.GetMask("Platform"));
        bool retVal = raycastHit.collider != null;

        // moving platform bullshit
        if (retVal) {
            if (raycastHit.collider.tag == "movingPlatform") {
                transform.parent = raycastHit.collider.transform;
            } else if (raycastHit.collider.gameObject.layer == 12 && raycastHit.collider.tag != "box") { // if the thing is a character but not a box might be unnecessary
                retVal = false; // overwrite retVal to be false since ya can't stand on that stuff
            }
        } else {
            if (parentObj != null) {
                transform.parent = parentObj.transform;
            }
        }
        return retVal;
    }

    void OnCollisionEnter2D(Collision2D col) {
        GameObject other = col.gameObject;
        if (other.tag == "Player") {
            float strength = knockbackStrength;
            if (other.transform.position.x <= transform.position.x) {
                strength *= -1;
            }
            other.GetComponent<PlayerMovement>().GetHit(1, strength, true, false);
        }
    }

    private void ChangeLookDirection() {
        if (_body.velocity.x != 0) {
            transform.localScale = new Vector3(Mathf.Sign(_body.velocity.x), 1f, 1f);
        }
    }
}
