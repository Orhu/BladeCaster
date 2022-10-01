using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon {
    [SerializeField] private LayerMask hurtboxLayerMask; //???
    public int damage {get; private set;} = 1;
    public int abilityEnergyCost {get; private set;} = 0;

    private BoxCollider2D _box;

    void Start() {
        _box = GetComponent<BoxCollider2D>();
    }

    public void WeaponUpdate() {
        // TO DO What does this even do
    }

    public void Attack() {
        float direction = transform.localScale.x; // which way are you facing
        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(_box.bounds.center + new Vector3((_box.bounds.extents.x * 3) * direction, 0, 0), new Vector3 (_box.bounds.size.x * 2, _box.bounds.size.y, 0f), 0f, hurtboxLayerMask);
        foreach (Collider2D hit in enemiesHit) {
            Debug.Log(hit);
            // check for tags for different results of getting hit (do later)
            if (hit.tag == "enemy") {
                hit.GetComponent<IEnemy>().GetHit(damage);
            }
        }
    }


    public void Ability() {
        // Not Used
    }
}
