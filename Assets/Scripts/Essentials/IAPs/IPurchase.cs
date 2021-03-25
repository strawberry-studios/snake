using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This interface provides methods for showing error messages related to IAPs. These methods must be implemented by a script
/// in the scene.
/// </summary>
public interface IPurchase
{
    /// <summary>
    /// Shows an error on a panel.
    /// </summary>
    /// <param name="error">The error as string.</param>
    void ShowErrorMessageOnPanel(string error);
}
