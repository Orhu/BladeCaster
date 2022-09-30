using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon {
    int damage {get;}
    int abilityEnergyCost {get;} // sword wont use this

    void Attack();
    void Ability(); // sword and claymore wont use this
}
