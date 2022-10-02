using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour, ILevelProp {
    [SerializeField] float knockbackForce;

    private BoxCollider2D _box; // trigger zone
    private Animator _anim;

    void Start() {
        _box = GetComponent<BoxCollider2D>();
        _anim = GetComponent<Animator>();
    }

    public void Interact() { // pop
        _anim.SetTrigger("pop");
    }

    public void OnTriggerEnter() { // push the entity away and pop
        // TO DO
    }
}
