using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
  private static int maxHealth;
  public int curHealth;
  public bool debugWeapon = false;
  public bool godMode = false;

  public IWeapon currentWeapon;
  public int weaponNumber = 0;
  private int weaponsUnlocked = 0;
  public WeaponWheel wheel;

  private float fixedDeltaTime;

  // 0: Sword, 1: Grapple, 2: Spear, 3: Claymore, 4: Claws, 5: Musket, 6: Shield
  public static bool[] weaponUnlocks = {false, false, false, false, false, false, false};

  private Rigidbody2D _body;
  private Animator _anim;
  private PlayerMovement _movement;
  private SFXHandler _voice;


  private int pendingWeapon;

  // respawning stuff
  public Vector3 respawnPoint {get; private set;}
  [SerializeField] Image screenBlackout;

  [SerializeField] HeartMeter heartMeter;

  void Awake(){
    this.fixedDeltaTime = Time.fixedDeltaTime;
    screenBlackout.color = new Vector4 (0f,0f,0f,0f);
    screenBlackout.gameObject.SetActive(false);
  }


  void Start() {
    screenBlackout.gameObject.SetActive(false);
    _body = GetComponent<Rigidbody2D>();
    _anim = GetComponent<Animator>();
    _movement = GetComponent<PlayerMovement>();
    _voice = GetComponent<SFXHandler>();

    if (weaponsUnlocked > 1) {
      maxHealth = 6 + (2 * (weaponsUnlocked - 1));
    } else {
      maxHealth = 6;
    }

    curHealth = maxHealth;
    respawnPoint = transform.position;

    heartMeter.Refresh(curHealth, maxHealth);

    if(debugWeapon){
      ActivateDebugWeapon();
    }
  }


  void Update() {
    bool isStunned = _movement.StunQuery(0);
    // Weapon Swap
    // press [button] to open weapon wheel theoretically
    if (Input.GetKeyDown(KeyCode.LeftShift)) {
      StartCoroutine(DoWeaponWheel());
    } else if (Input.GetKey(KeyCode.LeftShift)){
      pendingWeapon = wheel.WeaponChange();
    }
    Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;

    if (Input.GetKeyDown(KeyCode.C)) {
      if (weaponsUnlocked > 0) {
        SwitchWeapon(weaponNumber % weaponsUnlocked); // this formula should work
      }
    }

    if (Input.GetKeyDown("1")) { // sword
      Debug.Log("Switching to Sword");
      SwitchWeapon(0);
    } else if (Input.GetKeyDown("2")) { // spear
      Debug.Log("Switching to Spear");
      SwitchWeapon(1);
    } else if (Input.GetKeyDown("3")) { // grapple
      SwitchWeapon(2);
    } else if (Input.GetKeyDown("4")) { // claymore
      SwitchWeapon(3);
    } /*else if (Input.GetKeyDown("5")) { // claws
      SwitchWeapon(4);
    } else if (Input.GetKeyDown("6")) { // musket
      SwitchWeapon(5);
    } else if (Input.GetKeyDown("7")) { // shield
      SwitchWeapon(6);
    }*/

    if (currentWeapon != null && !isStunned) {
      // Attacking/Ability
      if (Input.GetKeyDown(KeyCode.X)) {
        _anim.SetTrigger("ability");
        currentWeapon.Ability();
      } else if (Input.GetKeyDown(KeyCode.Z)) {
        _anim.SetTrigger("attack");
        currentWeapon.Attack();
      } else {
        currentWeapon.WeaponUpdate();
      }
    }
  }


  private void SwitchWeapon(int weaponNum) {
    if (weaponNum != 2 && weaponNumber == 3) {
      GetComponent<Grapple>().CleanUp();
    }
    if (_anim.GetBool("swordPogo") && weaponNum!= 0) {
      _anim.SetBool("swordPogo", false);
    }
    switch (weaponNum) {
      case 0:
        if(weaponUnlocks[weaponNum] == false) break;
        currentWeapon = WeaponsMaster.sword;
        weaponNumber = 1;
        Debug.Log("Sword equipped successfully");
        break;
      case 1:
        if(weaponUnlocks[weaponNum] == false) break;
        currentWeapon = WeaponsMaster.spear;
        weaponNumber = 2;
        Debug.Log("Spear equipped successfully");
        break;
      case 2:
        if(weaponUnlocks[weaponNum] == false) break;
        currentWeapon = WeaponsMaster.grapple;
        weaponNumber = 3;
        break;
      case 3:
        if(weaponUnlocks[weaponNum] == false) break;
        currentWeapon = WeaponsMaster.claymore;
        weaponNumber = 4;
        break;
      /*
      case 4:
        currentWeapon = WeaponsMaster.claws;
        break;
      case 5:
        currentWeapon = WeaponsMaster.musket;
        break;
      case 6:
        currentWeapon = WeaponsMaster.shield;
        break;*/
    }
    if(weaponUnlocks[weaponNum]){
      _voice.PlaySFX("Sounds/SFX/weaponSwap");
      _anim.SetInteger("weapon", (weaponNum + 1));
      GetComponent<PlayerMovement>().SwitchWeapon(weaponNum + 1);
    }
  }


  public void UnlockWeapon(int weaponNum) {
    _voice.PlaySFX("Sounds/SFX/weaponGet");
    weaponsUnlocked++;
    if (weaponNum != 0) {
      IncreaseMaxHealth();
    }
    if (weaponsUnlocked > 4) {
      weaponsUnlocked = 4;
    }
    weaponUnlocks[weaponNum] = true;
    wheel.IncrementWeaponsUnlocked();
    if (weaponNum == 0) { // else keep that shit hidden :O
      SwitchWeapon(weaponNum);
    }
  }


  private IEnumerator DoWeaponWheel() {
    wheel.gameObject.SetActive(true); // show wheel
    Image _wheelImage = wheel.GetComponent<Image>();
    float t = 0.0f;
    while (Input.GetKey(KeyCode.LeftShift)) {
      if (t != 1.0f) {
        Color deltaA = new Vector4(1f, 1f, 1f, Mathf.Lerp(0.0f, 1.0f, t));
        _wheelImage.color = deltaA;
        Time.timeScale = Mathf.Lerp(1.0f, 0.05f, t);

        t += 10f * Time.deltaTime;
      }
      yield return null; // skip frame
    }

    // when shift key released
    SwitchWeapon(pendingWeapon);
    t = Mathf.Abs(t - 1f);
    while (Time.timeScale != 1f) {
      Color deltaA = new Vector4(1f, 1f, 1f, Mathf.Lerp(1.0f, 0.0f, t));
      _wheelImage.color = deltaA;
      Time.timeScale = Mathf.Lerp(0.05f, 1.0f, t);

      t += 10f * Time.deltaTime;

      yield return null;
    }
    wheel.gameObject.SetActive(false);
  }


  public void RestoreHealth(int amount){
    if (curHealth + amount > maxHealth) {
      curHealth = maxHealth;
    } else {
      curHealth += amount;
    }

    heartMeter.Refresh(curHealth, maxHealth);
  }


  private void IncreaseMaxHealth() {
    maxHealth += 2;
    curHealth = maxHealth;
    heartMeter.Refresh(curHealth, maxHealth);
  }


  public void DealDamage(int damage) {
    if (godMode) {
      return;
    }
    curHealth -= damage;
    Debug.Log($"Player dealt {damage} damage. Health = {curHealth}/{maxHealth}");
    heartMeter.Refresh(curHealth, maxHealth);
    if (curHealth <= 0) {
      Debug.Log("health below zero, killing player");
      KillPlayer();
    }
  }


  public void KillPlayer() {
    _voice.PlaySFX("Sounds/SFX/playerDie");
    if (curHealth > 0) {
      StartCoroutine(EventRespawn(_movement.IsGrounded()));
    } else {
      StartCoroutine(OutOfHealthRespawn());
    }
  }

  private IEnumerator EventRespawn(bool showDeath) { // showDeath is temporarily unavailable
    SpriteRenderer _sprite = GetComponent<SpriteRenderer>();
    _movement.StunPlayer(999f, true, "respawn"); // very long because we will unstun the player manually.
    gameObject.layer = 14;
    if (showDeath) {
      _anim.SetTrigger("die");
      yield return new WaitForSeconds(0.75f);
    }
    _sprite.enabled = false;
    if (!_movement.IsGrounded()) {
      _body.gravityScale = 0; // stop you from falling
    }
    _body.velocity = new Vector3(0f, 0f, 0f);

    // fade out

    if (!showDeath) {
      yield return new WaitForSeconds(0.5f); // wait to make sure the player knows what happened
    }
    screenBlackout.gameObject.SetActive(true);

    float t = 0f;
    while (t != 1f) {
      Color deltaA = new Vector4(0f, 0f, 0f, Mathf.Lerp(0f, 1f, t));
      screenBlackout.color = deltaA;
      t += Time.deltaTime;
      if (t > 1f) {
        t = 1;
      }

      yield return null;
    }

    // respawn the player
    // Reload the room prefab (not yet implemented)
    transform.position = respawnPoint;

    _body.gravityScale = 1;
    _sprite.enabled = true;

    curHealth = maxHealth;
    _anim.SetTrigger("respawned");
    yield return new WaitForSeconds(1f);

    heartMeter.Refresh(curHealth, maxHealth);

    // fade in
    t = 0f;
    while (t != 1f) {
      Color deltaA = new Vector4(0f, 0f, 0f, Mathf.Lerp(1f, 0f, t));
      screenBlackout.color = deltaA;
      t += Time.deltaTime;
      if (t > 1f) {
        t = 1;
      }

      yield return null;
    }

    screenBlackout.gameObject.SetActive(false);

    // release the player
    gameObject.layer = 6;
    _movement.Unstun();
  }

  // need to fix camera nonsense during respawn.

  private IEnumerator OutOfHealthRespawn() {
    SpriteRenderer _sprite = GetComponent<SpriteRenderer>();
    _movement.StunPlayer(999f, true, "respawn"); // very long because we will unstun the player manually.
    gameObject.layer = 14;
    while (!_movement.IsGrounded()) {
      yield return null;
    }
    _anim.SetTrigger("die");
    yield return new WaitForSeconds(0.75f); // wait to make sure the player knows what happened
    _sprite.enabled = false;
    _body.velocity = new Vector3(0f, 0f, 0f);

    // fade out
    screenBlackout.gameObject.SetActive(true);

    float t = 0f;
    while (t != 1f) {
      Color deltaA = new Vector4(0f, 0f, 0f, Mathf.Lerp(0f, 1f, t));
      screenBlackout.color = deltaA;
      t += Time.deltaTime;
      if (t > 1f) {
        t = 1;
      }

      yield return null;
    }

    // respawn the player
    // Reload the room prefab (not yet implemented)
    transform.position = respawnPoint;

    _body.gravityScale = 1;
    _sprite.enabled = true;

    curHealth = maxHealth;
    _anim.SetTrigger("respawned");

    yield return new WaitForSeconds(1f);

    heartMeter.Refresh(curHealth, maxHealth);

    // fade in
    t = 0f;
    while (t != 1f) {
      Color deltaA = new Vector4(0f, 0f, 0f, Mathf.Lerp(1f, 0f, t));
      screenBlackout.color = deltaA;
      t += Time.deltaTime;
      if (t > 1f) {
        t = 1;
      }

      yield return null;
    }

    screenBlackout.gameObject.SetActive(false);

    // release the player
    gameObject.layer = 6;
    _movement.Unstun();
  }


  public void UpdateRespawnPoint(Vector3 checkpointPosition) {
    respawnPoint = checkpointPosition;
  }

  public void ActivateDebugWeapon() {
    for (int i = 0; i < 4; i++) {
        UnlockWeapon(i);
        Debug.Log("Weapon " + i + " Unlocked");
      }
  }
}
