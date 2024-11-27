using System;
using System.Collections;
using UnityEngine;

public class ExplodedCarDeleter : MonoBehaviour
{
    [SerializeField] private Transform _center;

    private float _removedOffset;
    private bool _isRemoved;

    private void Awake() => StartCoroutine(Remove());

    private void Update()
    {
        if (_isRemoved)
        {
            _removedOffset -= Time.deltaTime;
            _center.localPosition = new Vector3(0, 0, _removedOffset);
        }
    }

    private IEnumerator Remove()
    {
        yield return new WaitForSeconds(1f);
        _isRemoved = true;
        yield return new WaitForSeconds(6f);
        Destroy(gameObject);
    }
}
