using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private PlayerAnimator _animator;
    private CharacterController controller;
    private Camera mainCamera;
    private Transform cameraTransform;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float speed = 5.0f;
    float airControlFactor = 0.5f; // Коэффициент управления в воздухе
    private float jumpHeight = 1.5f;
    private float checkJumpTime = 0.2f; // Время на чек прыжка
    private float checkJump;
    float fallGravityMultiplier = 1.0f;  // Ускорение падения
    float jumpGravityMultiplier = 2.0f;  // Замедление прыжка
    private float gravityValue = -9.81f;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        
        controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<PlayerAnimator>(true);

        mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movement = InputManager.Instance.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0f, movement.y);
        if (!groundedPlayer) {
            move.x *= airControlFactor;
        }
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * speed);
        
        if (_animator) {
            _animator.MoveAnimation(move.magnitude);
        }

        if (InputManager.Instance.GetJumped())
        {
            checkJump = checkJumpTime;
        }
        if (checkJump > 0 && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            checkJump = 0;

            if (_animator) {
                _animator.JumpAnimation();
            }
        }

        // Если игрок падает (playerVelocity.y < 0), используем ускоренное падение
        if (playerVelocity.y < 0)
        {
            playerVelocity.y += gravityValue * fallGravityMultiplier * Time.deltaTime;
        }
        // Если игрок поднимается (playerVelocity.y > 0), используем замедленное падение
        else if (playerVelocity.y > 0 && !InputManager.Instance.GetJumped())
        {
            playerVelocity.y += gravityValue * jumpGravityMultiplier * Time.deltaTime;
        }
        else
        {
            playerVelocity.y += gravityValue * Time.deltaTime;
        }
        controller.Move(playerVelocity * Time.deltaTime);

        // Вращение
        // Получаем только горизонтальное вращение (по оси Y)
        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = targetRotation;
    }
}
