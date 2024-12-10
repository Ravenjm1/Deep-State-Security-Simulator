using System;
using System.Collections;
using System.Linq.Expressions;
using Mirror;
using UnityEngine;

public class PlayerInteractor : NetworkBehaviour
{
    [SerializeField] private float _distance;
    [SerializeField] private LayerMask _interact_layers;
    [SerializeField] private LayerMask _click_layers;
    private IInteractable _interactionObject;
    private OutlineInteract _outlineObject;
    private IClickable _clickableObject;
    private PlayerInventory playerInventory;
    private Camera _mainCamera;
    [field: SerializeField] public bool IsLocked { private set; get; }
    [NonSerialized] public IInteractable grabbedObject;
    
    private void Start() 
    {
        _mainCamera = Camera.main;
        playerInventory = GetComponent<PlayerInventory>();
    }

    private void OnEnable() => StartCoroutine(InputSubscribe());
    private void OnDisable() {
        InputManager.Instance.OnInteraction -= Interact;
        InputManager.Instance.OnClick -= Click;
        InputManager.Instance.OnClickCanceled -= Released;
    }

    private IEnumerator InputSubscribe()
    {
        yield return new WaitForSeconds(1f);
        if (isLocalPlayer) {
            InputManager.Instance.OnInteraction += Interact;
            InputManager.Instance.OnClick += Click;
            InputManager.Instance.OnClickCanceled += Released;
        }
    }

    private void FixedUpdate()
    {
        if (IsLocked && grabbedObject != null) {
            return;
        }
        else {
            IsLocked = false;
        }
        
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        // Отображаем луч длиной 100 единиц, зеленым цветом
        if (Physics.Raycast(ray, out RaycastHit hit, _distance, _interact_layers))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                if (interactable.IsActive())
                {
                    SetInteractable(interactable);
                }
                else
                {
                    ResetInteractable();
                }
            }
            else
            {
                ResetInteractable();
            }
        }
        else
        {
            ResetInteractable();
        }

        if (Physics.Raycast(ray, out RaycastHit _hit, _distance, _click_layers))
        {
            IClickable clickable = _hit.collider.GetComponentInParent<IClickable>();
            if (clickable != null)
            {
                if (clickable.IsActive())
                    SetClickable(clickable);
                else
                    ResetClickable();
            }
            else
            {
                ResetClickable();
            }
        }
        else
        {
            ResetClickable();
        }
    }

    void Update()
    {
        if (InputManager.Instance.ClickHolding())
            Pressed();
    }

    public void LockInteractions(IInteractable _obj) {
        IsLocked = true;
        grabbedObject = _obj;
    }
    public bool IsInteractionAvailable() => _interactionObject != null;

    public IInteractable GetInteractionObject() => _interactionObject;

    private void Interact()
    {
        if (_interactionObject != null)
        {
            if (_interactionObject.Type() == InteractionType.GRAB)
            {
                if (playerInventory.GetFreeSlot() != -1)
                {
                    _interactionObject.Interact();
                }
            }
            else
            {
                _interactionObject.Interact();
            }
        }
    }

    private void SetInteractable(IInteractable _object)
    {
        if (_interactionObject == null) {
            _interactionObject = _object;
            _interactionObject.Hover();
        }
    }

    private void ResetInteractable()
    {
        if (_interactionObject != null) {
            _interactionObject.Unhover();
            _interactionObject = null;
        }
    }

    public bool IsCkickAvailable() => _clickableObject != null;
    
    private void Click()
    {
        if (_clickableObject != null) {
            _clickableObject.Click();
        }
    }

    private void Pressed()
    {
        if (_clickableObject != null) {
            _clickableObject.Pressed();
        }
    }

    private void Released()
    {
        if (_clickableObject != null) {
            _clickableObject.Released();
        }
    }

    private void SetClickable(IClickable _object)
    {
        if (_clickableObject == null) {
            _clickableObject = _object;
            _clickableObject.Hover();
        }
    }

    private void ResetClickable()
    {
        if (_clickableObject != null) {
            _clickableObject.Unhover();
            _clickableObject = null;
        }
    }
}
