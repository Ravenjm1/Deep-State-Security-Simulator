using UnityEngine;
using Mirror;
using TMPro;
using Steamworks;
using System;

public class PlayerController : NetworkBehaviour
{
    [field: SerializeField] public PlayerInteractor Interactor { get; private set; }
    [field: SerializeField] public PlayerHands Hands { get; private set; }
    [field: SerializeField] public GameObject Model { get; private set; }
    [field: SerializeField] public TMP_Text TmpNickname { get; private set; }
    private Animator animator;

    private CameraManager cameraManager;
    public PlayerStats Stats { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerUI UI { get; private set; }

    private State currentState;
    private LookAtObject currentLookAtObject;

    [SyncVar] private string nickname = "";

    private enum State
    {
        Base,
        LookAt,
        Dead
    }

    void Awake()
    {
        Stats = GetComponent<PlayerStats>();
        Movement = GetComponent<PlayerMovement>();
        UI = FindAnyObjectByType<PlayerUI>();
        animator = GetComponentInChildren<Animator>();

        cameraManager = FindAnyObjectByType<CameraManager>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        Model.SetActive(false);
        
        currentState = State.Base;
        EnterBaseState();

        // Проверяем, инициализирован ли Steam API
        if (SteamManager.Initialized)
        {
            nickname = SteamFriends.GetPersonaName();
        }

        cameraManager.AssignCameraFollow(this);

        Stats.OnDead += OnDead;
        Stats.OnResurect += OnResurect;
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            switch (currentState)
            {
                case State.Base:
                    HandleBaseState();
                    break;
                case State.LookAt:
                    HandleLookAtState();
                    break;
                case State.Dead:
                    HandleDeadState();
                    break;
            }
        }
        else
        {
            TmpNickname.text = nickname;

            // Направляем ник в сторону игрока.
            Vector3 direction = NetworkClient.localPlayer.transform.position - TmpNickname.transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

            // Добавляем поворот на 180 градусов по оси Y.
            rotation *= Quaternion.Euler(0, 180, 0);

            TmpNickname.transform.rotation = rotation;
        }
    }

    private void OnDead()
    {
        SwitchToDeadState();
    }

    private void OnResurect()
    {
        SwitchToBaseState();
    }

    public void SwitchToBaseState()
    {
        ExitCurrentState();
        currentState = State.Base;
        EnterBaseState();
    }

    public void SwitchToLookAtState(LookAtObject lookAtObject)
    {
        ExitCurrentState();
        currentLookAtObject = lookAtObject;
        currentState = State.LookAt;
        EnterLookAtState();
    }

    public void SwitchToDeadState()
    {
        ExitCurrentState();
        currentState = State.Dead;
        EnterDeadState();
    }

    private void ExitCurrentState()
    {
        switch (currentState)
        {
            case State.Base:
                ExitBaseState();
                break;
            case State.LookAt:
                ExitLookAtState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }
    }

    // === Base State ===
    private void EnterBaseState()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void HandleBaseState()
    {
        // do...
    }

    private void ExitBaseState()
    {
        // Действия при выходе из BaseState
    }

    // === LookAt State ===
    private void EnterLookAtState()
    {
        Movement.enabled = false;
        Cursor.lockState = CursorLockMode.Confined;
        InputManager.Instance.OnInteraction += InteractWithObject;
        UI.ChangeRenderUI(false);
    }

    private void HandleLookAtState()
    {
        // do...
    }

    private void ExitLookAtState()
    {
        Movement.enabled = true;
        currentLookAtObject?.StopLooking();
        InputManager.Instance.OnInteraction -= InteractWithObject;
        UI.ChangeRenderUI(true);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void InteractWithObject()
    {
        SwitchToBaseState();
    }

    // === Dead State ===
    private void EnterDeadState()
    {
        GetComponent<CharacterController>().enabled = false;
        Movement.enabled = false;
        UI.ChangeRenderUI(false);
    }

    private void HandleDeadState()
    {
        // do...
    }

    private void ExitDeadState()
    {
        transform.position = new Vector3(0f, 2f, 0f); // Перемещаем игрока
        GetComponent<CharacterController>().enabled = true;
        Movement.enabled = true;
        UI.ChangeRenderUI(true);
    }

    public bool TryToLookAt(LookAtObject lookAtObject)
    {
        if (currentState == State.Base)
        {
            SwitchToLookAtState(lookAtObject);
            return true;
        }
        return false;
    }
}
