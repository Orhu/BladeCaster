using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatCodes : MonoBehaviour {
    private SFXHandler _voice;

    void Start() {
        _voice = GetComponent<SFXHandler>();
    }

    void Update() {
        if (Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.Return)) {
            GetComponent<Player>().godMode = !GetComponent<Player>().godMode;
            _voice.PlaySFX("Sounds/SFX/checkpoint");
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.U) && Input.GetKey(KeyCode.P) && Input.GetKeyDown(KeyCode.Return)) {
            GetComponent<Player>().ActivateDebugWeapon();
            _voice.PlaySFX("Sounds/SFX/checkpoint");
        }
        
    }
}
