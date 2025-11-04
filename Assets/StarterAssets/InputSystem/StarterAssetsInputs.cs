using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using TouchPhase = UnityEngine.TouchPhase;

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool attack;
        public bool interact;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

        [Header("Mobile Input")]
        public MobileInputManager mobileInput;
        public bool useMobileInput = false;

        [Header("Mobile Look Settings")]
        public bool enableTouchLook = true;
        [Range(0.05f, 0.5f)] public float touchSensitivity = 0.15f;

        private int activeTouchId = -1;
        private Vector2 lastTouchPosition;

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

        public void OnAttack(InputValue value)
        {
            AttackInput(value.isPressed);
        }

        public void OnInteract(InputValue value)
        {
            InteractInput(value.isPressed);
        }
#endif

        void Update()
        {
            // Handle joystick-based movement (left thumb)
            if (useMobileInput && mobileInput != null)
            {
                move = mobileInput.GetMoveInput();
                sprint = mobileInput.IsSprinting();
                attack = mobileInput.IsAttackPressed();
                interact = mobileInput.IsInteractPressed();
            }

            // Handle swipe-based camera look (right thumb or drag anywhere)
            if (enableTouchLook)
            {
                HandleTouchLook();
            }
        }

        private void HandleTouchLook()
        {
            if (Input.touchCount == 0)
            {
                look = Vector2.zero;
                activeTouchId = -1;
                return;
            }

            foreach (Touch touch in Input.touches)
            {
                // Ignore touches on UI (like joystick or buttons)
                if (UnityEngine.EventSystems.EventSystem.current != null &&
                    UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    continue;

                if (touch.phase == TouchPhase.Began && activeTouchId == -1)
                {
                    activeTouchId = touch.fingerId;
                    lastTouchPosition = touch.position;
                }
                else if (touch.fingerId == activeTouchId && touch.phase == TouchPhase.Moved)
                {
                    Vector2 delta = touch.deltaPosition;
                    look = new Vector2(delta.x * touchSensitivity, -delta.y * touchSensitivity);
                }
                else if (touch.phase == TouchPhase.Ended && touch.fingerId == activeTouchId)
                {
                    activeTouchId = -1;
                    look = Vector2.zero;
                }
            }
        }

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void AttackInput(bool newAttackState)
        {
            attack = newAttackState;
        }

        public void InteractInput(bool newInteractState)
        {
            interact = newInteractState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}