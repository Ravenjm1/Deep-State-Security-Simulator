using UnityEngine;

public class CarExplosion : MonoBehaviour
{
    [SerializeField] private GameObject _explosionPrefab;
    
    public void Explode()
    {
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
    }
}
