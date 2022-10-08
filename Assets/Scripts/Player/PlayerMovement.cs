using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

  [SerializeField] private LayerMask platformLayerMask;
  public float speed = 1f;
  public float baseJump = 2.5f;
  public float jumpBoost = 0.01f;

  private bool jumping = false;

  private bool stun = false;
  private bool stunTimed = false;
  private string stunMessage = "";
  private IEnumerator stunCR;

  private Rigidbody2D _body;
  private BoxCollider2D _box;
  private Animator _anim;

  // spear horizontal movement variables
  [Header("Spear Movement Modifiers")]
  [SerializeField] float spearChargeMod = 1.25f;
  [SerializeField] float spearDashMod = 2f;
  [SerializeField] float spearCtrlMod = 0.25f;


  void Start() {
    _body = GetComponent<Rigidbody2D>();
    _box = GetComponent<BoxCollider2D>();
    _anim = GetComponent<Animator>();
  }


  void Update() {
    if (IsGrounded() && stun && !stunTimed) {
      StunReset();
    }

    float deltaX;
    if (_anim.GetBool("charging")) { // charging (and therefore we don't want to have player movement inputs)
      Debug.Log("spear charge");
      deltaX = gameObject.transform.localScale.x * speed * spearChargeMod;
    } else {
      if (!stun) {
        deltaX = Input.GetAxisRaw("Horizontal") * speed;
      } else {
        switch (stunMessage) {
          case "vault":
            deltaX = (gameObject.transform.localScale.x * speed * spearChargeMod) + (Input.GetAxisRaw("Horizontal") * spearCtrlMod);
            break;
          case "dash":
            deltaX = gameObject.transform.localScale.x * speed * spearDashMod;
            break;
          default:
            deltaX = _body.velocity.x;
            break;
        }
      }
    }
    if (stunMessage != "hit") {
      _body.velocity = new Vector2(deltaX, _body.velocity.y);
    }

    //Vertical movement
    if (jumping) {
      if (Input.GetKey(KeyCode.Space)) {
      _body.AddForce(Vector2.up * jumpBoost, ForceMode2D.Impulse);
      } else {
        jumping = false;
      }
    } else if (IsGrounded() && Input.GetKeyDown(KeyCode.Space)) {
      _body.AddForce(Vector2.up * baseJump, ForceMode2D.Impulse);
      jumping = true;
      StartCoroutine(JumpMod());
    }

    // animation
    _anim.SetFloat("speed", Mathf.Abs(deltaX));
    if (!Mathf.Approximately(deltaX,0)) { // lets you turn around
      transform.localScale = new Vector3(Mathf.Sign(deltaX), 1, 1);
    }

    _anim.SetBool("jump", !IsGrounded());    
  }


  private bool IsGrounded() {
    float bonusHeight = 0.075f;
    RaycastHit2D raycastHit = Physics2D.BoxCast(_box.bounds.center - new Vector3(0f, _box.bounds.extents.y, 0f), _box.bounds.size - new Vector3(0.02f, _box.bounds.extents.y,0f), 0f, Vector2.down, bonusHeight, platformLayerMask);
    bool retVal = raycastHit.collider != null;
    
    // moving platform bullshit
    if (retVal) {
      if (raycastHit.collider.tag == "movingPlatform") {
        transform.parent = raycastHit.collider.transform; 
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


  private IEnumerator JumpMod() { // jump higher when space is pressed for longer
    yield return new WaitForSeconds(0.2f);
    jumping = false;
  }


  public void GetHit(float strength) {
    if (!_anim.GetBool("spearDash")) {
      _body.velocity = Vector2.zero;
      float angle = 45f * Mathf.Deg2Rad;
      StunPlayer(0.1f, false, "hit");
      Vector2 knockback = new Vector2(Mathf.Cos(angle) * strength, Mathf.Sin(angle) * Mathf.Abs(strength));
      _body.AddForce(knockback, ForceMode2D.Impulse); 
      _anim.SetTrigger("hit");
    // this might work?
    }
  }


  // stun/input-locking stuff
  public void StunPlayer(float time, bool endOnTime, string message) {
    stunMessage = message;
    if (_anim.GetBool("charging")) {
      _anim.SetBool("charging", false);
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
    stun = false;
    stunTimed = false;
    stunMessage = "";
  }
}
