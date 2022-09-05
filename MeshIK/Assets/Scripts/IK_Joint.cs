using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the glue of the IK algorithm.  This would be a wrist, an elbow, shoulder, etc
/// </summary>
[Serializable]
public class IK_Joint : MonoBehaviour
{
    /// <summary>
    /// This needs to be unique within the mesh
    /// </summary>
    /// <remarks>
    /// It can be any string as long as it's unique (an incrementing number, guid, friendly name, etc)
    /// </remarks>
    public string ID;

    /// <summary>
    /// This is a link to a gameobject that contains an IK_Target script that this joint could be tied to
    /// </summary>
    /// <remarks>
    /// Only leaf joints would be tied to targets.  Interior joints are moved based on their links to other
    /// joints
    /// 
    /// Not sure about the best type to use.  It looks like json serialization can't handle null, so string
    /// for now (leave it as empty string if there is no target)
    /// </remarks>
    public string TargetID;

    /// <summary>
    /// True: The IK algorithm can't move this joint
    /// False: The IK algorithm can freely move this joint
    /// </summary>
    /// <remarks>
    /// It's not required that any joints be anchored
    /// </remarks>
    public bool IsAnchored;

    /// <remarks>
    /// Some joints are good mid skeleton junctions (pelvis, shouders).  When chaining joints together, the
    /// chain should stop when it encounters one of these center anchors.  There should also be chains that
    /// start from the center anchors
    /// 
    /// This should help optimize the number of chains that get generated
    /// 
    /// This could probably be calculated at runtime
    /// </remarks>
    //TODO: Come up with a better name
    private bool IsBottleneck;

    public bool IsMatch(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(ID))
            return false;

        return id.Equals(ID, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return string.Format("{0} | {1}", ID, IsAnchored ? "anchored" : string.IsNullOrWhiteSpace(TargetID) ? "" : $"target={TargetID}");
    }
}
