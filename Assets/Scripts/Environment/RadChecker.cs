using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadChecker : NetworkBehaviour
{
    float maxDistance = 5f;  // Дистанция, на которую будет проверяться столкновение
    [SerializeField] private LayerMask layerMask;  // Опционально можно добавить маску слоёв для проверки только определённых объектов
    [SerializeField] private TMP_Text visualText;
    [SerializeField] private Slider loadBar;
    Collider m_Collider;

    Baggage baggage;
    bool checkRad = false;
    [SyncVar] float loadRad = 0f;
    [SyncVar] float resultRadiation = 0f;

    void Start()
    {
        m_Collider = GetComponentInChildren<Collider>();
    }

    void FixedUpdate()
    {
        CheckForObjectAbove();
    }
    void Update()
    {
        LoadRadiation();
        loadBar.value = loadRad;
        visualText.text = System.Math.Round(resultRadiation, 2).ToString();
    }

    [Server]
    void CheckForObjectAbove()
    {
        // Центр коробки будет исходить из позиции объекта (например, снизу объекта)
        Vector3 origin = m_Collider.bounds.center + new Vector3(0f, -1f, 0f);
        Vector3 direction = Vector3.up;  // Направление проверки (вверх)
        Vector3 boxHalfExtents = transform.localScale*0.5f;
        // Выполняем BoxCast
        if (Physics.BoxCast(origin, boxHalfExtents, direction, out RaycastHit hit, Quaternion.identity, maxDistance, layerMask))
        {
            baggage = hit.collider.GetComponent<Baggage>();
        }
        else
        {
            baggage = null;
        }
    }
    [Server]
    void LoadRadiation()
    {
        if (checkRad)
        {
            if (!baggage)
            {
                checkRad = false;
            }
            else {
                loadRad = Mathf.Clamp01(loadRad + Time.deltaTime / 3f);

                if (loadRad == 1f)
                {
                    resultRadiation = baggage.radiation;
                }
            }
        }
        else 
        {
            loadRad = 0f;
            if (!baggage)
            {
                resultRadiation = 0f;
            }
        }
    }

    public void CheckRadiation()
    {
        CmdCheckRadiation();
    }
    [Command (requiresAuthority = false)]
    void CmdCheckRadiation()
    {
        if (baggage)
        {
            checkRad = true;
        }
    }
}
