using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    // Инвентарь
    [NonSerialized] public GrabObject[] inventory = new GrabObject[1];
    [NonSerialized] public int currentSlot = 0; 

    private void OnEnable() => StartCoroutine(InputSubscribe());
    private void OnDisable() 
    {
        InputManager.Instance.OnSlot1 -= _Equip1;
        InputManager.Instance.OnSlot2 -= _Equip2;
        InputManager.Instance.OnDrop -= _Drop;
    } 

    private IEnumerator InputSubscribe()
    {
        yield return new WaitForSeconds(1f);
        if (isLocalPlayer) {
            InputManager.Instance.OnSlot1 += _Equip1;
            InputManager.Instance.OnSlot2 += _Equip2;
            InputManager.Instance.OnDrop += _Drop;
        }
    }
    private void _Equip1() => EquipItem(0);
    private void _Equip2() => EquipItem(1);
    private void _Drop() => DropItem(currentSlot);

    public int GetFreeSlot()
    {
        if (inventory[currentSlot] == null) 
            return currentSlot;
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    public void AddItemToInventory(GrabObject item)
    {
        // Проверяем, есть ли место в инвентаре
        int slotIndex = GetFreeSlot();

        if (slotIndex != -1)
        {
            inventory[slotIndex] = item;
            
            // Если нет текущего выбранного предмета, выбираем этот
            if (currentSlot == slotIndex)
            {
                EquipItem(slotIndex);
            }
            else 
            {
                item.SetInInventory(false); // Деактивируем предмет, если он не в текущем слоте
            }
        }
        else
        {
            Debug.Log("Инвентарь полон");
        }
    }

    public void EquipItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < inventory.Length)
        {
            if (currentSlot != slotIndex && inventory[currentSlot] != null)
            {
                // Скрываем предыдущий предмет
                inventory[currentSlot].SetInInventory(false);
            }
            if (inventory[slotIndex] != null)
            {
                // Активируем новый предмет
                inventory[slotIndex].SetInInventory(true);
            }
            currentSlot = slotIndex;
        }
    }

    public void DropItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < inventory.Length)
        {
            if (inventory[slotIndex] != null)
            {
                GrabObject item = inventory[slotIndex];

                item.SetInInventory(true);

                // Сообщаем предмету, что он сброшен
                item.Drop();

                // Удаляем предмет из инвентаря
                inventory[slotIndex] = null;
            }
        }
    }
}
