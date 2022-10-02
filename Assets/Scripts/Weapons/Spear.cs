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
            StartCoroutine(Dash());
        } else {
            if (vaultAvailable) {
                StartCoroutine(Vault());
            } else {
                StartCoroutine(Charge());
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
        StopCoroutine(Charge());
        _anim.SetTrigger("ability"); // now playing the vault animation.
        vaultAvailable = false;
        

        while(_anim.GetCurrentAnimatorStateInfo(0).IsName("player_spear_vault")) { // keep moving while vaulting.
            yield return null;
        }

        _body.AddForce(Vector2.up * vaultForce, ForceMode2D.Impulse); // jump        
        GetComponent<PlayerMovement>().SetInputLockState(true); // maintains horizontal velocity (need to make that until we interact with something
        _anim.SetBool("charging", false);
    }

    private IEnumerator Dash() {
        // tell the animator to play the dash animation
        // add a horizontal force to the player's rigidbody and removes the player's control over their horizontal movement for 0.5 seconds.
        // reduce player's energy meter
        yield return null;
    }
}
