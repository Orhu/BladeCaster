using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour, IWeapon {
    [SerializeField] private LayerMask hurtboxLayerMask;
    public int damage {get; private set;} = 1;
    public int abilityEnergyCost {get; private set;} = 1;

    [SerializeField] float vaultForce = 4f;

    private Rigidbody2D _body;
    private BoxCollider2D _box;
    private Animator _anim;

    private bool vaultAvailable = false;

    private IEnumerator activeCR;

    void Start() {
        _body = GetComponent<Rigidbody2D>();
        _box = GetComponent<BoxCollider2D>();
        _anim = GetComponent<Animator>();
    }

    public void WeaponUpdate() {
        // TO DO
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


    public void Ability() {
        if (_anim.GetBool("jump")) {
            activeCR = Dash();
            StartCoroutine(activeCR);
        } else {
            if (vaultAvailable) {
                StopCoroutine(activeCR);
                activeCR = Vault();
                StartCoroutine(activeCR);
            } else {
                activeCR = Charge();
                StartCoroutine(activeCR);
            }
        }
    }

    private IEnumerator Charge() {
        _anim.SetBool("charging", true);
        bool charging = true;
        while(charging) {
            Attack();
            // charging movement happens in the PlayerMovement script.
            yield return null; // wait a frame
            if (!Input.GetKey(KeyCode.X)) {
                charging = false;
            }
        }
        vaultAvailable = true;
        yield return new WaitForSeconds(0.25f);
        vaultAvailable = false;
        _anim.SetBool("charging", false);
    }
    private IEnumerator ChargeEnergyTick() {
        while(_anim.GetBool("charging")) {
            // tick down energy
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator Vault() {
        // tell the animator to play the vault animation
        // add a force to the player's rigidbody going up and to the direction the player is facing at 60 degrees with whatever velocity gives the player a vertical component of 4f.
        // also until the player next touches anything (ground, enemy, prop, etc.) they no longer have control over their horizontal movement.
        vaultAvailable = false;

        yield return new WaitForSeconds(_anim.GetCurrentAnimatorStateInfo(0).length);
        _anim.SetBool("charging", false);

        Vector2 xDirection = Vector2.right;
        if(transform.localScale.x == -1) {
            xDirection = Vector2.left;
        }

        _body.AddForce(Vector2.up * vaultForce, ForceMode2D.Impulse); // jump vertical   
        GetComponent<PlayerMovement>().StunPlayer(0.1f, false, "vault"); // maintains horizontal velocity (need to make that until we interact with something

        // energy stuff
    }

    private IEnumerator Dash() {
        // tell the animator to play the dash animatior
        // reduce player's energy meter
        _anim.SetBool("spearDash", true);
        _body.gravityScale = 0;
        _body.velocity = new Vector2(_body.velocity.x, 0f);
        GetComponent<PlayerMovement>().StunPlayer(0.25f, true, "dash");
        StartCoroutine(DashAttacker());
        yield return new WaitForSeconds(0.25f);
        _body.gravityScale = 1;
        _anim.SetBool("spearDash", false);

        // energy stuff
    }
    private IEnumerator DashAttacker() {
        while (_anim.GetBool("spearDash")) {
            Attack();
            yield return null;
        }
    }
}
