using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Helpers for working with assemblies.
/// </summary>
public static class AssemblyUtility
{
    /// <summary>
    /// Try to find Assembly with a specified name in this application domain.
    /// </summary>
    public static Assembly FindAssemblyByName(string assemblyName)
    {
        Assembly assembly = null;
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var it in assemblies)
        {
            if (it.GetName().Name == assemblyName)
            {
                assembly = it;
                break;
            }
        }

        return assembly;
    }
}
