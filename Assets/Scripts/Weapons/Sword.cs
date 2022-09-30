using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon {
    public int damage {get; private set;}
    public int abilityEnergyCost {get; private set;} = 0;

    public void WeaponUpdate() {
        // TO DO
    }

    public void Attack() {
        // animate later, turn on hurtbox and check for collision, basically.
    }


    public void Ability() {
        // Not Used
    }
}
