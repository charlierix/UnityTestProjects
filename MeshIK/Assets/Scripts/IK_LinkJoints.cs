using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This ties two joints together
/// </summary>
/// <remarks>
/// The IK solutions that I looked at use parent/child relationships to know which joints are linked.  But
/// that limits what kinds of structures can be supported
/// </remarks>
[Serializable]
public class IK_LinkJoints
{
    public string JointID_1;
    public string JointID_2;

    /// <summary>
    /// A helper that tells if the ID matches either of this instance's IDs
    /// </summary>
    public bool Contains(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        return
            (!string.IsNullOrWhiteSpace(JointID_1) && id.Equals(JointID_1, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(JointID_2) && id.Equals(JointID_2, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// This returns the the ID passed in and Other ID in a more certain form
    /// WARNING: Only call this if Contains came back true, otherwise you'll get an exception
    /// </summary>
    public (string passedIn, string other) GetOtherLink(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID passed in must have a value");

        if (string.IsNullOrWhiteSpace(JointID_1) || string.IsNullOrWhiteSpace(JointID_2))
            throw new InvalidOperationException($"Both link IDs must have a value: \"{JointID_1 ?? ""}\", \"{JointID_2 ?? ""}\" ");

        if (id.Equals(JointID_1))
            return (JointID_1, JointID_2);
        else if (id.Equals(JointID_2))
            return (JointID_2, JointID_1);
        else
            throw new ArgumentException($"The ID passed in doesn't match either of the link IDs. Passed In: \"{id}\", 1: \"{JointID_1}\", 2: \"{JointID_2}\"");
    }

    public override string ToString()
    {
        return $"{JointID_1} | {JointID_2}";
    }
}
