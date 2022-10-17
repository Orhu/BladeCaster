using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalProjectile : MonoBehaviour, IEnemy {
    public int health {get; private set;} = 1;
    public bool invulnerable {get; private set;} = false;

    private float direction = 1f;
    [SerializeField] float speed = 2f;
    [SerializeField] int damage = 1;
    [SerializeField] float strength = 1.5f;

    void Update() {
        transform.position += new Vector3((speed * direction) * Time.deltaTime, 0f, 0f);
        if (transform.position.x <= 50.475f || transform.position.x >= 53.647f) {
            Break();
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.collider.tag == "Player") {
            other.collider.GetComponent<PlayerMovement>().GetHit(damage, strength * direction, true, false);
            Break();
        }
    }

    public void Setup(float dir) {
        direction = dir;
        transform.localScale = new Vector3(direction, 1f, 1f);
    }

    public void GetHit(int damage, float strength) {
        health = 0;
        Break();
    }

    public bool IsInvulnerable() {
        return invulnerable;
    }

    private void Break() {
        GetComponent<SFXHandler>().PlaySFX("Sounds/SFX/crystalShatter");
        Destroy(this.gameObject);
    }
}
