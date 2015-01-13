// -*- mode:csharp -*-
//
// MIT License.
//

using UnityEngine;

using TPS;

public class ExampleRunner: MonoBehaviour, IThrowGestureDelegate
{
    void Start()
    {
        GetComponent<ThrowGestureManager>().throwGestureHandler = this;
    }

    // IThrowGestureDelegate
    public void OnThrow(int direction)
    {
        Debug.Log("Detect: direction: " + direction);
    }
}

// EOF
