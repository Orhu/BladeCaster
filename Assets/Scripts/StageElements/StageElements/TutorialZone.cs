using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialZone : MonoBehaviour {
    [SerializeField] int tutorialNumber;
    [SerializeField] GameObject tutorialPopupPrefab;

    private KeyCode tutorialClearButton = KeyCode.KeypadDivide;

    private GameObject _tutorialPopup;

    private IEnumerator activeCR;

    void Start() {
        OnEnable();
    }

    void OnEnable() {
        if (Onboarder.GetTutorialStatus(tutorialNumber)) {
            Destroy(this.gameObject);
        }
        
        if (tutorialClearButton == KeyCode.KeypadDivide) {
            tutorialClearButton = Onboarder.GetTutorialKeyCode(tutorialNumber);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player" && !Onboarder.GetTutorialStatus(tutorialNumber)) {
            activeCR = ShowTutorial(other);
            StartCoroutine(activeCR);
        } else if (Onboarder.GetTutorialStatus(tutorialNumber)) {
            CleanUp();
        }
    }
    
    void OnTriggerExit2D(Collider2D other) {
        if (activeCR != null) {
            StopCoroutine(activeCR);
            activeCR = null;
        }

        CleanUp();
    }

    private IEnumerator ShowTutorial(Collider2D other) {
        if (_tutorialPopup == null) {
            _tutorialPopup = Instantiate(tutorialPopupPrefab) as GameObject;
        }

        _tutorialPopup.transform.parent = other.transform;
        _tutorialPopup.transform.localPosition = new Vector3(-0.004571877f, 0.18f, 0f);

        SpriteRenderer _tutorialSprite = _tutorialPopup.GetComponent<SpriteRenderer>();
        Color noAlpha = new Vector4(1f, 1f, 1f, 0f);
        _tutorialSprite.color = noAlpha;
        
        _tutorialPopup.GetComponent<Animator>().SetInteger("tutorialNum", tutorialNumber);

        yield return new WaitForSeconds(1.5f);

        float t = 0.00f;
        while (t != 1f) {
            Color deltaA = new Vector4(1f, 1f, 1f, Mathf.Lerp(0.0f, 1.0f, t));
            _tutorialSprite.color = deltaA;

            t += 2f * Time.deltaTime;

            if (Input.GetKeyDown(tutorialClearButton)) {
                ClearTutorial();
            }

            yield return null;
        }
        
        while (true) {
            if (Input.GetKeyDown(tutorialClearButton)) {
                ClearTutorial();
            }
        }
    }

    private void ClearTutorial() {
        Onboarder.CompleteTutorial(tutorialNumber);
        if (activeCR != null) {
            StopCoroutine(activeCR);
            activeCR = null;
        }
        CleanUp();
    }

    private void CleanUp() {
        if (_tutorialPopup != null) {
            Destroy(_tutorialPopup);
            _tutorialPopup = null;
        }

        if (Onboarder.GetTutorialStatus(tutorialNumber)) {
            Destroy(this.gameObject);
        }
    }
}
