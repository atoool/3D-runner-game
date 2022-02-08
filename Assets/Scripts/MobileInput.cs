using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour
{
    private float deadzone = 100f;

    public static MobileInput Instance { set; get; }

    private bool tap, swipeRight, swipeLeft, swipeDown, swipeUp;
    private Vector2 swipeDelta, startTouch;

    public bool Tap { get { return tap; } }
    public bool SwipeRight { get { return swipeRight; } }
    public bool SwipeLeft { get { return swipeLeft; } }
    public bool SwipeDown { get { return swipeDown; } }
    public bool SwipeUp { get { return swipeUp; } }
    public Vector2 SwipeDelta { get { return swipeDelta; } }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        tap = swipeRight = swipeDown = swipeUp = swipeLeft = swipeRight = false;

        #region Standalone inputs
        if (Input.GetMouseButtonDown(0))
        {
            tap = true;
            startTouch = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            startTouch = swipeDelta = Vector2.zero;
        }
        #endregion

        #region mobile inputs
        if (Input.touches.Length!=0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                tap = true;
                startTouch = Input.mousePosition;
            }
          else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                startTouch = swipeDelta = Vector2.zero;
            }
        }

        #endregion

        //distance
        swipeDelta = Vector2.zero;
        if (startTouch != Vector2.zero)
        {
            //mobile
            if (Input.touches.Length != 0)
            {
                swipeDelta = Input.touches[0].position - startTouch;
            }
            else if (Input.GetMouseButton(0))
            {
                swipeDelta =(Vector2)Input.mousePosition - startTouch;
            }
        }

        //deadzone
        if (swipeDelta.magnitude > deadzone)
        {
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if (Mathf.Abs(x) > Mathf.Abs(y)) {
                if(x<0)
                    swipeLeft = true;
                else
                    swipeRight = true;

            }
            else
            {
                if (y < 0)
                    swipeDown = true;
                else
                    swipeUp = true;
            }

            startTouch = swipeDelta = Vector2.zero;

        }
    }
}
