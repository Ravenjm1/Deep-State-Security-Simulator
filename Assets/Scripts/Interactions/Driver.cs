using System.Collections;
using Mirror;
using UnityEngine;

public class Driver : NetworkBehaviour, IInteractable
{
    private Car car;
    private bool gave;
    [SyncVar (hook=nameof(OnIdCardSet))] GrabObject idCard;
    [SerializeField] private Transform _hand;

    void Awake()
    {
        car = GetComponentInParent<Car>();
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
        CmdGiveIdCard(NetworkClient.localPlayer);
    }

    [Command (requiresAuthority = false)]
    void CmdGiveIdCard(NetworkIdentity player)
    {
        if (!gave)
        {
            gave = true;
            idCard.Drop();
            idCard.CmdGrab(player);
        }
        else
        {
            var playerInventory = player.GetComponentInChildren<PlayerInventory>();
            if (CheckItem(player))
            {
                playerInventory.DropItem(0);
                StartCoroutine(WaitToGrabNpc());
            }
        }
    }

    IEnumerator WaitToGrabNpc()
    {
        yield return new WaitForSeconds(0.1f);
        idCard.GrabNpc(_hand);
        gave = false;
    }

    bool CheckItem(NetworkIdentity player)
    {
        var playerInventory = player.GetComponentInChildren<PlayerInventory>();
        var itemSlot = playerInventory.GetItem();

        return itemSlot != null && itemSlot == idCard;
    }
    
}
