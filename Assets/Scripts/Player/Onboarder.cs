using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*                             WELCOME TO THE ONBOARDER
========================================================================================

    This script contains a number of static variables (flags) indicating the player's
    progression through learning the game's controls. Each one is tied to a tutorial
    that will appear at some point in the game. These variables will be looked at by
    the tutorial trigger field to check if the player has cleared a tutorial before
    displaying a button tooltip above them as a little hint/control teaching method.

    Please feel free to add new variables here. Make sure you use the same format as
    the existing variables for consistency. We may want to use this for things like
    teaching how to use various weapons or interact with different elements of the
    levels like switches. Check the demo level for an example of how to use this.

    If you add a new flag, make sure you update the GetTutorialStatus(),
    GetTutorialKeyCode(), and CompleteTutorial() methods to add the new flag into the 
    switch statement.

*/

public class Onboarder : MonoBehaviour {
    public static bool CLEARED_ATTACK_TUTORIAL = false; // z key tutorial (0)
    public static bool CLEARED_ABILITY_TUTORIAL = false; // x key tutorial (1)
    public static bool CLEARED_WEAPONWHEEL_TUTORIAL = false; // shift key tutorial (2)
    public static bool CLEARED_RAWCYCLE_TUTORIAL = false; // c key tutorial (3)

    public static bool SPEAR_BOSS_DEFEATED = false;

    public static KeyCode GetTutorialKeyCode(int tutorialNumber) {
        switch (tutorialNumber) {
            case 0:
                return KeyCode.Z;
            case 1:
                return KeyCode.X;
            case 2:
                return KeyCode.LeftShift;
            case 3:
                return KeyCode.C;
            default:
                Debug.LogWarning($"Unknown tutorial flag requested: {tutorialNumber}");
                return KeyCode.Z;
        }
    }

    public static bool GetTutorialStatus(int tutorialNumber) {
        switch (tutorialNumber) {
            case 0:
                return CLEARED_ATTACK_TUTORIAL;
            case 1:
                return CLEARED_ABILITY_TUTORIAL;
            case 2:
                return CLEARED_WEAPONWHEEL_TUTORIAL;
            case 3:
                return CLEARED_RAWCYCLE_TUTORIAL;
            default:
                Debug.LogWarning($"Unknown tutorial flag requested: {tutorialNumber}");
                return false;
        }
    }

    public static void CompleteTutorial(int tutorialNumber) {
        switch (tutorialNumber) {
            case 0:
                CLEARED_ATTACK_TUTORIAL = true;
                return;
            case 1:
                CLEARED_ABILITY_TUTORIAL = true;
                return;
            case 2:
                CLEARED_WEAPONWHEEL_TUTORIAL = true;
                return;
            case 3:
                CLEARED_RAWCYCLE_TUTORIAL = true;
                return;
            default:
                Debug.LogWarning($"Unknown tutorial flag requested: {tutorialNumber}");
                return;
        }
    }
}
