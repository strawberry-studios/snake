using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a singleton that doesn't implement any method, yet it has access to the functionality of a MonoBehaviour.
/// Hence it can be used for executing monoo-related tasks by those scripts that do not have these functionality.
/// </summary>
public class Helper : Singleton<Helper>
{
    
}
