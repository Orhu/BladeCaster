using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearBossPhase2 : MonoBehaviour, IEnemy {
    public SpearBossMain bossHealth;

    [SerializeField] LayerMask platformLayerMask;
    [SerializeField] LayerMask playerLayerMask;

    public int health {get; private set;} = 10;
    public bool invulnerable {get; private set;} = false;

    [Header("Prefabs")]
    [SerializeField] GameObject shootProjectilePrefab;
    [SerializeField] GameObject spearUnlockPrefab;
    
    [Header("Polygon Collider Paths")]
    [SerializeField] Vector2[] pathA; // used for idle, run, shoot, and slam crash states
    [SerializeField] Vector2[] pathB; // used for attack and jump states
    [SerializeField] Vector2[] pathC; // used for charge state
    [SerializeField] Vector2[] pathD; // used for wall cling state
    [SerializeField] Vector2[] pathE; // used for slam fall state

    // AI Randomization stuff
    [Header("AI Randomization Variables")]
    [SerializeField] float chargeScalar = 0.02f; // for every tenth of a second, add this value to the building charge chance meter
    [SerializeField] float shootScalar = 0.05f; // for every tenth of a second, add this value to the building shoot chance meter
    
    private float chargeChance = 0f;
    private float shootChance = 0f;

    private int jumpCount = 0;
    private float jumpWaitTime = 1f;

    // behavioral stuff
    private float direction = -1f;
    private bool active = false;
    private bool acting = false;
    private bool noMove = false;
    private bool charging = false;
    private bool doJumpNext = false;
    private bool attackOnCD = false;

    private bool doingDmgJump = false;
    private bool doingBigJump = false;
    private int damageToJump = 2;

    // movement stuff
    private float speed = 1.5f;
    private float chargeSpeed = 2f;
    private float jumpForce = 4f;
    private float leftXExtreme = 50.75f;
    private float rightXExtreme = 53.365f;
    private float dmgJumpSpeed = 2f;
    private float bigJumpSpeed = 4f;

    // damage stuff
    [Header("Damage")]
    [SerializeField] int atkDamage = 1;
    [SerializeField] int chargeDamage = 1;

    // knockback stuff
    [Header("Knockback")]
    [SerializeField] float atkHitStrength = 2f;
    [SerializeField] float chargeHitStrength = 3.5f;

    private Rigidbody2D _body;
    private PolygonCollider2D _box;
    private Animator _anim;
    private SFXHandler _voice;

    [SerializeField] float invulnTime = 0.2f;

    public void Setup(SpearBossMain mainhealth) {
        bossHealth = mainhealth;
    }

    void Start() {
        _body = GetComponent<Rigidbody2D>();
        _box = GetComponent<PolygonCollider2D>();
        _anim = GetComponent<Animator>();
        _voice = GetComponent<SFXHandler>();

        RefreshCollider(0);
        active = true;
        StartCoroutine(JumpControl());
        StartCoroutine(ScalarController());
    }


    void Update() {
        if (!active) {
            FinishUpdate();
            return;
        }

        if (noMove) {
            _body.velocity = new Vector2(0f, _body.velocity.y);
            FinishUpdate();
            return;
        }

        if (!IsGrounded()) {
            _anim.SetBool("jump", true);            
            if (charging) {
                _body.velocity = new Vector2(chargeSpeed * direction, _body.velocity.y);
            } else {
                _body.velocity = new Vector2(speed * direction, _body.velocity.y);
            }
            FinishUpdate();
            return;
        } else { // if (IsGrounded()) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                jumpCount += 1;
            }
            _anim.SetBool("jump", false);
            bool turn = false;
            if ((transform.position.x >= rightXExtreme && direction == 1f) || (transform.position.x <= leftXExtreme && direction == -1f)) {
                Debug.Log("Reached end of line");
                if (charging) {
                    StopCharge();
                    FinishUpdate();
                    return;
                }
                if (acting) {
                    FinishUpdate();
                    return;
                }
                direction = direction * -1;
                FinishUpdate(); // need to update the direction for shoot
                turn = true;
            }

            if (charging) { // catch this case before sending away all acting things
                _body.velocity = new Vector2(chargeSpeed * direction, _body.velocity.y);
                FinishUpdate();
                return;
            }
            if (acting) { // dont continue if you're already doing something.
                FinishUpdate();
                return;
            }

            if (damageToJump <= 0) {
                damageToJump = 2;
                StartCoroutine(DmgJumpInvuln());
                _body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // jump vertical
                _voice.PlaySFX("Sounds/SFX/sbJump");
                RefreshCollider(1);
            }

            if (turn) {
                float randomRollCharge = Random.Range(0f,1f);
                float randomRollShoot = Random.Range(0f,1f);
                Debug.Log($"Charge Chance = {chargeChance}, roll = {randomRollCharge}; Shoot Chance = {shootChance}, roll = {randomRollShoot}");
                if (chargeChance >= randomRollCharge) {
                    chargeChance = 0f;
                    Charge();
                    _body.velocity = new Vector2(chargeSpeed * direction, _body.velocity.y);
                    FinishUpdate();
                    return;
                }
                if (shootChance >= randomRollShoot) {
                    shootChance = 0f;
                    Shoot();
                    FinishUpdate();
                    return;
                }
            }

            // weirdness with bounds and polygon collider, figure out tomorrow.
            if (!attackOnCD) {
                RaycastHit2D attackCast = Physics2D.BoxCast(_box.bounds.center + new Vector3(_box.bounds.extents.x * direction, 0f, 0f), new Vector3(0.18f, _box.bounds.size.y, 0f), 0f, Vector3.right * direction, Mathf.Infinity, playerLayerMask);
                if (attackCast.collider != null) {
                    Attack();
                    FinishUpdate();
                    return;
                }
            }

            if (doJumpNext) {
                doJumpNext = false;
                jumpCount = 0;
                _body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // jump vertical
                _voice.PlaySFX("Sounds/SFX/sbJump");
                RefreshCollider(1);
            }
        }
        RefreshCollider(0);
        if (doingDmgJump) {
            _body.velocity = new Vector2 (dmgJumpSpeed * direction, _body.velocity.y);
        } else if (doingBigJump) {
            _body.velocity = new Vector2 (bigJumpSpeed * direction, _body.velocity.y);
        } else {
            _body.velocity = new Vector2(speed * direction, _body.velocity.y);
        }
        FinishUpdate();
    }
    private void FinishUpdate() {
        _anim.SetFloat("speed", Mathf.Abs(_body.velocity.x));
        transform.localScale = new Vector3(Mathf.Sign(direction), 1f, 1f);
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.collider.tag == "Player") {
            if (charging) {
                other.collider.GetComponent<PlayerMovement>().GetHit(chargeDamage, chargeHitStrength * direction, true, false);
            } else {
                other.collider.GetComponent<PlayerMovement>().GetHit(1, atkHitStrength * direction, true, false);
            }
        }
    }

    public bool IsGrounded() {
        float bonusHeight = 0.075f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(_box.bounds.center - new Vector3(0f, _box.bounds.extents.y, 0f), _box.bounds.size - new Vector3(0.02f, _box.bounds.extents.y,0f), 0f, Vector2.down, bonusHeight, platformLayerMask);
        return raycastHit.collider != null;
    }

    // Actions
    private void Attack() {
        acting = true;
        noMove = true;
        _anim.SetTrigger("atk");
        RefreshCollider(1);
    }
    public void DoAttack() { // triggered by animation event
        _voice.PlaySFX("Sounds/SFX/spearBossP2Attack");
        Collider2D[] playerHit = Physics2D.OverlapBoxAll(_box.bounds.center + new Vector3((_box.bounds.extents.x + 0.1f) * direction, 0f, 0f), new Vector3(0.2f, _box.bounds.size.y, 0f), 0f, playerLayerMask);
        foreach (Collider2D hit in playerHit) {
            Debug.Log(hit);
            if (hit.tag == "Player") {
                hit.GetComponent<PlayerMovement>().GetHit(atkDamage, atkHitStrength * direction, true, false);
            }
        }
        noMove = false;
        acting = false;
        StartCoroutine(AtkCD());
        RefreshCollider(0);
    }
    private IEnumerator AtkCD() {
        attackOnCD = true;
        yield return new WaitForSeconds(0.5f);
        attackOnCD = false;
    }

    private void Shoot() {
        RefreshCollider(0);
        // fire a projectile straight
        acting = true;
        noMove = true;
        _anim.SetTrigger("shoot");
    }
    public void FireShot() { // triggered by animation event
        _voice.PlaySFX("Sounds/SFX/sbShoot");
        GameObject _shot = Instantiate(shootProjectilePrefab) as GameObject;
        _shot.transform.position = _box.bounds.center + new Vector3(_box.bounds.extents.x * direction, 0f, 0f);
        _shot.GetComponent<CrystalProjectile>().Setup(direction);
        StartCoroutine(Wait());
    }

    private IEnumerator Wait() {
        yield return new WaitForSeconds(0.5f);
        acting = false;
        noMove = false;
    }

    private void Charge() {
        RefreshCollider(2);
        acting = true;
        charging = true;
        _anim.SetBool("charging", true);
        // damaging handled by collider
    }

    private void StopCharge() {
        RefreshCollider(0);
        charging = false;
        noMove = true;
        _anim.SetBool("charging", false);
    }
    public void Recover() { // triggered by animation event
        noMove = false;
        acting = false;
    }

    public void GetHit(int damage, float strength) {
        // throw out strength
        _voice.PlaySFX("Sounds/SFX/spearBossP2Hit");
        health -= damage;
        damageToJump -= 1;
        bossHealth.LowerHealth(damage);
        if (health <= 0) {
            Die();
            return;
        }
        StartCoroutine(DoInvuln());
    }


    private IEnumerator DoInvuln() {
        SpriteRenderer _sprite = GetComponent<SpriteRenderer>();
        invulnerable = true;
        gameObject.layer = 10;
        _sprite.color = new Vector4(1f, 0.5f, 0.5f, 1f);
        yield return new WaitForSeconds(invulnTime);
        invulnerable = false;
        gameObject.layer = 7;
        _sprite.color = new Vector4(1f, 1f, 1f, 1f);
    }

    private IEnumerator DmgJumpInvuln() {
        invulnerable = true;
        if (health > 5) {
            doingDmgJump = true;
        } else {
            doingBigJump = true;
        }
        yield return new WaitForSeconds(0.25f);
        invulnerable = false;
        doingDmgJump = false;
        doingBigJump = false;
    }


    public bool IsInvulnerable() {
        return invulnerable;
    }


    private void RefreshCollider(int colNum) {
        switch (colNum) {
            case 0:
                _box.SetPath(0, pathA);
                return;
            case 1:
                _box.SetPath(0, pathB);
                return;
            case 2:
                _box.SetPath(0, pathC);
                return;
            case 3:
                _box.SetPath(0, pathD);
                return;
            case 4:
                _box.SetPath(0, pathE);
                return;
            default:
                Debug.LogWarning("Attempting to assign invalid path to SpearBossPhase2's PolygonCollider2D. Assigning default path.");
                _box.SetPath(0, pathA);
                return;
        }
    }

    private IEnumerator JumpControl() {
        StartCoroutine(JumpDecay());
        while (active) {
            if (IsGrounded()) {
                if (jumpCount >= Random.Range(3, 10)) {
                        Debug.Log("jump time");
                        yield return new WaitForSeconds(jumpWaitTime);
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

    private IEnumerator ScalarController() {
        while (active) {
            if (IsGrounded() && !acting) {
                chargeChance += chargeScalar;
                shootChance += shootScalar;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void Die() {
        bossHealth.Anticipation();
        _voice.PlaySFX("Sounds/SFX/finalHit");
        _voice.PlaySFX("Sounds/SFX/spearBossDie");
        active = false;
        acting = true;
        noMove = true;
        transform.localScale = new Vector3(1f, 1f, 1f);
        _body.velocity = Vector2.zero;
        _anim.SetTrigger("die");
        StartCoroutine(LetFall());
    }

    private IEnumerator LetFall() {
        while (!IsGrounded()) {
            yield return null;
        }
        _body.simulated = false;
    }

    public void LowerHeight() {
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.01f, transform.position.z);
    }

    public void SpawnCrystal() {
        GameObject _crystal = Instantiate(spearUnlockPrefab) as GameObject;
        _crystal.transform.position = new Vector3(52.04f, 10.63f, 0f);
        bossHealth.EndFight(_crystal);
        Destroy(this.gameObject);
    }
}
