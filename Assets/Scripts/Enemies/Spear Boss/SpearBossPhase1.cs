using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearBossPhase1 : MonoBehaviour, IEnemy {
    [SerializeField] SpearBossMain bossHealth;

    [SerializeField] LayerMask platformLayerMask;
    [SerializeField] LayerMask playerLayerMask;

    public int health {get; private set;} = 10;
    public bool invulnerable {get; private set;} = false;

    // ai randomization stuff
    private int jumpCount = 0;
    private float jumpWaitTime = 1f;

    // behavioral stuff
    private int damageToDash = 2;
    private bool active = false;
    private float direction = -1f;
    private bool acting = false;
    private bool noMove = false;
    private bool cutscene = false;
    private bool doJumpNext = false;

    // movement stuff
    private float speed = 1.1f;
    private float dashMod = 2f;
    private float vaultForce = 4f;
    private float leftXExtreme = 50.62f;
    private float rightXExtreme = 53.60f;

    // knockback stuff
    [SerializeField] float atkHitStrength = 1.75f;
    [SerializeField] float dashHitStrength = 3f;

    private Rigidbody2D _body;
    private BoxCollider2D _box;
    private Animator _anim;
    private SFXHandler _voice;

    [SerializeField] GameObject phase2Prefab;

    [SerializeField] float invulnTime = 0.2f;

    void Start() {
        if (Onboarder.SPEAR_BOSS_DEFEATED) {
            Destroy(this.gameObject);
        }
        _body = GetComponent<Rigidbody2D>();
        _box = GetComponent<BoxCollider2D>();
        _anim = GetComponent<Animator>();
        _voice = GetComponent<SFXHandler>();
    }

    void Update() {
        if (active) {
            if (health <= 0) {
                StartCoroutine(Transition());
                return;
            }
            if (!cutscene && !acting) {
                if ((transform.position.x <= leftXExtreme && direction == -1) || (transform.position.x >= rightXExtreme && direction == 1)) {
                    direction = direction * -1;
                }

                if (damageToDash <= 0) {
                    damageToDash = 2;
                    Dash();
                }

                if (Input.GetKeyDown(KeyCode.Space)) {
                    jumpCount += 1;
                }
                if (IsGrounded()) {
                    _anim.SetBool("jump", false);
                    RaycastHit2D attackCast = Physics2D.BoxCast(_box.bounds.center + new Vector3(0.17f * direction, 0f, 0f), new Vector3(0.18f, 0.2f, 0f), 0f, Vector3.right * direction, Mathf.Infinity, playerLayerMask);
                    if (attackCast.collider != null) {
                        Attack();
                    } else if (doJumpNext) {
                        doJumpNext = false;
                        StartCoroutine(Vault());
                    }
                } else {
                    _anim.SetBool("jump", true);
                    RaycastHit2D dashCast = Physics2D.BoxCast(_box.bounds.center + new Vector3(0.4f - _box.bounds.extents.x * direction, _box.bounds.size.y * 1.5f - _box.bounds.extents.y, 0f), new Vector3 (0.8f, _box.bounds.size.y * 3f, 0f), 0f, Vector3.right * direction, Mathf.Infinity, playerLayerMask);
                    if (dashCast.collider != null) {
                        Dash();
                    }
                }
            }

            if (!noMove) {
                _body.velocity = new Vector2(direction * speed, _body.velocity.y); // move forward
            }

            _anim.SetFloat("speed", Mathf.Abs(_body.velocity.x));
            if (_body.velocity.x != 0) {
                transform.localScale = new Vector3(Mathf.Sign(_body.velocity.x), 1f, 1f);
            }     
        }           
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.collider.tag == "Player") {
            other.collider.GetComponent<PlayerMovement>().GetHit(1, atkHitStrength * direction, true, false);
        }
    }

    public void BeginFight() {
        active = true;
        StartCoroutine(SafetyJump());
        StartCoroutine(JumpControl());
    }

    private void Attack() {
        _anim.SetTrigger("attack");
        _voice.PlaySFX("Sounds/SFX/sbp1Atk");
        acting = true;
        noMove = true;
        float direction = transform.localScale.x; // which way are you facing
        Collider2D[] playerHit = Physics2D.OverlapBoxAll(_box.bounds.center + new Vector3((_box.bounds.extents.x * 3) * direction, 0f, 0f), new Vector3(_box.bounds.size.x * 2, _box.bounds.size.y, 0f), 0f, playerLayerMask);
        foreach (Collider2D hit in playerHit) {
            Debug.Log(hit);
            if (hit.tag == "Player") {
                hit.GetComponent<PlayerMovement>().GetHit(1, atkHitStrength * direction, true, false);
            }
        }
        acting = false;
        noMove = false;
    }

    private IEnumerator Vault() {
        // from spear
        yield return new WaitForSeconds(jumpWaitTime);
        _anim.SetTrigger("vault");
        acting = true;
        yield return new WaitForSeconds(_anim.GetCurrentAnimatorStateInfo(0).length);
        if (active) {
            _voice.PlaySFXPitch("Sounds/SFX/vault", 0.5f);
            _body.AddForce(Vector2.up * vaultForce, ForceMode2D.Impulse); // jump vertical
        }
        acting = false;
    }

    private void Dash() {
        // also from spear
        _anim.SetTrigger("dash");
        _voice.PlaySFXPitch("Sounds/SFX/dash", -1f);
        acting = true;
        StartCoroutine(DashHelper());
    }

    private IEnumerator DashHelper() {
        noMove = true;
        _body.gravityScale = 0;
        _body.velocity = new Vector2(_body.velocity.x, 0f);
        StartCoroutine(DashAttacker());
        yield return new WaitForSeconds(0.25f);
        _body.gravityScale = 1;
        noMove = false;
        acting = false;
    }
    private IEnumerator DashAttacker() {
        float t = 0f;
        while (t <= 0.25f) {
            DashAttack();
            _body.velocity = new Vector2(dashMod * direction, _body.velocity.y);
            t += Time.deltaTime;
            yield return null;
        }
    }
    private void DashAttack() {
        float direction = transform.localScale.x; // which way are you facing
        Collider2D[] playerHit = Physics2D.OverlapBoxAll(_box.bounds.center + new Vector3((_box.bounds.extents.x * 3) * direction, 0f, 0f), new Vector3(_box.bounds.size.x * 2, _box.bounds.size.y, 0f), 0f, playerLayerMask);
        foreach (Collider2D hit in playerHit) {
            Debug.Log(hit);
            // check for tags for different results of getting hit (complete later)
            if (hit.tag == "Player") {
                hit.GetComponent<PlayerMovement>().GetHit(2, dashHitStrength * direction, true, false);
            }
        }
    }

    private IEnumerator JumpControl() {
        StartCoroutine(JumpDecay());
        while (active) {
            if (IsGrounded()) {
                if (jumpCount >= Random.Range(3, 10)) {
                        jumpCount = 0;
                        doJumpNext = true;
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator JumpDecay() {
        int lastJump = jumpCount;
        while (active) {
            yield return new WaitForSeconds(2f);
            if (jumpCount == lastJump) {
                jumpCount -= 1;
            }
            lastJump = jumpCount;
        }
    } 

    public bool IsGrounded() {
        float bonusHeight = 0.075f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(_box.bounds.center - new Vector3(0f, _box.bounds.extents.y, 0f), _box.bounds.size - new Vector3(0.02f, _box.bounds.extents.y,0f), 0f, Vector2.down, bonusHeight, platformLayerMask);
        return raycastHit.collider != null;
    }

    public bool IsInvulnerable() {
        return invulnerable;
    }

    public void GetHit(int damage, float strength) {
        if (!active) {
            return;
        }
        // throw out strength
        _voice.PlaySFX("Sounds/SFX/sbp1Hurt");
        health -= damage;
        damageToDash -= damage;
        speed += 0.05f;
        bossHealth.LowerHealth(damage);
        if (health <= 0) {
            StartCoroutine(Transition());
            return;
        }
        StartCoroutine(DoInvuln());
    }

    private IEnumerator DoInvuln() {
        SpriteRenderer _sprite = GetComponent<SpriteRenderer>();
        invulnerable = true;
        gameObject.layer = 10;
        _sprite.color = new Vector4(0.75f, 0.125f, 0.125f, 1f);
        yield return new WaitForSeconds(invulnTime);
        invulnerable = false;
        gameObject.layer = 7;
        _sprite.color = new Vector4(1f, 1f, 1f, 1f);
    }

    private IEnumerator Transition() {
        active = false;
        noMove = true;
        acting = true;
        gameObject.layer = 10; // make invincible
        invulnerable = true;
        _body.velocity = new Vector2(0f, _body.velocity.y);
        

        while (!IsGrounded()) { // wait for him to hit the ground.
            yield return null;
        }
        _body.gravityScale = 0;
        _body.velocity = Vector2.zero;

        _anim.SetTrigger("die");        
    }

    private IEnumerator SafetyJump() {
        yield return new WaitForSeconds(1f);
        if (active) {
            StartCoroutine(Vault());
        }
    }

    public void SpawnPhase2() {
        // need to do some math to get the exact positioning to spawn in the phase 2 prefab at.
        GameObject _phase2 = Instantiate(phase2Prefab) as GameObject;
        _phase2.GetComponent<SpearBossPhase2>().Setup(bossHealth);
        _phase2.transform.position = transform.position; /* + new Vector3(something)*/
        Destroy(this.gameObject);
    }

    public void RaisePosition(float amount) {
        transform.position = transform.position + new Vector3 (0f, amount, 0f);
    }
}
