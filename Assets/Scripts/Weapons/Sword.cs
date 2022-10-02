using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon {
    [SerializeField] private LayerMask hurtboxLayerMask; //???
    public int damage {get; private set;} = 1;
    public int abilityEnergyCost {get; private set;} = 0;

    [SerializeField] float pogoForce = 2.5f;

    private Rigidbody2D _body;
    private BoxCollider2D _box;
    private Animator _anim;

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
                    hitEnemy.GetHit(damage, 2.5f * direction);
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
            } else if (hit.tag == "levelProp") {
                hit.GetComponent<ILevelProp>().Interact();
                hitThing = true;
            }
        }

        if (hitThing) { // if you hit something, pogo up
            _body.velocity = new Vector2 (_body.velocity.x, 0f);
            _body.AddForce(Vector2.up * pogoForce, ForceMode2D.Impulse);
        }
    }

    public void Ability() {
        // Not Used by Sword
    }
}
