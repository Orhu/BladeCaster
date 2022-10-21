using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For the boss health bar.

public class SpearBossMain : MonoBehaviour {
    public int health {get; private set;} = 20;

    private GameObject player; // for post boss tutorial
    private BossHeartMeter healthMeter;
    [SerializeField] AudioSource _voice;

    [SerializeField] GameObject tutorialPopupPrefab; // yes I know this is weird to do like this but idgaf
    private GameObject _tutorialPopup;
    private bool initialPlayerXScaleSame;

    private bool tutorialTime = false;
    private bool currentTutorialCleared = false;

    void Start() {
        player = GameObject.Find("Player");
        healthMeter = GameObject.Find("Boss Health").GetComponent<BossHeartMeter>();
        _voice = GameObject.Find("Main Camera").GetComponent<AudioSource>();
    }

    void OnDestroy() {
        healthMeter.EndFight();
        _voice.Stop();
        _voice.clip = Resources.Load("Sounds/Music/level1") as AudioClip;
        _voice.Play();
    }

    void Update() {
        if (!tutorialTime) {
            return;
        }

        if (_tutorialPopup != null) {
            if (initialPlayerXScaleSame) {
                if (player.transform.localScale.x == -1f) {
                    _tutorialPopup.transform.localScale = new Vector3(-1f,1f,1f);
                } else {
                    _tutorialPopup.transform.localScale = new Vector3(1f,1f,1f);
                }
            } else {
                if (player.transform.localScale.x == -1f) {
                    _tutorialPopup.transform.localScale = new Vector3(1f,1f,1f);
                } else {
                    _tutorialPopup.transform.localScale = new Vector3(-1f,1f,1f);
                }
            }
        }
    }

    public void Anticipation() {
        _voice.Stop();
    }

    public void StartFight() {
        healthMeter.StartFight();
        healthMeter.Refresh(20);
        _voice.clip = Resources.Load("Sounds/Music/boss") as AudioClip;
        _voice.Play();
    }

    public void LowerHealth(int damage) {
        health -= damage;
        healthMeter.Refresh(health);
    }
    
    public void EndFight(GameObject crystal) { // do tutorials
        healthMeter.EndFight();
        _voice.Stop();
        _voice.clip = Resources.Load("Sounds/Music/level1") as AudioClip;
        _voice.Play();
        StartCoroutine(Tutorial(crystal));
    }

    private IEnumerator Tutorial(GameObject crystal) {
        Debug.Log("starting tutorial");
        Animator _anim = player.GetComponent<Animator>();

        while (crystal != null) {
            yield return null;
        }

        // cycle tutorial
        tutorialTime = true;
        StartCoroutine(ShowTutorial(3, KeyCode.C, player.GetComponent<Collider2D>()));
        while (!currentTutorialCleared) {
            yield return null;
        }

        Debug.Log("finished c tutorial");

        // Cycle tutorial cleared, weapon wheel tutorial
        currentTutorialCleared = false;
        StartCoroutine(ShowTutorial(2, KeyCode.LeftShift, player.GetComponent<Collider2D>()));
        while (!currentTutorialCleared) {
            yield return null;
        }

        Debug.Log("finished shift tutorial");
        yield return new WaitForSeconds(0.2f);

        // weapon wheel tutorial cleared, charge tutorial
        currentTutorialCleared = false;
        StartCoroutine(ShowTutorial(4, KeyCode.X, player.GetComponent<Collider2D>())); // hold x tutorial
        while (!currentTutorialCleared) {
            yield return null;
        }

        Debug.Log("finished charge tutorial");
        yield return new WaitForSeconds(0.2f);

        // charge tutorial cleared
        
        currentTutorialCleared = false;
        if (_tutorialPopup != null) {
            Destroy(_tutorialPopup);
            _tutorialPopup = null;
        }
        _tutorialPopup = Instantiate(tutorialPopupPrefab) as GameObject;
        initialPlayerXScaleSame = player.transform.localScale.x == 1f;

        _tutorialPopup.transform.parent = player.transform;
        _tutorialPopup.transform.localPosition = new Vector3(-0.004571877f, 0.18f, 0f);

        _tutorialPopup.GetComponent<Animator>().SetInteger("tutorialNum", 3); // c tutorial

        SpriteRenderer _tutorialSprite = _tutorialPopup.GetComponent<SpriteRenderer>();
        Color yesAlpha = new Vector4(1f, 1f, 1f, 1f);
        _tutorialSprite.color = yesAlpha;

        while (!currentTutorialCleared) {
            if (_anim.GetInteger("weapon") != 2) {
                _tutorialPopup.GetComponent<Animator>().SetInteger("tutorialNum", 3);
                yield return new WaitForSeconds(0.1f);
            } else if (_anim.GetBool("charging")) {
                _tutorialPopup.GetComponent<Animator>().SetInteger("tutorialNum", 5);
                if (Input.GetKeyDown(KeyCode.X)) {
                    currentTutorialCleared = true;
                }
            } else {
                _tutorialPopup.GetComponent<Animator>().SetInteger("tutorialNum", 4);
                yield return new WaitForSeconds(0.1f);
            }
            yield return null;
        }

        Destroy(_tutorialPopup);
        _tutorialPopup = null;

        Debug.Log("finished vault tutorial");

    }

    private IEnumerator ShowTutorial(int tutorialNumber, KeyCode tutorialClearButton, Collider2D other) {
        Debug.Log("Starting tutorial coroutine");
        if (_tutorialPopup == null) {
            _tutorialPopup = Instantiate(tutorialPopupPrefab) as GameObject;
            initialPlayerXScaleSame = other.transform.localScale.x == 1f;
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

            t += 4f * Time.deltaTime;
            if (t >= 1f) {
                t = 1f;
            }

            if (Input.GetKeyDown(tutorialClearButton)) {
                currentTutorialCleared = true;
            }

            if (currentTutorialCleared) {
                break;
            }

            yield return null;
        }
        
        while (!currentTutorialCleared) {
            if (Input.GetKeyDown(tutorialClearButton)) {
                currentTutorialCleared = true;
            }
            yield return null;
        }

        Destroy(_tutorialPopup);
        _tutorialPopup = null;
    }
}
