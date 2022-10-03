using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour, ILevelProp {
    [SerializeField] float knockbackForce;

    private Animator _anim;

    void Start() {
        _anim = GetComponent<Animator>();
    }

    public void Interact() { // pop
        _anim.SetTrigger("pop");
    }

    public void SwitchToggle() {
        // not used by bubble
    }

    public void OnTriggerEnter() { // push the entity away and pop
        // TO DO
    }
}
