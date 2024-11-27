using System;
using Mirror;
using UnityEngine;

public class GrabObject : NetworkBehaviour, IInteractable
{
    private Rigidbody _rigidbody;
    BoxCollider _boxCollider;
    PlayerController grabber;
    GameObject grabberHands;
    [SyncVar] private bool _isGrabbed;
    [SyncVar] private NetworkIdentity ownerIdentity;
    private Vector3 targetLookAtPosition;
    private int interactLayer; // Слой по умолчанию
    private int inventoryLayer; // Слой для объектов в инвентаре

    public bool IsGrabbed => _isGrabbed;
    
    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
        interactLayer = LayerMask.NameToLayer("Interact"); // Слой по умолчанию
        inventoryLayer = LayerMask.NameToLayer("InventoryLayer"); // Слой для объектов в инвентаре
    }

    void Update()
    {
        if (_isGrabbed && grabber != null)
        {
            //...
        }
    }

    public void SetInInventory(bool inInventory)
    {
        SetLayerRecursively(gameObject, inInventory? interactLayer : inventoryLayer);
        CmdSetInInventory(inInventory);
    }

    // Устанавливаем слой для всех дочерних объектов
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    [Command (requiresAuthority = false)]
    void CmdSetInInventory(bool inInventory)
    {
        RpcSetInInventory(inInventory);
    }

    [ClientRpc]
    void RpcSetInInventory(bool inInventory)
    {
        SetLayerRecursively(gameObject, inInventory? interactLayer : inventoryLayer);
    }

    public void Interact()
    {
        // Говорим серверу, что мы взяли предмет
        CmdGrab(NetworkClient.localPlayer);
    }

    public void Drop()
    {
        CmdDrop();
    }

    [Command (requiresAuthority = false)]
    private void CmdGrab(NetworkIdentity player)
    {
        if (!_isGrabbed)
        {
            _isGrabbed = true;
            _rigidbody.isKinematic = true;
            _boxCollider.enabled = false;
            
            grabber = player.GetComponent<PlayerController>();
            grabberHands = grabber.Hands.Center.gameObject;

            transform.SetParent(grabberHands.transform);
            transform.localPosition = Vector3.zero; // Устанавливаем начальное положение
            transform.localRotation = Quaternion.Euler(0, 180, 0);

            RpcSetOwner(player);
            // Локаем интеракцию у клиента
            TargetGrab(player.connectionToClient, player);
        }
    }

    [Command (requiresAuthority = false)]
    private void CmdDrop()
    {
        if (_isGrabbed)
        {
            _isGrabbed = false;
            _rigidbody.isKinematic = false;
            _boxCollider.enabled = true;
            RpcDrop();
        }
    } 

    [ClientRpc]
    void RpcSetOwner(NetworkIdentity player)
    {
        grabber = player.GetComponent<PlayerController>();
        grabberHands = grabber.Hands.Center.gameObject;

        transform.SetParent(grabberHands.transform);
        transform.localPosition = Vector3.zero; // Устанавливаем начальное положение
    }

    [TargetRpc]
    void TargetGrab(NetworkConnectionToClient target, NetworkIdentity player)
    {
        player.GetComponentInChildren<PlayerInventory>().AddItemToInventory(this);
    }

    [ClientRpc]
    void RpcDrop()
    {
        grabber = null;

        // Убираем объект из дочерней иерархии
        transform.SetParent(null);

    }

    public bool IsGrabberPlayer()
    {
        return grabber? grabber == LocationContext.GetDependency.Player : false;
    }
    public PlayerController GetGrabber() => grabber;

    public InteractionType Type() => InteractionType.GRAB;
    public bool IsActive() => !_isGrabbed;
}
