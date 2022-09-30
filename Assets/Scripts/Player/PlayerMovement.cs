using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
  
  [SerializeField] private LayerMask platfromLayerMask;
  public float speed;
  public float baseJump;
  public float jumpBoost;

  private bool jumping = false;

  private Rigidbody2D _body;
  private BoxCollider2D _box;

  void Start() {
    _body = GetComponent<Rigidbody2D>();
    _box = GetComponent<BoxCollider2D>();
  }

  void Update() {
    Vector3 max = _box.bounds.max;
    Vector3 min = _box.bounds.min;
    Vector2 corner1 = new Vector2(max.x, min.y - .1f);
    Vector2 corner2 = new Vector2(min.x, min.y - .2f);
    Collider2D hit = Physics2D.OverlapArea(corner1, corner2);

    //Horizontal Movement
    float deltaX = Input.GetAxisRaw("Horizontal") * speed;
    Vector2 movement = new Vector2(deltaX, _body.velocity.y);
    _body.velocity = movement;

    //Vertical movement
    bool grounded = false;
    if (hit != null) {
      grounded = true;
    }

    _body.gravityScale = grounded && deltaX == 0 ? 0 : 1;
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

  }

  private bool IsGrounded() {
    float bonusHeight = 0.1f;
    RaycastHit2D raycastHit = Physics2D.BoxCast(_box.bounds.center, _box.bounds.size, 0f, Vector2.down, bonusHeight, platfromLayerMask);
    bool retVal = raycastHit.collider != null;

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
    Debug.Log(raycastHit.collider);
    return retVal;
  }

  private IEnumerator JumpMod() { // jump higher when space is pressed for longer
    yield return new WaitForSeconds(0.5f);
    jumping = false;
  }
}


