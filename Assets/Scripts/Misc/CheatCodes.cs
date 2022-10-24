using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatCodes : MonoBehaviour {
    private SFXHandler _voice;

    void Start() {
        _voice = GetComponent<SFXHandler>();
    }

    void Update() {
        if (Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.Return)) { // Hold G + O + D, then press Return to get all weapons
            GetComponent<Player>().godMode = !GetComponent<Player>().godMode;
            _voice.PlaySFX("Sounds/SFX/checkpoint");
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.P) && Input.GetKeyDown(KeyCode.Return)) { // Hold W + E + P, then press Return to get all weapons
            GetComponent<Player>().ActivateDebugWeapon();
            _voice.PlaySFX("Sounds/SFX/checkpoint");
        }

        if (Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.Period) && Input.GetKeyDown(KeyCode.Return)) { // Hold G + O + Right Arrow, then press Return to go to the next level
            int cIndex = SceneManager.GetActiveScene().buildIndex;
            if (cIndex == 1) {
                SceneManager.LoadScene(2);
            }
            if (cIndex == 2) {
                SceneManager.LoadScene(1);
            }
        }
    }
}
