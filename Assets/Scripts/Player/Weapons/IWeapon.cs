using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon {
    int damage {get;}

    void WeaponUpdate(); // called in player's update
    void Attack();
    void Ability(); // sword and claymore wont use this
}
