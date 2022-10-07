using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour, IWeapon {
    [SerializeField] private LayerMask hurtboxLayerMask;
    public int damage {get; [SerializeField] set;} = 1;
    public int abilityEnergyCost {get; [SerializeField] set;} = 1;

    [SerializeField] float atkHitStrength = 1.75f;
    [SerializeField] float chargeHitStrength = 2.5f;
    [SerializeField] float dashHitStrength = 3f;

    [SerializeField] float vaultForce = 4f;
    [SerializeField] float dashCooldownTime = 0.4285f; // ~ 140 bpm rhythm to dashes

    private Rigidbody2D _body;
    private BoxCollider2D _box;
    private Animator _anim;

    private bool vaultAvailable = false;
    private bool vaulting = false;
    private bool dashOnCooldown = false;

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
                    hitEnemy.GetHit(damage, atkHitStrength * direction);
                }
            } else if (hit.tag == "levelProp") {
                hit.GetComponent<ILevelProp>().Interact();
            }
        }
    }


    public void Ability() {
        if (_anim.GetBool("jump") && !vaulting) {
            activeCR = Dash();
            StartCoroutine(activeCR);
        } else {
            if (vaultAvailable) {
                if (!_anim.GetBool("jump")) {
                    StopCoroutine(activeCR);
                    activeCR = Vault();
                    StartCoroutine(activeCR);
                }
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
            ChargeAttack();
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
    public void ChargeAttack() {
        float direction = transform.localScale.x; // which way are you facing
        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(_box.bounds.center + new Vector3((_box.bounds.extents.x * 3) * direction, 0f, 0f), new Vector3(_box.bounds.size.x * 2, _box.bounds.size.y, 0f), 0f, hurtboxLayerMask);
        foreach (Collider2D hit in enemiesHit) {
            Debug.Log(hit);
            // check for tags for different results of getting hit (complete later)
            if (hit.tag == "enemy") {
                IEnemy hitEnemy = hit.GetComponent<IEnemy>();
                if (!hitEnemy.IsInvulnerable()) {
                    hitEnemy.GetHit(damage, chargeHitStrength * direction);
                }
            } else if (hit.tag == "levelProp") {
                hit.GetComponent<ILevelProp>().Interact();
            }
        }
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
        Debug.Log("starting vault");
        vaultAvailable = false;
        vaulting = true;

        yield return new WaitForSeconds(_anim.GetCurrentAnimatorStateInfo(0).length);
        _anim.SetBool("charging", false);

        Vector2 xDirection = Vector2.right;
        if(transform.localScale.x == -1) {
            xDirection = Vector2.left;
        }
        if (!_anim.GetBool("jump")) {
            _body.AddForce(Vector2.up * vaultForce, ForceMode2D.Impulse); // jump vertical
            GetComponent<PlayerMovement>().StunPlayer(0.1f, false, "vault"); // maintains horizontal velocity (need to make that until we interact with something
            yield return new WaitForSeconds(0.1f); // adjust to match roughly the length of the start of the jump
        }
        vaulting = false;
        // energy stuff
    }

    private IEnumerator Dash() {
        if (!dashOnCooldown) {
            Debug.Log("starting dash");
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
            StartCoroutine(DashCooldown());

            // energy stuff
        }
    }
    private IEnumerator DashAttacker() {
        while (_anim.GetBool("spearDash")) {
            DashAttack();
            yield return null;
        }
    }
    private void DashAttack() {
        float direction = transform.localScale.x; // which way are you facing
        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(_box.bounds.center + new Vector3((_box.bounds.extents.x * 3) * direction, 0f, 0f), new Vector3(_box.bounds.size.x * 2, _box.bounds.size.y, 0f), 0f, hurtboxLayerMask);
        foreach (Collider2D hit in enemiesHit) {
            Debug.Log(hit);
            // check for tags for different results of getting hit (complete later)
            if (hit.tag == "enemy") {
                IEnemy hitEnemy = hit.GetComponent<IEnemy>();
                if (!hitEnemy.IsInvulnerable()) {
                    hitEnemy.GetHit(damage, dashHitStrength * direction);
                }
            } else if (hit.tag == "levelProp") {
                hit.GetComponent<ILevelProp>().Interact();
            }
        }
    }
    private IEnumerator DashCooldown() { // this feels like a pretty generic coroutine, maybe you should make it global and generalized so you can use it with whatever values you want in any script?
        dashOnCooldown = true;
        yield return new WaitForSeconds(dashCooldownTime);
        dashOnCooldown = false;
    }
}
