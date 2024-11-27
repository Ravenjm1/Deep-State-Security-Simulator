using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using System;
using Mirror.Examples.Basic;
using TMPro;
using Steamworks;

public class PlayerController : NetworkBehaviour
{
    [field: SerializeField] public PlayerInteractor Interactor { get; private set; }
    [field: SerializeField] public PlayerHands Hands { get; private set; }
    [field: SerializeField] public GameObject Model { get; private set; }
    [field: SerializeField] public TMP_Text TmpNickname { get; private set; }

    private CameraManager cameraManager;
    public PlayerStats Stats { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerUI UI { get; private set; }
    
    public IPlayerState CurrentState { get; private set; }
    public BaseState BaseStateInstance { get; private set; } = new BaseState();
    public LookAtState LookAtStateInstance { get; private set; } = new LookAtState();
    public DeadState DeadStateInstance { get; private set; } = new DeadState();

    [SyncVar] string nickname = "";
    
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        Stats = GetComponent<PlayerStats>();
        Movement = GetComponent<PlayerMovement>();
        UI = FindAnyObjectByType<PlayerUI>();

        cameraManager = FindAnyObjectByType<CameraManager>();
        cameraManager.AssignCameraFollow(this); 
        Model.SetActive(false);
        
        // Начальное состояние игрока
        BaseStateInstance.Player = this;
        LookAtStateInstance.Player = this;
        DeadStateInstance.Player = this;

        CurrentState = BaseStateInstance;
        CurrentState.EnterState();

        // Проверяем, инициализирован ли Steam API
        if (SteamManager.Initialized)
        {
            // Получаем ник текущего игрока
            nickname = SteamFriends.GetPersonaName();
        }
    }

    void Update()
    {
        if (isLocalPlayer) 
        {
            // Обновляем текущее состояние
            CurrentState.UpdateState();
        }
        else 
        {
            TmpNickname.text = nickname;
        }
    }

    public void SwitchState(IPlayerState newState)
    {
        CurrentState.ExitState();
        CurrentState = newState;
        CurrentState.EnterState();
    }

    public bool TryToLookAt(LookAtObject lookAtObject)
    {
        if (CurrentState == BaseStateInstance)
        {
            LookAtStateInstance.lookAtObject = lookAtObject;
            SwitchState(LookAtStateInstance);
            return true;
        }
        return false;
    }

    public void SetRotationZ(float zAngle)
    {
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.z = zAngle;
        transform.eulerAngles = currentRotation;
    }
}

public interface IPlayerState
{
    public PlayerController Player { get; set; }
    void EnterState();
    void UpdateState();
    void ExitState();
}

public class BaseState : IPlayerState
{
    public PlayerController Player { get; set; }
    public void EnterState()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UpdateState()
    {
        if (Player.Stats.IsDead) // Проверка на смерть
        {
            Player.SwitchState(Player.DeadStateInstance);
        }
    }

    public void ExitState()
    {
        // Действия при выходе из состояния
    }
}

public class LookAtState : IPlayerState
{
    public PlayerController Player { get; set; }
    public LookAtObject lookAtObject;
    public void EnterState()
    {
        Player.Movement.enabled = false;
        Cursor.lockState = CursorLockMode.Confined;
        InputManager.Instance.OnInteraction += Interact;
        Player.UI.ChangeRenderUI(false);
    }

    void Interact()
    {
        Player.SwitchState(Player.BaseStateInstance);
    }

    public void UpdateState()
    {
        if (Player.Stats.IsDead) // Проверка на смерть
        {
            Player.SwitchState(Player.DeadStateInstance);
        }
    }

    public void ExitState()
    {
        // Скрываем курсор при выходе из состояния
        Cursor.lockState = CursorLockMode.Locked;
        Player.Movement.enabled = true;
        lookAtObject.StopLooking();
        InputManager.Instance.OnInteraction -= Interact;
        Player.UI.ChangeRenderUI(true);
    }
}

public class DeadState : IPlayerState
{
    public PlayerController Player { get; set; }
    public void EnterState()
    {
        Player.Movement.enabled = false;
        Player.SetRotationZ(90);
        Player.UI.ChangeRenderUI(false);
    }

    public void UpdateState()
    {
        if (!Player.Stats.IsDead) // Респавн игрока
        {
            Player.transform.position = new Vector3(0f, 2f, 0f); // Перемещаем в координаты 0,0
            Player.SwitchState(Player.BaseStateInstance);
        }
    }

    public void ExitState()
    {
        Player.Movement.enabled = true;
        Player.SetRotationZ(0);
        Player.UI.ChangeRenderUI(true);
    }
}

