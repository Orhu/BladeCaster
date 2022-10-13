using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claymore : MonoBehaviour, IWeapon {
    public int damage {get; private set;} = 3;
    public int slamDamage {get; private set;} = 5;

    [SerializeField] private LayerMask hurtboxLayerMask;

    [Header("Knockback Values")]
    [SerializeField] float attackKnockback = 3f;
    [SerializeField] float plummetKnockback = 2.5f;
    [SerializeField] float slamKnockback = 4.5f;

    private Rigidbody2D _body;
    private BoxCollider2D _box;
    private Animator _anim;

    void Start() {
        _body = GetComponent<Rigidbody2D>();
        _box = GetComponent<BoxCollider2D>();
        _anim = GetComponent<Animator>();
    }

    public void WeaponUpdate() {
        // Unused
    }

    public void Attack() {
        // Unused, replaced by ClaymoreAttack via Animation Event
    }
    public void ClaymoreAttack() {
        float direction = transform.localScale.x; // which way are you facing
        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(_box.bounds.center + new Vector3((_box.bounds.extents.x * 4) * direction, 0f, 0f), new Vector3(_box.bounds.size.x * 4f, _box.bounds.size.y, 0f), 0f, hurtboxLayerMask);
        foreach (Collider2D hit in enemiesHit) {
            Debug.Log(hit);
            // check for tags for different results of getting hit (complete later)
            if (hit.tag == "enemy" || hit.tag == "box") {
                IEnemy hitEnemy = hit.GetComponent<IEnemy>();
                if (!hitEnemy.IsInvulnerable()) {
                    hitEnemy.GetHit(damage, attackKnockback * direction);
                }
            } else if (hit.tag == "levelProp") {
                hit.GetComponent<ILevelProp>().Interact();
            }
        }
    }

    
    public void Ability() {
        if (_anim.GetBool("jump")) {
            StartCoroutine(Plummet());
        }
    }

    private IEnumerator Plummet() {
        float slamStrengthRatio = 0.00f;
        GetComponent<PlayerMovement>().StunPlayer(0.01f, false, "plummet");
        while (_anim.GetBool("jump")) {
            PlummetAttack();
            slamStrengthRatio += 0.1f;
            if (slamStrengthRatio > 1) {
                slamStrengthRatio = 1f;
            }
            yield return null;
        }
        Slam(slamStrengthRatio);
    }

    private void PlummetAttack() {
        Collider2D[] hits = Physics2D.OverlapBoxAll(_box.bounds.center - new Vector3(0f, _box.bounds.extents.y + 0.1f, 0f), new Vector3(_box.bounds.size.x, 0.04f, 0f), 0f, hurtboxLayerMask);
        foreach (Collider2D hit in hits) {
            Debug.Log(hit);
            // check tags for each hit (complete later)
            if (hit.tag == "enemy") {
                IEnemy hitEnemy = hit.GetComponent<IEnemy>();
                if (!hitEnemy.IsInvulnerable()) {
                    hitEnemy.GetHit(damage, plummetKnockback);
                } else {
                    hitEnemy.GetHit(0, plummetKnockback);
                }        
            } else if (hit.tag == "levelProp") {
                hit.GetComponent<ILevelProp>().Interact();
            }
        }
    }

    private void Slam(float strengthRatio) {
        Collider2D[] hits = Physics2D.OverlapBoxAll(_box.bounds.center, new Vector3(_box.bounds.size.x * 3f, _box.bounds.size.y + 0.12f, 0f), 0f, hurtboxLayerMask);
        foreach (Collider2D hit in hits) {
            Debug.Log(hit);
            if (hit.tag == "enemy") {
                IEnemy hitEnemy = hit.GetComponent<IEnemy>();
                if (!hitEnemy.IsInvulnerable()) {
                    float direction = Mathf.Sign(hit.gameObject.transform.position.x - transform.position.x);
                    hitEnemy.GetHit((int)Mathf.Round(slamDamage * strengthRatio), (slamKnockback * strengthRatio) * direction);
                }
            }
        }
    }

}
