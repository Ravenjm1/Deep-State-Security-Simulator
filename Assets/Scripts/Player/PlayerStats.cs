using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    public float MaxHp {get; private set; } = 10f;
    public float Hp {get; private set; }
    private PlayerMovement playerMovement;
    [SyncVar (hook=nameof(OnDeadChange))] public bool IsDead;

    public event Action OnDead = delegate {  };
    public event Action OnResurect = delegate { };

    private float respawnTime = 3;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        Hp = MaxHp;
    }

    public void Update()
    {
        if (!isLocalPlayer)
            return;
        if (Input.GetKeyDown(KeyCode.K))
        {
            GetDamage(5);
        }
    }

    public void GetDamage(float damage)
    {
        Hp = Math.Max(Hp - damage, 0);

        if (Hp == 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (!IsDead) 
        {
            IsDead = true;
            StartCoroutine(Respawn());
        }
    }
    // For all clients
    void OnDeadChange(bool oldDead, bool newDead)
    {
        if (newDead) 
        {
            OnDead();
        }
        else
        {
            OnResurect();
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);

        Resurect();
    }

    void Resurect()
    {
        if (IsDead) 
        {
            Hp = MaxHp;
            IsDead = false;
        }
    }
}
