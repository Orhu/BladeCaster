using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBox : MonoBehaviour, IEnemy {
    [SerializeField] int size; // 0 = small, 1 = big

    public int health {get; private set;}
    public bool invulnerable {get; private set;} = false; // not used
    private int damageThreshold;

    private Rigidbody2D _body;
    private BoxCollider2D _box;
    private SpriteRenderer _sprite;
    private SFXHandler _voice;


    [SerializeField] Sprite[] boxSprites; // 0 = broken, 1 = damaged, 2 = undamaged

    void Awake() {
        _body = GetComponent<Rigidbody2D>();
        _box = GetComponent<BoxCollider2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _voice = GetComponent<SFXHandler>();

        switch (size) {
            case 0:
                health = 1;
                damageThreshold = 1;
                break;
            case 1:
                health = 2;
                damageThreshold = 3;
                break;
        }

        _sprite.sprite = boxSprites[health];
    }

    void OnEnable() {
        _sprite.sprite = boxSprites[health];
    }

    public bool IsInvulnerable() {
        return false;
        // not used
    }

    public void GetHit(int damage, float strength) {
        if (damage >= damageThreshold) {
            health = health - 1;
            Debug.Log(health);
            if (health <= 0) {
                Debug.Log("Break");
                StartCoroutine(Break());
            } else {
                _voice.PlaySFX("Sounds/SFX/boxDamage");
                UpdateSprite();
            }
        } else if (damage >= damageThreshold * 2) {
            StartCoroutine(Break());
        }
    }

    private void UpdateSprite() {
        _sprite.sprite = boxSprites[health];
    }

    private IEnumerator Break() {
        _sprite.sprite = boxSprites[health];
        _box.enabled = false;
        _voice.PlaySFX("Sounds/SFX/boxBreak");
        yield return new WaitForSeconds(0.2f);
        Destroy(this.gameObject);
    }
}
