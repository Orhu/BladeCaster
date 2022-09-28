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
}
