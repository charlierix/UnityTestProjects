using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This should be tied to a GameObject that will be a target for an IK_Joint.  The joint will then try to
/// keep itself at the same position as this target's position (pushing/pulling intermediate joints)
/// </summary>
[Serializable]
public class IK_Target : MonoBehaviour
{
    public string ID;

    public bool IsMatch(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(ID))
            return false;

        return id.Equals(ID, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return ID;
    }
}