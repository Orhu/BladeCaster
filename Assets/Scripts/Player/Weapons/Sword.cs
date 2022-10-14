using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon {
    [SerializeField] private LayerMask hurtboxLayerMask;
    [SerializeField] private LayerMask rollCheckLayerMask;
    public int damage {get; [SerializeField] set;} = 1;

    [SerializeField] float hitStrength = 2.5f;
    [SerializeField] float pogoForce = 2.5f;

    private Rigidbody2D _body;
    private BoxCollider2D _box;
    private Animator _anim;

    private bool readyToExit = false;
    private bool allowExit = false;

    void Start() {
        _body = GetComponent<Rigidbody2D>();
        _box = GetComponent<BoxCollider2D>();
        _anim = GetComponent<Animator>();
    }

    public void WeaponUpdate() {
        if (_anim.GetBool("jump")) {
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
                if (!_anim.GetBool("swordPogo")) {
                    _anim.SetBool("swordPogo", true); // turn on pogo mode when down is being held
                }
                AttackPogo();
            } else if (_anim.GetBool("swordPogo")) {
                _anim.SetBool("swordPogo", false); // turn off if not being pressed but is on
            }
        } else if (_anim.GetBool("swordPogo")) { // turn off if no longer jumping
            _anim.SetBool("swordPogo", false); 
        }
    }

    public void Attack() {
        float direction = transform.localScale.x; // which way are you facing
        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(_box.bounds.center + new Vector3((_box.bounds.extents.x * 3) * direction, 0f, 0f), new Vector3(_box.bounds.size.x * 2, _box.bounds.size.y, 0f), 0f, hurtboxLayerMask);
        foreach (Collider2D hit in enemiesHit) {
            Debug.Log(hit);
            // check for tags for different results of getting hit (complete later)
            if (hit.tag == "enemy") {
                IEnemy hitEnemy = hit.GetComponent<IEnemy>();
                if (!hitEnemy.IsInvulnerable()) {
                    hitEnemy.GetHit(damage, hitStrength * direction);
                }
            } else if (hit.tag == "levelProp") {
                hit.GetComponent<ILevelProp>().Interact();
            }
        }
    }

    private void AttackPogo() {
        bool hitThing = false;
        Collider2D[] hits = Physics2D.OverlapBoxAll(_box.bounds.center - new Vector3(0f, _box.bounds.extents.y + 0.03f, 0f), new Vector3(_box.bounds.size.x, 0.04f, 0f), 0f, hurtboxLayerMask);
        foreach (Collider2D hit in hits) {
            Debug.Log(hit);
            // check tags for each hit (complete later)
            if (hit.tag == "enemy") {
                IEnemy hitEnemy = hit.GetComponent<IEnemy>();
                if (!hitEnemy.IsInvulnerable()) {
                    hitEnemy.GetHit(damage, 0f);
                    hitThing = true;
                }
            } else if (hit.tag == "levelProp" && hit.name != "Switch") {
                hit.GetComponent<ILevelProp>().Interact();
                hitThing = true;
            }
        }

        if (hitThing) { // if you hit something, pogo up
            PlayerMovement _playerMove = GetComponent<PlayerMovement>();
            _playerMove.Unstun(); // pogo unstuns
            _playerMove.RefreshMovement(); // and refreshes movement
            _body.velocity = new Vector2 (_body.velocity.x, 0f);
            _body.AddForce(Vector2.up * pogoForce, ForceMode2D.Impulse);
        }
    }

    public void Ability() { // Roll
        StartCoroutine(Roll());
    }

    private IEnumerator Roll() {
        PlayerMovement _playerMove = GetComponent<PlayerMovement>();
        _anim.SetBool("rolling", true);

        Vector2 normalBoxSize = _box.bounds.size; // save the normal size before changing anything
        Vector2 normalBoxOffset = _box.offset; // save the normal offset before changing anything

        // shrink the player's box collider
        _box.size = new Vector2(0.12f,0.12f);
        _box.offset = new Vector2(0f, -0.02f);

        _playerMove.makeInvuln(); // make the player invincible
        _playerMove.StunPlayer(0.01f, false, "roll");
        allowExit = true;

        while (!readyToExit) { // wait for one cycle of animation to complete
            yield return null;
        }

        bool ableToExit = false;
        while (!ableToExit) {
            RaycastHit2D hit = Physics2D.BoxCast(_box.bounds.center + new Vector3(-0.004571877f, 0.06f, 0f), new Vector3(normalBoxSize.x - 0.01f, normalBoxSize.y - 0.06f, 0f), 0f, Vector2.up, 0f, rollCheckLayerMask);
            Debug.Log(hit.collider);
            ableToExit = hit.collider == null; // if hit.collider is null, you can stand up
            if (hit.collider != null) {
                yield return null;
            }
        }

        _playerMove.Unstun();
        _playerMove.removeInvuln();
        _box.size = normalBoxSize;
        _box.offset = normalBoxOffset;
        allowExit = false;
        readyToExit = false;
        Debug.Log("Exit Roll");
    }
    public void AllowRollExit() {
        if (allowExit) {
            readyToExit = true;
        }
    }
}
