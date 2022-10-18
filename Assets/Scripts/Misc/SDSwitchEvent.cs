using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SDSwitchEvent : MonoBehaviour {
    [SerializeField] Switch _switch;

    private Animator _anim;

    void Start() {
        _anim = GetComponent<Animator>();
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void PlayAnimation() {
        Debug.Log("play anim called switch thing");
        GetComponent<SpriteRenderer>().enabled = true;
        _anim.SetTrigger("go");
    }

    public void FlipSwitch() {
        _switch.Interact();
    }
    
    public void Complete() {
        Destroy(this.gameObject);

    }
}
