using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour, ILevelProp {
    [SerializeField] float knockbackStrength = 1.5f;
    [SerializeField] float respawnTime = 2.5f;

    private Animator _anim;
    private Collider2D _box;

    void Start() {
        _anim = GetComponent<Animator>();
        _box = GetComponent<Collider2D>();
    }

    public void Interact() { // pop
        _anim.SetTrigger("pop");
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn() {
        AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length*3f);
        SpriteRenderer _sprite = GetComponent<SpriteRenderer>();
        _box.enabled = false;
        _sprite.enabled = false;
        yield return new WaitForSeconds(respawnTime);
        _box.enabled = true;
        _sprite.enabled = true;
    }

    public void SwitchOperate() {
        // not used by bubble
    }

    void OnCollisionEnter2D(Collision2D col) {
        GameObject other = col.gameObject;
        if (other.tag == "Player") {
            float strength = knockbackStrength;
            if (other.transform.position.x <= transform.position.x) {
                strength *= -1;
            }
            other.GetComponent<PlayerMovement>().GetHit(strength);
        } else if (other.tag == "enemy") {
            float strength = knockbackStrength;
            if (other.transform.position.x <= transform.position.x) {
                strength *= -1;
            }
            other.GetComponent<IEnemy>().GetHit(0, strength);
        }
        _anim.SetTrigger("pop");
        StartCoroutine(Respawn());
    }
}
