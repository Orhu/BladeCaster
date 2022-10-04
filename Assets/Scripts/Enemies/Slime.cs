using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour, IEnemy { // basic AI for the slime enemy (right now it just sits still.)
    public int health {get; [SerializeField] set;} = 2;
    public bool invulnerable {get; private set;} = false;

    [SerializeField] float iFrames = 0.2f;
    [SerializeField] float knockbackStrength = 1.5f;

    private BoxCollider2D _box;
    private Rigidbody2D _body;
    private Animator _anim;

    void Start() {
        _box = GetComponent<BoxCollider2D>();
        _body = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    void Update() {
        if (health == 0 && IsGrounded()) {
            StartCoroutine(Die());
            health = -1;
        }

    }

    public bool IsInvulnerable() {
        return invulnerable;
    }

    public void Attack() {
        // TO DO
    }

    public void GetHit(int damage, float strength) {
        health -= damage;
        Vector2 knockback = new Vector2(Mathf.Cos(0.6f) * strength, Mathf.Sin(0.6f) * Mathf.Abs(strength));
        _body.AddForce(knockback, ForceMode2D.Impulse);
        if (health != 0) {
            StartCoroutine(DoIFrames());
        }
    }

    private IEnumerator DoIFrames() {
        invulnerable = true;
        yield return new WaitForSeconds(iFrames);
        invulnerable = false;
    }

    private IEnumerator Die() {
        Debug.Log("Enemy killed");
        invulnerable = true;
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private bool IsGrounded() {
        float bonusHeight = 0.1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(_box.bounds.center, _box.bounds.size, 0f, Vector2.down, bonusHeight, LayerMask.GetMask("Platform"));
        return raycastHit.collider != null;
    }

    void OnCollisionEnter2D(Collision2D col) {
        GameObject other = col.gameObject;
        if (other.tag == "Player") {
            float strength = knockbackStrength;
            if (other.transform.position.x <= transform.position.x) {
                strength *= -1;
            }
            other.GetComponent<PlayerMovement>().GetHit(strength);
        }
    }
}
