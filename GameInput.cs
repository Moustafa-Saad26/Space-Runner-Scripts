using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance {private set; get;}
    
    private const float _MinGap = 1f;
    Vector2 touchStartPosition;
    Vector2 touchEndPosition;
    Vector2 touchDelta;
    
    public event EventHandler<OnInputPressedEventArgs> OnInputPressed;
    public event EventHandler OnSwipeUp;
    public event EventHandler OnSwipeDown;

    public class OnInputPressedEventArgs : EventArgs
    {
        public Vector3 moveDirection;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        
    }
    private void Update()
    {
        TouchInput();
    }

    private void KehyboadInput()
    {
        //Used for Testing Keyboard Input until full touch implementation or if we convert game to PC later
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnInputPressed?.Invoke(this, new OnInputPressedEventArgs { moveDirection = Vector3.left });
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            OnInputPressed?.Invoke(this, new OnInputPressedEventArgs { moveDirection = Vector3.right });
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            OnSwipeUp?.Invoke(this, EventArgs.Empty);
        }
    }

    private void TouchInput()
    {
        if (Touchscreen.current == null) return;
        TouchControl touch = Touchscreen.current.primaryTouch;
        if (touch.press.wasPressedThisFrame)
        {
            //Debug.Log("Touch pressed");
            touchStartPosition = touch.position.ReadValue();
        }

        if (touch.press.wasReleasedThisFrame)
        {
            //Debug.Log("Touch released");
            touchEndPosition = touch.position.ReadValue();
            touchDelta = touchEndPosition - touchStartPosition;
            //Debug.Log("Touch Delta: " + touchDelta);
            //Debug.Log("Touch Delta magnitude: " + touchDelta.magnitude);
            if (touchDelta.magnitude > _MinGap)
            {
               // Debug.Log("more than Gap Threshold");
                if (Mathf.Abs(touchDelta.x) > Mathf.Abs(touchDelta.y))
                { 
                    if (touchDelta.x > 0)
                    {
                        //Debug.Log("Swipe Right");
                        OnInputPressed?.Invoke(this, new OnInputPressedEventArgs { moveDirection = Vector3.right });
                    }
                    else
                    {
                       // Debug.Log("Swipe Left");
                        OnInputPressed?.Invoke(this, new OnInputPressedEventArgs { moveDirection = Vector3.left });
                    }
                }
                else
                {
                    if (touchDelta.y > 0)
                    {
                       // Debug.Log("Swipe Up");
                        OnSwipeUp?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    { 
                        OnSwipeDown?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            else
            {
               // Debug.Log("Less than Gap Threshold");
            }
        }
    }
}
