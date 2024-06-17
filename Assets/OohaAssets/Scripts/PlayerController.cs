using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Cinemachine;

namespace EmotionalBaggage.Player
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private CinemachineVirtualCamera virtualCamera;

        [SerializeField]
        private float initialPlayerSpeed = 4f;

        [SerializeField]
        private float maximumPlayerSpeed = 30f;

        [SerializeField]
        private float playerSpeedIncreaseRate = .1f;

        [SerializeField]
        private float horizontalSpeedMultiplier = 5f;

        [SerializeField]
        private float jumpHeight = 1.0f;

        [SerializeField]
        private float initialGravityValue = -9.81f;

        [SerializeField]
        private LayerMask groundLayer;
        [SerializeField]
        private LayerMask turnLayer;
        [SerializeField]
        private LayerMask obstacleLayer;
        [SerializeField]
        private LayerMask airObstacleLayer;
        [SerializeField]
        private Animator animator;
        private float horizontalSpeed = 2f;
        [SerializeField]
        private AnimationClip slideAnimationClip;

        [SerializeField]
        private AnimationClip dieAnimationClip;
        private float gravity;
        private float playerSpeed;
        private Vector3 movementDirection = Vector3.forward;
        private Vector3 playerVelocity;
        private PlayerInput playerInput;
        private InputAction turnAction;
        private InputAction jumpAction;
        private InputAction slideAction;
        private InputAction moveAction;

        private CharacterController controller;

        private int slidingAnimationId;
        private int dyingAnimationId;
        private bool sliding = false;
        private float score = 0;
        private bool isFalling = false;
        private float fallTimer = 0f;
        private float maxFallTime = 2f;
        private float targetOffsetZ;
        private float offsetTransitionSpeed = 0.7f;
        private bool isGameOver = false;

        [SerializeField]
        private UnityEvent<Vector3> turnEvent;

        [SerializeField]
        private UnityEvent<int> gameOverEvent;

        [SerializeField]
        private UnityEvent<int> scoreUpdateEvent;

        private Vector3? lastTurnTilePosition = null;
        private bool isHorizHeld;
        private Vector3 horizontalMove;

        private void Awake() {
            playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<CharacterController>();

            slidingAnimationId = Animator.StringToHash("Sliding");
            dyingAnimationId = Animator.StringToHash("Dying");

            moveAction = playerInput.actions["Move"];
            turnAction = playerInput.actions["Turn"];
            jumpAction = playerInput.actions["Jump"];
            slideAction = playerInput.actions["Slide"];
            
        }

        private void OnEnable() {
            moveAction.performed += PlayerMove;
            moveAction.canceled += HorizReleased;
            turnAction.performed += PlayerTurn;
            slideAction.performed += PlayerSlide;
            jumpAction.performed += PlayerJump;
        }

        private void OnDisable() {
            moveAction.performed -= PlayerMove; 
            moveAction.canceled -= HorizReleased;
            turnAction.performed -= PlayerTurn;
            slideAction.performed -= PlayerSlide;
            jumpAction.performed -= PlayerJump;
        }

        private void PlayerMove(InputAction.CallbackContext context)
        {
            Vector3? turnBlock = CheckTurn(context.ReadValue<float>());
            if (turnBlock.HasValue)
            {
                return;
            }

            isHorizHeld = true;
            float direction = context.ReadValue<float>();
            horizontalMove = transform.right * context.ReadValue<float>() * horizontalSpeed * Time.deltaTime;

        }
        private void Start()
        {
            virtualCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = -2.44f;
            gravity = initialGravityValue;
            playerSpeed = initialPlayerSpeed;
        }

        private void HorizReleased(InputAction.CallbackContext context)
        {
            isHorizHeld = false;
        }

        private void PlayerTurn(InputAction.CallbackContext context)
        {
            if (!isGameOver){
                Vector3? turnPosition = CheckTurn(context.ReadValue<float>());
                if (!turnPosition.HasValue)
                {
                    return;
                }
                Vector3 targetDirection = Quaternion.AngleAxis(90 * context.ReadValue<float>(), Vector3.up) * movementDirection;
                turnEvent.Invoke(targetDirection);
                Turn(context.ReadValue<float>(), turnPosition.Value);
            }
        }

        private Vector3? CheckTurn(float turnValue)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, .1f, turnLayer);
            if (hitColliders.Length != 0)
            {
                Tile tile = hitColliders[0].transform.parent.GetComponent<Tile>();
                TileType type = tile.type;
                Vector3 tilePosition = tile.transform.position;

                if (lastTurnTilePosition.HasValue && lastTurnTilePosition.Value == tilePosition)
                {
                    return null;
                }
                if ((type == TileType.LEFT && turnValue == -1) ||
                    (type == TileType.RIGHT && turnValue == 1) ||
                    (type == TileType.SIDEWAYS))
                {
                    lastTurnTilePosition = tilePosition;
                    return tile.pivot.position;
                }
            }
            return null;
        }

        private void Turn(float turnValue, Vector3 turnPosition)
        {
            Vector3 tempPlayerPosition = new Vector3(turnPosition.x, transform.position.y, turnPosition.z);
            controller.enabled = false;
            transform.position = tempPlayerPosition;
            controller.enabled = true;

            Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 90 * turnValue, 0);
            transform.rotation = targetRotation;
            movementDirection = transform.forward.normalized;
        }
        private void PlayerSlide(InputAction.CallbackContext context)
        {
            if (!isGameOver)
            {
                if (!sliding && isGrounded())
                {
                    StartCoroutine(Slide());
                }
            }
        }

        private IEnumerator Slide()
        {
            sliding = true;
            // Shrink the collider
            Vector3 originalControllerCenter = controller.center;
            Vector3 newControllerCenter = originalControllerCenter;
            controller.height /= 2;
            newControllerCenter.y -= controller.height / 2;
            controller.center = newControllerCenter;

            // Play the sliding animation
            animator.Play(slidingAnimationId);
            yield return new WaitForSeconds(slideAnimationClip.length / animator.speed);
            // Set the character controller back to normal after sliding
            controller.height *= 2;
            controller.center = originalControllerCenter;
            sliding = false;
        }
        private void PlayerJump(InputAction.CallbackContext context)
        {
            if (!isGameOver)
            {
                if (isGrounded())
                {
                    playerVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
                    controller.Move(playerVelocity * Time.deltaTime);
                }
            }
        }

        private void Update()
        {
            if (!isGrounded(20f))
            {

                if (!isFalling)
                {
                    isFalling = true;
                    fallTimer = 0f;
                }
                else
                {
                    fallTimer += Time.deltaTime;
                    if (fallTimer != 0)
                    {
                        isGameOver = true;
                    }
                    if (fallTimer >= maxFallTime)
                    {
                        GameOver();
                        return;
                    }
                }
            }
            else
            {
                isFalling = false;
            }

            //if game done, stop the score
            if (!isGameOver)
            {
                score += playerSpeed * Time.deltaTime;
                scoreUpdateEvent.Invoke((int)score);
            }

            controller.Move(transform.forward * playerSpeed * Time.deltaTime);

            if (isHorizHeld && horizontalMove != null)
            {
                controller.Move(horizontalMove);
            }

            if (isGrounded() && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            playerVelocity.y += gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

            if (!Physics.CheckSphere(transform.position, .1f, turnLayer))
            {
                lastTurnTilePosition = null;
            }

            if (playerSpeed < maximumPlayerSpeed)
            {
                playerSpeed += Time.deltaTime * playerSpeedIncreaseRate;
                gravity = initialGravityValue - playerSpeed;

                if (animator.speed < 1.25)
                {
                    animator.speed += (1 / playerSpeed) * Time.deltaTime;
                }
            }

        }

        private bool isGrounded(float length = .2f)
        {
            Vector3 raycastOriginFirst = transform.position;
            raycastOriginFirst.y -= controller.height / 2f;
            raycastOriginFirst.y += .1f;

            Vector3 raycastOriginSecond = raycastOriginFirst;
            raycastOriginFirst += transform.forward * .2f;
            raycastOriginSecond -= transform.forward * .2f;

            if (Physics.Raycast(raycastOriginFirst, Vector3.down, out RaycastHit hit, length, groundLayer) || Physics.Raycast(raycastOriginSecond, Vector3.down, out RaycastHit hit2, length, groundLayer))
            {
                return true;
            }
            return false;
        }

        private void GameOver()
        {
            isGameOver = true;
            StartCoroutine(Die());
        }


        private IEnumerator Die()
        {
            if (!isFalling)
            {
                animator.Play(dyingAnimationId);
                yield return new WaitForSeconds(dieAnimationClip.length);
            }
            gameObject.SetActive(false);
            gameOverEvent.Invoke((int)score);
        }

        private IEnumerator WaitUntilGroundedThenDie()
        {
            yield return new WaitUntil(() => isGrounded());
            GameOver();
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (sliding)
            {
                if (((1 << hit.collider.gameObject.layer) & obstacleLayer) != 0)
                {
                    isGameOver = true;
                    StartCoroutine(ChangeCameraOffsetSmoothly(-7f));
                    StartCoroutine(WaitUntilGroundedThenDie());
                }
            }
            else if (((1 << hit.collider.gameObject.layer) & obstacleLayer) != 0 || ((1 << hit.collider.gameObject.layer) & airObstacleLayer) != 0)
            {
                isGameOver = true;
                StartCoroutine(ChangeCameraOffsetSmoothly(-7f));
                StartCoroutine(WaitUntilGroundedThenDie());
            }
        }

        private IEnumerator ChangeCameraOffsetSmoothly(float targetOffset)
        {
            CinemachineTransposer transposer = virtualCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();
            float initialOffset = transposer.m_FollowOffset.z;
            float elapsedTime = 0f;

            while (Mathf.Abs(transposer.m_FollowOffset.z - targetOffset) > 0.01f)
            {
                elapsedTime += Time.deltaTime;
                transposer.m_FollowOffset.z = Mathf.Lerp(initialOffset, targetOffset, elapsedTime * offsetTransitionSpeed);
                yield return null;
            }

            transposer.m_FollowOffset.z = targetOffset;
        }
    }
}
