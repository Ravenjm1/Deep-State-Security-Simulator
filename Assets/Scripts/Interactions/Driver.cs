using System.Collections;
using Mirror;
using UnityEngine;

public class Driver : NetworkBehaviour, IInteractable
{
    private Car car;
    [SyncVar] private bool gave;
    [SyncVar (hook=nameof(OnIdCardSet))] GrabObject idCard;
    [SerializeField] private Transform _hand;
    private OutlineInteract _outline;

    void Awake()
    {
        car = GetComponentInParent<Car>();
        _outline = gameObject.AddComponent<OutlineInteract>();
    }
    [Server]
    void Start()
    {
        idCard = car.SpawnIdCard().GetComponent<GrabObject>();
    }

    void OnIdCardSet(GrabObject oldVar, GrabObject newidCard)
    {
        newidCard.GrabNpc(_hand);
    }

    public bool IsActive() => (!gave || CheckItem(NetworkClient.localPlayer))? true : false;
    public string GetInteractText() => !gave? "Take Id card" : "Give back";
    public void Interact()
    {
        if (!gave)
        {
            CmdGiveIdCard(NetworkClient.localPlayer);
        }
        else
        {
            var playerInventory = NetworkClient.localPlayer.GetComponentInChildren<PlayerInventory>();
            if (CheckItem(NetworkClient.localPlayer))
            {
                playerInventory.DropItem(0);
                CmdGrabNpc();
            }
        }
    }

    [Command (requiresAuthority = false)]
    void CmdGiveIdCard(NetworkIdentity player)
    {
        gave = true;
        StartCoroutine(DropAndGrab(player));
    }

    IEnumerator DropAndGrab(NetworkIdentity player)
    {
        idCard.Drop();
        while (idCard.IsGrabbed)
            yield return null;
        idCard.CmdGrab(player);
    }

    [Command (requiresAuthority = false)]
    void CmdGrabNpc()
    {
        gave = false;
        StartCoroutine(WaitToGrabNpc());
    }
    IEnumerator WaitToGrabNpc()
    {
        while (idCard.IsGrabbed)
            yield return null;
        idCard.GrabNpc(_hand);
    }

    bool CheckItem(NetworkIdentity player)
    {
        var playerInventory = player.GetComponentInChildren<PlayerInventory>();
        var itemSlot = playerInventory.GetItem();

        return itemSlot != null && itemSlot == idCard;
    }
    
    public void Hover() => _outline.EnableOutline();
    public void Unhover() => _outline.DisableOutline();
}
