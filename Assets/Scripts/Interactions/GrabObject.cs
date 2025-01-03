using System;
using Mirror;
using UnityEngine;

public class GrabObject : NetworkBehaviour, IInteractable
{
    private Rigidbody _rigidbody;
    BoxCollider _boxCollider;
    PlayerController grabber;
    Transform grabberHands;
    private bool _isGrabbed;
    private int interactLayer; // Слой по умолчанию
    private int inventoryLayer; // Слой для объектов в инвентаре
    private OutlineInteract _outline;

    public bool IsGrabbed => _isGrabbed;
    
    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
        interactLayer = LayerMask.NameToLayer("Interact"); // Слой по умолчанию
        inventoryLayer = LayerMask.NameToLayer("InventoryLayer"); // Слой для объектов в инвентаре

        _outline = gameObject.AddComponent<OutlineInteract>();
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

    void SetGrabbed(Transform grabTransform)
    {
        _isGrabbed = true;
        grabberHands = grabTransform;
        transform.SetParent(grabberHands);
        transform.localPosition = Vector3.zero; // Устанавливаем начальное положение
        transform.localRotation = Quaternion.Euler(0, 150 /*180*/, 0);
    }
    [Server]
    public void GrabNpc(Transform grabTransform)
    {
        if (!_isGrabbed)
        {
            if (_rigidbody)
            {
                _rigidbody.isKinematic = true;
                _boxCollider.enabled = false;
            }
            RpcGrabNpc(grabTransform);
        }
    }
    [ClientRpc]
    void RpcGrabNpc(Transform grabTransform)
    {
        SetGrabbed(grabTransform);
    }   

    public void Drop()
    {
        CmdDrop();
    }

    [Command (requiresAuthority = false)]
    private void CmdDrop()
    {
        if (_isGrabbed)
        {
            if (_rigidbody)
            {
                _rigidbody.isKinematic = false;
                _boxCollider.enabled = true;
            }
            RpcDrop();
        }
    } 

    [ClientRpc]
    void RpcDrop()
    {
        _isGrabbed = false;
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
    public bool IsActive() => !IsGrabbed;
    public string GetInteractText() => "Grab";
    public void Hover() => _outline.EnableOutline();
    public void Unhover() => _outline.DisableOutline();
}
