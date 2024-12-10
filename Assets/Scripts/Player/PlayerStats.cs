using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    public float MaxHp {get; private set; } = 10f;
    public float Hp {get; private set; }
    private PlayerMovement playerMovement;
    private PlayerController playerController;
    [SyncVar] public bool IsDead;

    public event Action OnDead = delegate {  };
    public event Action OnResurect = delegate { };

    private float respawnTime = 10;

    [SerializeField] private GameObject _corpsePrefab;
    private GameObject _corpseObj;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerController = GetComponent<PlayerController>();
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
        if (!isLocalPlayer)
            return;

        Hp = Mathf.Max(Hp - damage, 0);
        if (Hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (!IsDead)
        {
            bool oldDead = IsDead;
            IsDead = true;

            OnDead();

            AlertDie();

            StartCoroutine(Respawn());
        }
    }

    [Command]
    void AlertDie()
    {
        _corpseObj = Instantiate(_corpsePrefab, playerController.Model.transform.position, Quaternion.identity);
        NetworkServer.Spawn(_corpseObj);

        RpcSetVisibilityModel(false);
    }

    [Command]
    void AlertResurect()
    {
        RpcSetVisibilityModel(true);
    }

    [ClientRpc]
    void RpcSetVisibilityModel(bool isVisible)
    {
        if (!isLocalPlayer)
            playerController.Model.SetActive(isVisible); 
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
            OnResurect();

            AlertResurect();
        }
    }
}
