// -*- mode:csharp -*-
//
// MIT License.
//

using UnityEngine;
using UnityEngine.Events;

using System.Collections;

namespace TPS
{

public interface IThrowGestureDelegate
{
    void OnThrow(int direction);
}

public class ThrowGestureManager : MonoBehaviour
{
    public IThrowGestureDelegate throwGestureHandler;
    public int direction;
    public float minSwipeDist;
    public float maxSwipeTime;

    static int DEFAULT_DIRECTION = 4;
    static float DEFAULT_MIN_THROW_DIST = 100.0f;
    static float DEFAULT_MAX_THROW_TIME = 0.5f;

    float startTime;
    Vector2 startPos;
    bool couldBeSwipe;

    // Unity Events.
    void Awake()
    {
        if (direction == 0 && minSwipeDist == 0 && maxSwipeTime == 0) {
            // Set default value.
            direction = DEFAULT_DIRECTION;
            minSwipeDist = DEFAULT_MIN_THROW_DIST;
            maxSwipeTime = DEFAULT_MAX_THROW_TIME;
        }
    }

    void Update()
    {
// If build for UnityEditor, handle mouse inputs as touch for easy debugging.
        if (UseFakeInput()) {
            MouseInput();
        } else {
            TouchInput();
        }
    }

    // Private.

    bool UseFakeInput()
	{
        return !Application.isMobilePlatform;
    }

    void TouchInput()
    {
        if (Input.touchCount <= 0) {
            return;
        }

        Touch touch = Input.touches[0];
        switch (touch.phase) {
            case TouchPhase.Began:
                InputPhaseBegan(touch.position);
                break;
            case TouchPhase.Canceled:
                InputPhaseCancelled(touch.position);
                break;
            case TouchPhase.Ended:
                InputPhaseEnded(touch.position);
                break;
        }
    }

    void MouseInput()
    {
        if (Input.GetMouseButtonDown(0)) {
            InputPhaseBegan(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0)) {
            InputPhaseEnded(Input.mousePosition);
        }
    }

    void InputPhaseBegan(Vector2 position)
    {
        couldBeSwipe = true;
        startPos = position;
        startTime = Time.time;
    }

    void InputPhaseCancelled(Vector2 position)
    {
        couldBeSwipe = false;
    }

    void InputPhaseEnded(Vector2 position)
    {
        float swipeTime = Time.time - startTime;
        float swipeDist = (position - startPos).magnitude;
        bool swipeActive =
            couldBeSwipe
            && (swipeTime < maxSwipeTime)
            && (swipeDist > minSwipeDist);

        if (!swipeActive) {
            return;
        }

        float dX = position.x - startPos.x;
        float dY = position.y - startPos.y;
        float degree = Mathf.Atan2(dY, dX) * Mathf.Rad2Deg;
        if (degree < 0) {
            degree += 360;
        } else if (degree >= 360) {
            degree -= 360;
        }

        int result = -1;
        float shift = 360.0f / direction / 2;
        for (int i = 0; i < direction+1; i++) {
            float b = 360.0f / direction * i;
            float min = b - shift;
            float max = b + shift;
            int num = i % direction;
            if (min <= degree && degree < max) {
                result = num;
                break;
            }
        }

        if (result < 0) {
            return;
        }

        if (throwGestureHandler != null) {
            throwGestureHandler.OnThrow(result);
        }
    }
}

}

// EOF
