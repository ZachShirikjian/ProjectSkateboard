using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ControlsScreenController : MonoBehaviour
{
    [SerializeField, Tooltip("The list of control scheme diagrams.")] private GameObject[] controlSchemes;

    private PlayerControls playerControls;

    private int currentControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.UI.Tab.performed += _ => CycleControls();
    }

    private void OnEnable()
    {
        playerControls?.Enable();

        currentControls = 0;
        ShowControlScheme(currentControls);
    }

    private void OnDisable()
    {
        playerControls?.Disable();
    }

    /// <summary>
    /// Cycle through the control schemes and show the current control scheme.
    /// </summary>
    private void CycleControls()
    {
        int direction = Mathf.RoundToInt(playerControls.UI.Tab.ReadValue<float>());

        currentControls += direction;

        if (currentControls < 0)
            currentControls = controlSchemes.Length - 1;
        
        else if (currentControls > controlSchemes.Length - 1)
            currentControls = 0;

        ShowControlScheme(currentControls);
    }

    /// <summary>
    /// Shows the control scheme highlighted and hides any other control schemes active.
    /// </summary>
    /// <param name="currentControlScheme">The control scheme to show.</param>
    private void ShowControlScheme(int currentControlScheme)
    {
        foreach(var scheme in controlSchemes)
        {
            if (scheme.activeInHierarchy && scheme != controlSchemes[currentControlScheme])
                scheme.SetActive(false);
            else if (!scheme.activeInHierarchy && scheme == controlSchemes[currentControlScheme])
                scheme.SetActive(true);
        }
    }
}
