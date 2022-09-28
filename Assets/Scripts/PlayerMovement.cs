using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  public float speed = 75.0f;
  public float jump = 5.0f;

  private Rigidbody2D _body;
  private Animator _anim;
  private BoxCollider2D _box;

  void Start()
  {
    _body = GetComponent<Rigidbody2D>();
    _anim = GetComponent<Animator>();
    _box = GetComponent<BoxCollider2D>();
  }

  void Update()
  {
    Vector3 max = _box.bounds.max;
    Vector3 min = _box.bounds.min;
    Vector2 corner1 = new Vector2(max.x, min.y - .1f);
    Vector2 corner2 = new Vector2(min.x, min.y - .2f);
    Collider2D hit = Physics2D.OverlapArea(corner1, corner2);

    //Horizontal Movement
    float deltaX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
    Vector2 movement = new Vector2(deltaX, _body.velocity.y);
    _body.velocity = movement;

    //Vertical movement
    bool grounded = false;

    if (hit != null)
    {
      grounded = true;
    }

    _body.gravityScale = grounded && deltaX == 0 ? 0 : 1;
    if (grounded && Input.GetKeyDown(KeyCode.Space))
    {
      _body.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
    }
  }
}
