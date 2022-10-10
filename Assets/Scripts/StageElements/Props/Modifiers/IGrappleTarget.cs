using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrappleTarget {
    bool rooted {get;} // is the target rooted in place
    float weight {get;} // for the ratio of how much the thing is pulled to you (unless rooted)

    void Target(); // creates a target particle to show you can hook the object
}
