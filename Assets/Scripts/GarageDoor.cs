using Mirror;
using UnityEngine;

public class GarageDoor : NetworkBehaviour
{
    private Vector3 _startPosition;
    private Vector3 _openPosition;
    [SyncVar] private bool open = false;
    public float offset = 5f;

    void Awake()
    {
        _startPosition = transform.localPosition;
        _openPosition = _startPosition + new Vector3(0, offset, 0);
    }

    void Update()
    {
        if (open)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _openPosition, Time.deltaTime * 10f);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _startPosition, Time.deltaTime * 10f);
        }
    }
    public void Open()
    {
        CmdOpen();
    }

    [Command (requiresAuthority = false)]
    void CmdOpen()
    {
        open = true;
    }

    public void Close()
    {
        CmdClose();
    }

    [Command (requiresAuthority = false)]
    void CmdClose()
    {
        open = false;
    }
}
