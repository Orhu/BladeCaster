using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy {
    int health {get;}
    bool invulnerable {get;}
    
    bool IsInvulnerable(); // returns if the enemy is currently immune to player damage
    void Attack(); // enemy attacks player (instakill if it hits so no need for damage)
    void GetHit(int damage, float strength); // player attacks enemy
}
