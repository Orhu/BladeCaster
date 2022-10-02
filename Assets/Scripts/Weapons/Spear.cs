using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour, IWeapon {
    [SerializeField] private LayerMask hurtboxLayerMask;
    public int damage {get; private set;} = 1;
    public int abilityEnergyCost {get; private set;} = 1;

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
            Dash();
        } else {
            if (vaultAvailable) {
                Vault();
            } else {
                StartCoroutine(Charge());
            }
        }
    }

    private IEnumerator Charge() {
        Debug.Log("animator charging set to true");
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
        Debug.Log("animator charging set to false");
        _anim.SetBool("charging", false);
    }
    private IEnumerator ChargeEnergyTick() {
        while(_anim.GetBool("charging")) {
            // tick down energy
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Vault() {
        // tell the animator to play the vault animation
        // add a force to the player's rigidbody going up and to the direction the player is facing at 60 degrees with whatever velocity gives the player a vertical component of 4f.
        // also until the player next touches anything (ground, enemy, prop, etc.) they no longer have control over their horizontal movement.

    }

    private void Dash() {
        // tell the animator to play the dash animation
        // add a horizontal force to the player's rigidbody and removes the player's control over their horizontal movement for 0.5 seconds.
        // reduce player's energy meter
    }
}
