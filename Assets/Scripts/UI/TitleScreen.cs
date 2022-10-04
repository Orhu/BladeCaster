using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {
    [SerializeField] Image titleScreen;
    [SerializeField] Image controlsScreen;

    void Start() {
        titleScreen.gameObject.SetActive(true);
        controlsScreen.gameObject.SetActive(false);
    }

    public void OnPlay() {
        SceneManager.LoadScene(1);
    }

    public void OnControls() {
        titleScreen.gameObject.SetActive(false);
        controlsScreen.gameObject.SetActive(true);
    }

    public void OnBack() {
        titleScreen.gameObject.SetActive(true);
        controlsScreen.gameObject.SetActive(false);
    }
}
