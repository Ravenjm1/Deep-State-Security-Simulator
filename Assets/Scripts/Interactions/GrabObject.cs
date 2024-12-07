using System;
using Mirror;
using UnityEngine;

public class GrabObject : NetworkBehaviour, IInteractable
{
    private Rigidbody _rigidbody;
    BoxCollider _boxCollider;
    PlayerController grabber;
    Transform grabberHands;
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

        gameObject.AddComponent<OutlineInteract>();
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
    public void CmdGrab(NetworkIdentity player)
    {
        if (!_isGrabbed)
        {
            _isGrabbed = true;
            if (_rigidbody)
            {
                _rigidbody.isKinematic = true;
                _boxCollider.enabled = false;
            }
            
            grabber = player.GetComponent<PlayerController>();

            SetGrabbed(grabber.Hands.Center);

            RpcSetOwner(player);
            // Локаем интеракцию у клиента
            TargetGrab(player.connectionToClient, player);
        }
    }

    void SetGrabbed(Transform grabTransform)
    {
        grabberHands = grabTransform;
        transform.SetParent(grabberHands);
        transform.localPosition = Vector3.zero; // Устанавливаем начальное положение
        transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    public void GrabNpc(Transform grabTransform)
    {
        if (!_isGrabbed)
        {
            _isGrabbed = true;
            if (_rigidbody)
            {
                _rigidbody.isKinematic = true;
                _boxCollider.enabled = false;
            }
            SetGrabbed(grabTransform);
        }
    }

    [Command (requiresAuthority = false)]
    private void CmdDrop()
    {
        if (_isGrabbed)
        {
            _isGrabbed = false;
            if (_rigidbody)
            {
                _rigidbody.isKinematic = false;
                _boxCollider.enabled = true;
            }
            RpcDrop();
        }
    } 

    [ClientRpc]
    void RpcSetOwner(NetworkIdentity player)
    {
        grabber = player.GetComponent<PlayerController>();
        SetGrabbed(grabber.Hands.Center); // Устанавливаем начальное положение
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
    public string GetInteractText() => "Take";
}
