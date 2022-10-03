using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    public bool locked = false;
    public bool open = false; 

    private Rigidbody2D _body;
    private Animator _anim;

    // Start is called before the first frame update
    void Start() {
        _body = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();

        RefreshDoor();
    }

    public void Interact() {
        // not used by door
    }

    public void SwitchToggle() {
        if (!locked) {
            open = !open;
            RefreshDoor();
        }
    }

    public void Unlock() { // Call when the door's key is picked up. Once a door is unlocked, it will never be re-locked.
        locked = false;
    }

    public IEnumerator RefreshDoor() {
        if (open != _anim.GetBool("open")) {
            _anim.SetBool("open", open);
            if (open) { // wait for door animation to play to open the door to the player
                yield return new WaitForSeconds(_anim.GetCurrentAnimatorStateInfo(0).length); 
            }
        }
        _body.isKinematic = !open; // player can go through!
    }
}
