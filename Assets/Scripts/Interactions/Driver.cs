using Mirror;
using UnityEngine;

public class Driver : NetworkBehaviour, IInteractable
{
    private Car car;
    private bool gave;
    GameObject idCard;

    void Awake()
    {
        car = GetComponentInParent<Car>();
    }
    [Server]
    void Start()
    {
        idCard = car.SpawnIdCard();
    }

    public bool IsActive() => (!gave || CheckItem(NetworkClient.localPlayer))? true : false;
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
            idCard.GetComponent<GrabObject>().CmdGrab(player);
        }
        else
        {
            var playerInventory = player.GetComponentInChildren<PlayerInventory>();
            var itemSlot = playerInventory.GetItem();
            if (CheckItem(player))
            {
                playerInventory.DropItem(0);
                itemSlot.transform.SetParent(transform);
                itemSlot.transform.localPosition = new Vector3(0f, 0f, 0f);
                gave = false;
            }
        }
    }

    bool CheckItem(NetworkIdentity player)
    {
        var playerInventory = player.GetComponentInChildren<PlayerInventory>();
        var itemSlot = playerInventory.GetItem();

        return itemSlot != null && itemSlot.gameObject == idCard;
    }
    
}
