using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace EmotionalBaggage.Player {
[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float initialPlayerSpeed = 4f;

    [SerializeField]
    private float maximumPlayerSpeed = 30f;

    [SerializeField]
    private float playerSpeedIncreaseRate = .1f;

    [SerializeField]
    private float jumpHeight = 1.0f;

    [SerializeField]
    private float initialGravityValue = -9.81f;

    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private LayerMask turnLayer;
    private float gravity;
    private float playerSpeed;
    private Vector3 movementDirection = Vector3.forward;
    private Vector3 playerVelocity;
    private PlayerInput playerInput;
    private InputAction turnAction;
    private InputAction jumpAction;
    private InputAction slideAction;

    private CharacterController controller;

    [SerializeField]
    private UnityEvent<Vector3> turnEvent;

     private Vector3? lastTurnTilePosition = null;

    private void Awake() {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        turnAction = playerInput.actions["Turn"];
        jumpAction = playerInput.actions["Jump"];
        slideAction = playerInput.actions["Slide"];
        
    }

    private void OnEnable() {
        turnAction.performed += PlayerTurn;
        slideAction.performed += PlayerSlide;
        jumpAction.performed += PlayerJump;
    }

    private void OnDisable() {
        turnAction.performed -= PlayerTurn;
        slideAction.performed -= PlayerSlide;
        jumpAction.performed -= PlayerJump; 
    }

    private void Start() {
        gravity = initialGravityValue;
        playerSpeed = initialPlayerSpeed;
    }

    private void PlayerTurn(InputAction.CallbackContext context) {
        Vector3? turnPosition = CheckTurn(context.ReadValue<float>());
        if (!turnPosition.HasValue) {
            return;
        }
        Vector3 targetDirection = Quaternion.AngleAxis(90 * context.ReadValue<float>(), Vector3.up) * movementDirection;
        turnEvent.Invoke(targetDirection);
        Turn(context.ReadValue<float>(), turnPosition.Value);
    }

    private Vector3? CheckTurn(float turnValue) {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, .1f, turnLayer);
        if (hitColliders.Length != 0) {
            Tile tile = hitColliders[0].transform.parent.GetComponent<Tile>();
            TileType type = tile.type;
            Vector3 tilePosition = tile.transform.position;

            if (lastTurnTilePosition.HasValue && lastTurnTilePosition.Value == tilePosition) {
                    return null;
                }
            if ((type == TileType.LEFT && turnValue == -1) ||
                (type == TileType.RIGHT && turnValue == 1) ||
                (type == TileType.SIDEWAYS)) {
                lastTurnTilePosition = tilePosition;
                return tile.pivot.position;
            }
        }
        return null;
    }

    private void Turn(float turnValue, Vector3 turnPosition) {
        Vector3 tempPlayerPosition = new Vector3(turnPosition.x, transform.position.y, turnPosition.z);
        controller.enabled = false;
        transform.position = tempPlayerPosition;
        controller.enabled = true;

        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 90 * turnValue, 0);
        transform.rotation = targetRotation;
        movementDirection = transform.forward.normalized;
    }
    private void PlayerSlide(InputAction.CallbackContext context) {
        
    }

    private void PlayerJump(InputAction.CallbackContext context) {
        if (isGrounded()) {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * gravity * -3f);
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }   

    private void Update() {
        controller.Move(transform.forward * playerSpeed * Time.deltaTime);

        if (isGrounded() && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (!Physics.CheckSphere(transform.position, .1f, turnLayer)) {
            lastTurnTilePosition = null;
        }

        Debug.Log("Grounded: " + isGrounded());
    }

    private bool isGrounded(float length = .2f){
        Vector3 raycastOriginFirst = transform.position;
        raycastOriginFirst.y -= controller.height / 2f;
        raycastOriginFirst.y += .1f;

        Vector3 raycastOriginSecond = raycastOriginFirst;
        raycastOriginFirst += transform.forward * .2f;  
        raycastOriginSecond -= transform.forward * .2f; 

        Debug.DrawLine(raycastOriginFirst, raycastOriginFirst + Vector3.down * length, Color.green, 2f);
        Debug.DrawLine(raycastOriginSecond, raycastOriginSecond + Vector3.down * length, Color.red, 2f);

        if (Physics.Raycast(raycastOriginFirst, Vector3.down, out RaycastHit hit, length, groundLayer) || Physics.Raycast(raycastOriginSecond, Vector3.down, out RaycastHit hit2, length, groundLayer)){
            return true;
        }
        return false;
    }
}
}
