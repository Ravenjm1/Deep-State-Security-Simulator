using UnityEngine;

public class ModularWall : MonoBehaviour
{
    private bool _isOpen = true;
    private Vector3 startPos;
    private Vector3 upPos;

    void Awake() 
    {
        startPos = transform.position;
        upPos = new Vector3(startPos.x, startPos.y + 2.6f, startPos.z);
    }
    void Update()
    {
        if (!_isOpen)
        {
            transform.position = transform.position + (upPos - transform.position) / 10;
        }
        else 
        {
            transform.position = transform.position + (startPos - transform.position) / 10;
        }
    }

    public void Open()
    {
        _isOpen = true;
    }

    public void Close()
    {
        _isOpen = false;
    }
}
