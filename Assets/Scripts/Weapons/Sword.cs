using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon {
    [SerializeField] private LayerMask hurtboxLayerMask;
    public int damage {get; private set;}
    public int abilityEnergyCost {get; private set;} = 0;

    [SerializeField] float atkHurtboxXMod = 1.2f;
    [SerializeField] float atkHurtboxYMod = 0.8f;

    private Rigidbody2D _body;
    private BoxCollider2D _box;
    

    void Start() {
        _body = GetComponent<Rigidbody2D>();
        _box = GetComponent<BoxCollider2D>();
    }

    public void WeaponUpdate() {
        // TO DO
    }

    public void Attack() {
        Vector2 hurtboxDirection = Vector2.left;
        if (Mathf.Sign(transform.localScale.x) >= 0) {
            hurtboxDirection = Vector2.right;
        }
        float hurtboxDistance = _box.bounds.size.y * atkHurtboxXMod; 
        RaycastHit2D hurtboxHit = Physics2D.BoxCast(_box.bounds.center, _box.bounds.size + new Vector3(0f, _box.bounds.size.y * atkHurtboxYMod - _box.bounds.size.y,0f), 0f, hurtboxDirection, hurtboxDistance, hurtboxLayerMask);
        Debug.Log(hurtboxHit.collider);
    }


    public void Ability() {
        // Not Used
    }
}
