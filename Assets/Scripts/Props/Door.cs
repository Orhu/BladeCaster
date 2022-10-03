using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, ILevelProp {
    public bool locked = false;
    public bool open = false; 

    private Collider2D _box;
    private Animator _anim;

    // Start is called before the first frame update
    void Start() {
        _box = GetComponent<Collider2D>();
        _anim = GetComponent<Animator>();

        StartCoroutine(RefreshDoor());
    }

    public void Interact() {
        // not used by door
    }

    public void SwitchToggle() {
        Debug.Log("Door switch toggle");
        if (!locked) {
            open = !open;
            StartCoroutine(RefreshDoor());
        }
    }

    public void Unlock() { // Call when the door's key is picked up. Once a door is unlocked, it will never be re-locked.
        locked = false;
    }

    public IEnumerator RefreshDoor() {
        Debug.Log("Door Refresh");
        if (open != _anim.GetBool("open")) {
            _anim.SetBool("open", open);
            if (open) { // wait for door animation to play to open the door to the player
                yield return new WaitForSeconds(_anim.GetCurrentAnimatorStateInfo(0).length); 
            }
        }
        _box.enabled = !open; // player can go through!
    }
}
