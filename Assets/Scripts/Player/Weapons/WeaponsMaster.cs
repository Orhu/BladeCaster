using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Sword))]
[RequireComponent(typeof(Grapple))]
[RequireComponent(typeof(Spear))]
[RequireComponent(typeof(Claymore))]
//[RequireComponent(typeof(Claws))]
//[RequireComponent(typeof(Musket))]
//[RequireComponent(typeof(Shield))]
public class WeaponsMaster : MonoBehaviour { // A.K.A. the Arms Dealer A.K.A. The big box in your closet full of pointy shit
    public static Sword sword;
    public static Grapple grapple;
    public static Spear spear;
    public static Claymore claymore;
    /*public static Claws claws;
    public static Musket musket;
    public static Shield shield;*/

    void Start() {
        sword = GetComponent<Sword>();
        grapple = GetComponent<Grapple>();
        spear = GetComponent<Spear>();
        claymore = GetComponent<Claymore>();
        /*claws = GetComponent<Claws>();
        musket = GetComponent<Musket>();
        shield = GetComponent<Shield>();*/
    }
}
