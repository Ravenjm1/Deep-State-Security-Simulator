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
    [SerializeField] private GameObject _corpsePrefab;
    private GameObject _corpseObj;
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
        cameraManager.AssignCameraFollow(this);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        Model.SetActive(false);
        currentState = State.Base;

        // Проверяем, инициализирован ли Steam API
        if (SteamManager.Initialized)
        {
            nickname = SteamFriends.GetPersonaName();
        }
    }

    void Start()
    {
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
        }
    }

    private void OnDead()
    {
        if (isLocalPlayer)
        {
            SwitchToDeadState();
        }
        else 
        {
            Model.SetActive(false); 
        }
        if (isServer)
        {
            _corpseObj = Instantiate(_corpsePrefab, Model.transform.position, Quaternion.identity);
            NetworkServer.Spawn(_corpseObj);
        }
    }

    private void OnResurect()
    {
        if (isLocalPlayer)
        {
            SwitchToBaseState();
        }
        else
        {
            Model.SetActive(true);
        }
        if (isServer)
        {
            NetworkServer.Destroy(_corpseObj);
        }
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
