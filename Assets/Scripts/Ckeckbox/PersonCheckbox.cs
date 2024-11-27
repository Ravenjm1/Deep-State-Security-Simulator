using System;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PersonCheckbox : NetworkBehaviour
{
    public enum CheckboxState { Default, Decline, Approved }

    [SyncVar (hook = nameof(OnCurrentStateChanged))] 
    public CheckboxState CurrentState = CheckboxState.Default;
    private Toggle toggle;
    private Image backgroundImage;
    private Text labelText;
    private Color defaultColor = Color.white;
    private Color approvedColor = Color.green;
    private Color cancelColor = Color.red;
    private CheckboxListManager checkboxListManager;

    public delegate void CheckboxStateChanged(CheckboxState newState);
    public event CheckboxStateChanged OnStateChanged;
    [SyncVar] public Person.LieType type;
    
    [SyncVar (hook = nameof(OnLabelChange))]
    public string label;
    private GameObject checkboxUI;

    private void Awake()
    {
        checkboxListManager = FindFirstObjectByType<CheckboxListManager>(); 
        toggle = GetComponent<Toggle>();
        backgroundImage = GetComponentInChildren<Image>();
        labelText = GetComponentInChildren<Text>();

        transform.SetParent(checkboxListManager.checkboxContainer.transform);
        checkboxUI = checkboxListManager.AddCheckboxUI();
    }

    void Start()
    {
        UpdateVisualState();
    }

    void OnLabelChange(string oldLabel, string newLabel)
    {
        labelText.text = newLabel;
        
        checkboxUI.GetComponentInChildren<Text>().text = newLabel;
    }
    public void OnToggleClick()
    {
        // Переход между состояниями
        CmdToggleClick();
    }

    [Command (requiresAuthority = false)]
    private void CmdToggleClick()
    {
        // Переход между состояниями
        CurrentState = (CheckboxState)(((int)CurrentState + 1) % 2);
    }

    void OnCurrentStateChanged(CheckboxState oldState, CheckboxState newState)
    {
        // Переход между состояниями
        UpdateVisualState();
        // Вызов события
        OnStateChanged?.Invoke(CurrentState);
    }

    private void UpdateVisualState()
    {
        switch (CurrentState)
        {
            case CheckboxState.Default:
                backgroundImage.color = defaultColor;
                break;
            case CheckboxState.Approved:
                backgroundImage.color = approvedColor;
                break;
            case CheckboxState.Decline:
                backgroundImage.color = cancelColor;
                break;
        }
        label = CheckboxListManager.CheckboxText[(type, CurrentState)];
        if (checkboxUI != null)
            checkboxUI.GetComponentInChildren<Image>().color = backgroundImage.color;
    }

    void OnDestroy()
    {
        if (checkboxUI != null)
        {
            Destroy(checkboxUI);
        }
    }
}
