using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class Baggage : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnColorChanged))]
    private Color itemColor;
    [SyncVar(hook = nameof(OnPositionChanged))]
    private Vector3 setPositon;
    [SyncVar] public float radiation;
    GrabObject grabObject;

    void Start()
    {
        grabObject = GetComponent<GrabObject>();
    }
    public override void OnStartClient()
    {
        // Когда объект появляется на клиенте, сразу применяем цвет
        GetComponent<Renderer>().material.color = itemColor;
        transform.SetParent(FindFirstObjectByType<Car>().transform);
    }
    
    void FixedUpdate()
    {
        if (grabObject != null)
        {
            if (grabObject.IsGrabbed && !grabObject.IsGrabberPlayer() && radiation > 0)
            {
                var player = LocationContext.GetDependency.Player;
                if (Vector3.Distance(player.transform.position, transform.position) < 1) 
                {
                    player.GetComponent<PlayerStats>().GetDamage(0.1f);
                }
            }
        }
    }

    // Этот метод вызывается автоматически при изменении SyncVar
    void OnColorChanged(Color oldColor, Color newColor)
    {
        GetComponent<Renderer>().material.color = newColor;
    }

    void OnPositionChanged(Vector3 oldPos, Vector3 newPos)
    {
        transform.localPosition = newPos;
    }

    [Server]
    public void SetColor(Color color)
    {
        itemColor = color;
    }
    [Server]
    public void SetPositon(Vector3 pos)
    {
        setPositon = pos;
    }
}
