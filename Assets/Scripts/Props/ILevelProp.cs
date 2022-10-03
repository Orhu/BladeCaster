using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelProp {
    void Interact(); // letting the player interact with the prop
    void SwitchToggle(); // letting switches interact with the prop
}
