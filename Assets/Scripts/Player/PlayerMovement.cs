using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

  [SerializeField] private LayerMask platformLayerMask;
  public float speed = 1f;
  public float airCtrlMod = 8f;
  public float airSlowForce = 0.05f;
  public float baseJump = 2.5f;
  public float jumpBoost = 0.01f;

  private bool jumping = false;
  private bool invincible = false;
  [SerializeField] float iFrames = 2f;

  private bool stun = false;
  private bool stunTimed = false;
  private string stunMessage = "";
  private IEnumerator stunCR;
  private float lockDirection = 0;

  private Rigidbody2D _body;
  private BoxCollider2D _box;
  private Animator _anim;
  private Player _player;
  private SFXHandler _voice;

  private Vector3 lastGroundedPosition;
  private bool dead = false;

  // spear horizontal movement variables
  [Header("Spear Movement Modifiers")]
  [SerializeField] float spearChargeMod = 1.25f;
  [SerializeField] float spearDashMod = 2f;
  [SerializeField] float spearCtrlMod = 0.25f; //deprecated for airCtrlMod

  [Header("Claymore Movement Modifiers")]
  [SerializeField] float claymoreSpeed = 0.3f;
  [SerializeField] float claymoreBaseJump = 1.25f;
  [SerializeField] float claymoreJumpBoost = 0.05f;
  [SerializeField] float claymorePlummetSpeed = 3f;

  private int currentWeapon = 0; // 0 = none, 1 = sword, 2 = spear, 3 = grapple, 4 = claymore

  // claymore speed purposes
  private bool movementRefreshed = true; // deprecated soon hopefully

  void Start() {
    _body = GetComponent<Rigidbody2D>();
    _box = GetComponent<BoxCollider2D>();
    _anim = GetComponent<Animator>();
    _player = GetComponent<Player>();
    _voice = GetComponent<SFXHandler>();

    lastGroundedPosition = transform.position;
  }


  void Update() {
    if (IsMostlyGrounded() && !dead) {
      lastGroundedPosition = transform.position;
    }

    if (IsGrounded()) {
      movementRefreshed = true;
      if (stun && !stunTimed && stunMessage != "roll") {
        StunReset();
      }
    }

    float dir = Mathf.Sign(_body.velocity.x);

    // Horizontal Movement

    bool claymoreEquipped = (currentWeapon == 4 && movementRefreshed); // weapon-tied movement will only change once ground is touched after switching
    float horizontalInput = Input.GetAxisRaw("Horizontal");

    float deltaX;
    if (_anim.GetBool("charging")) { // charging (and therefore we don't want to have player movement inputs)
      float chargeDir = Mathf.Sign(_body.velocity.x);
      if (chargeDir != transform.localScale.x) { // if some bitch is bouncing
        chargeDir = transform.localScale.x;
      }
      if (_body.velocity.x == 0) {
        chargeDir = transform.localScale.x;
      }
      deltaX = chargeDir * speed * spearChargeMod;
    } else {
      if (!stun) { // if you're not stunned
        if (!IsGrounded()) {
          ChangeAirSpeed();
          deltaX = _body.velocity.x;
        } else {
          if (claymoreEquipped) {
            deltaX = horizontalInput * claymoreSpeed;
          } else {
            deltaX = horizontalInput * speed;
          }
        }
      } else { // if you are stunned
        switch (stunMessage) {
          case "vault":
            if (IsGrounded()) {
              deltaX = (dir * speed * spearChargeMod) + (horizontalInput * spearCtrlMod); // because of how we're switching the direction you're facing now there are issues...
            } else {
              ChangeAirSpeed();
              deltaX = _body.velocity.x;
            }
            break;
          case "dash":
            deltaX = gameObject.transform.localScale.x * speed * spearDashMod;
            break;
          case "plummet":
            deltaX = 0;
            break;
          default:
            deltaX = _body.velocity.x;
            break;
        }
      }
    }

    if ((stunMessage != "hit" /*|| stunMessage != "vault"*/) && (IsGrounded() || stun)) {
        _body.velocity = new Vector2(deltaX, _body.velocity.y);
    }

    if (_anim.GetBool("rolling")) {
      _body.velocity = new Vector2(speed * lockDirection, _body.velocity.y);
    }

    //Vertical movement
    if (stunMessage == "plummet") {
      _body.AddForce(Vector2.down * claymorePlummetSpeed, ForceMode2D.Impulse);
    }
    else if (jumping) {
      if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) {
        if (claymoreEquipped) {
          _body.velocity += new Vector2(0f, claymoreJumpBoost * Time.deltaTime);
        } else {
          _body.velocity += new Vector2(0f, jumpBoost * Time.deltaTime);
        }
      } else {
        jumping = false;
      }
    } else if (IsGrounded() && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))) {
      if (!_anim.GetBool("charging")) {
        _voice.PlaySFX("Sounds/SFX/playerJump");
        if (claymoreEquipped) {
          _body.AddForce(Vector2.up * claymoreBaseJump, ForceMode2D.Impulse);
        } else {
          _body.AddForce(Vector2.up * baseJump, ForceMode2D.Impulse);
        }
        jumping = true;
        StartCoroutine(JumpMod());
      }
    }


    // animation
    _anim.SetFloat("speed", Mathf.Abs(deltaX));
    if (horizontalInput != 0) { // lets you turn around
      transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);
    }

    _anim.SetBool("jump", !IsGrounded());    
  }


  private void ChangeAirSpeed() {    
    if (stunMessage == "hit" || stunMessage == "respawn") {
      return;
    }
    if (Input.GetAxisRaw("Horizontal") == 0) {
      Debug.Log("correct thing");
      float xVelIn = Mathf.Sign(_body.velocity.x);
      _body.AddForce((Vector2.left * Mathf.Sign(_body.velocity.x)) * airSlowForce, ForceMode2D.Impulse); // might be acceleration, unsure
      if (xVelIn != Mathf.Sign(_body.velocity.x)) {
        _body.velocity = new Vector2(0f, _body.velocity.y); // if x velocity change goes through 0, set x velocity to 0
      }
    } else {
      _body.AddForce((Vector2.right * Input.GetAxisRaw("Horizontal")) * (airCtrlMod * Time.deltaTime), ForceMode2D.Impulse);
      
      if (stunMessage == "vault") {
        if (Mathf.Abs(_body.velocity.x) > ((speed * spearChargeMod) + spearCtrlMod)) {
          _body.velocity = new Vector2(((speed * spearChargeMod) + spearCtrlMod) * Mathf.Sign(_body.velocity.x), _body.velocity.y);
        }
        if (Mathf.Sign(_body.velocity.x) != lockDirection) {
          _body.velocity = new Vector2(0f, _body.velocity.y);
        }
      } else if (currentWeapon != 4) {
        if (Mathf.Abs(_body.velocity.x) > speed) {
          _body.velocity = new Vector2(speed * Mathf.Sign(_body.velocity.x), _body.velocity.y);
        }
      } else {
        if (Mathf.Abs(_body.velocity.x) > claymoreSpeed) {
          _body.velocity = new Vector2(claymoreSpeed * Mathf.Sign(_body.velocity.x), _body.velocity.y);
        }
      }
    }
  }


  public bool IsGrounded() {
    float bonusHeight = 0.075f;
    RaycastHit2D raycastHit = Physics2D.BoxCast(_box.bounds.center - new Vector3(0f, _box.bounds.extents.y, 0f), _box.bounds.size - new Vector3(0.02f, _box.bounds.extents.y,0f), 0f, Vector2.down, bonusHeight, platformLayerMask);
    bool retVal = raycastHit.collider != null;
    
    // moving platform bullshit
    if (retVal) {
      if (raycastHit.collider.tag == "movingPlatform") {
        transform.parent = raycastHit.collider.transform; 
      } else if (raycastHit.collider.gameObject.layer == 12 && raycastHit.collider.tag != "box") { // if the thing is a character but not a box might be unnecessary
        retVal = false; // overwrite retVal to be false since ya can't stand on that stuff
      }
    } else {
        transform.parent = null;
    }


    // showing our raycast in the game

    Color rayColor;
    if (retVal) {
      rayColor = Color.green;
    } else {
      rayColor = Color.red;
    }

    Debug.DrawRay(_box.bounds.center + new Vector3(_box.bounds.extents.x,0), Vector2.down * (_box.bounds.extents.y + bonusHeight), rayColor);
    Debug.DrawRay(_box.bounds.center - new Vector3(_box.bounds.extents.x,0), Vector2.down * (_box.bounds.extents.y + bonusHeight), rayColor);
    Debug.DrawRay(_box.bounds.center - new Vector3(_box.bounds.extents.x, _box.bounds.extents.y + bonusHeight), Vector2.right * (_box.bounds.extents.x * 2), rayColor);
    //Debug.Log(raycastHit.collider);
    return retVal;
  }


  private bool IsMostlyGrounded() {
    if (stunMessage == "roll") {
      return false;
    }
    float bonusHeight = 0.075f;
    RaycastHit2D raycastHit = Physics2D.Raycast(_box.bounds.center - new Vector3(0f, _box.bounds.extents.y, 0f), Vector2.down, bonusHeight, platformLayerMask);
    bool retVal = raycastHit.collider != null;
    
    // moving platform bullshit
    if (retVal) {
      if (raycastHit.collider.tag == "movingPlatform" || raycastHit.collider.tag == "box" || raycastHit.collider.gameObject.layer == 7) {
        return false;
      }
    }

    return retVal;
  }


  private IEnumerator JumpMod() { // jump higher when space is pressed for longer
    yield return new WaitForSeconds(0.2f);
    jumping = false;
  }


  public void GetHit(int damage, float strength, bool giveInvuln, bool overrideInvincible) {
    if (!invincible || overrideInvincible) {
      if (damage >= 0) {
        _voice.PlaySFX("Sounds/SFX/playerHurt");
        _player.DealDamage(damage);
      }
      if (damage == -1) { // secret instakill damage val
        _player.KillPlayer();
      }
    }

    // knockback
    if (!_anim.GetBool("spearDash") && (overrideInvincible || !invincible) && strength != 0) {
      _body.velocity = Vector2.zero;
      float angle = 45f * Mathf.Deg2Rad;
      StunPlayer(0.1f, false, "hit");
      Vector2 knockback = new Vector2(Mathf.Cos(angle) * strength, Mathf.Sin(angle) * Mathf.Abs(strength));
      _body.AddForce(knockback, ForceMode2D.Impulse); 
      _anim.SetTrigger("hit");
    // this might work?
    }
    if (giveInvuln) {
      StartCoroutine(Invuln());
    }
  }


  // stun/input-locking stuff

  /* legend of stun messages:
  *  hit: used for when the player is stunned as a result of being hit by an enemy, attack, or prop
  *  respawn: used for when the player is respawning as a result of falling off a cliff or touching spikes
  *  plummet: used for when the player is under the influence of the claymore's slam ability
  *  roll: used for when the player is under the influence of the sword's roll ability
  *  vault: used for when the player is under the influence of the velocity changes made by the spear's vault ability
  *  dash: used for when the player is under the influence of the velocity changes made by the spear's dash ability */
  public void StunPlayer(float time, bool endOnTime, string message) {
    stunMessage = message;
    if (_anim.GetBool("charging")) {
      _anim.SetBool("charging", false);
    }

    switch (message) {
      case "plummet":
        invincible = true;
        break;
      case "dash":
        RefreshMovement();
        break;
      case "vault":
      case "roll":
        lockDirection = transform.localScale.x;
        break;
    }

    if (stunCR != null) {
      StopCoroutine(stunCR);
    }
    if (time == 0) {
      stun = true;
    } else {
      stunCR = StunTime(time, endOnTime);
      StartCoroutine(stunCR);
    }
  }

  public bool StunQuery(int mode) { // 0 = checking for if you can do weapon attacks and stuff
    if (!stun) {
      return false;
    }
    switch (mode) {
      case 0:
        switch (stunMessage) {
          case "hit":
          case "respawn":
          case "plummet":
          case "dash":
          case "roll":
            return true;
          default:
            return false;
        }
      default:
        Debug.LogWarning("Invalid stun query mode selected");
        return false;
    }
  }

  public void Unstun() {
    StunReset();
  }

  private IEnumerator StunTime(float time, bool endOnTime) {
    stunTimed = true;
    stun = true;
    yield return new WaitForSeconds(time);
    stunTimed = false;
    if (endOnTime) {
      StunReset();
    }
  }

  private void StunReset() {
    if (stunCR != null) {
      StopCoroutine(stunCR);
    }
    if (stunMessage != "hit") {
      invincible = false;
      gameObject.layer = 6;
    }
    if (_anim.GetBool("rolling")) {
      _anim.SetBool("rolling", false);
    }
    stun = false;
    stunTimed = false;
    stunMessage = "";
    lockDirection = 0;
  }

  private IEnumerator Invuln() {
    Debug.Log("Invincible");
    invincible = true;
    gameObject.layer = 14;
    yield return new WaitForSeconds(iFrames);
    Debug.Log("Uninvincible");
    invincible = false;
    gameObject.layer = 6;
  }

  public void SwitchWeapon(int weaponNum) {
    currentWeapon = weaponNum;
    movementRefreshed = false;
  }

  public void RefreshMovement() {
    movementRefreshed = true;
  }

  public void makeInvuln() {
    Debug.Log("Invincible");
    invincible = true;
    gameObject.layer = 14;
  }
  public void removeInvuln() {
    Debug.Log("Uninvincible");
    invincible = false;
    gameObject.layer = 6;
  }

  public void KillPlayer(bool trueKill, int damage = 0) {
    dead = true;
    if (trueKill) {
      _player.KillPlayer();
    } else {
      if (damage != 0) {
        _player.DealDamage(damage);
        if (_player.curHealth <= 0) {
          return;
        }
      }
      StartCoroutine(RespawnPlayer());
    }
  }

  private IEnumerator RespawnPlayer() {
    SpriteRenderer _sprite = GetComponent<SpriteRenderer>();
    StunPlayer(0.01f, false, "respawn");
    _sprite.enabled = false;
    _body.gravityScale = 0;
    _body.velocity = new Vector3(0f, 0f, 0f);
    yield return new WaitForSeconds(2f);
    transform.position = lastGroundedPosition;
    _body.gravityScale = 1;
    _sprite.enabled = true;
    dead = false;
    
    StartCoroutine(Invuln());
  }
}