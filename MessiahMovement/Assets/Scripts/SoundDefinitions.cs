using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a way for the editor to assign sound files to actions.  It is discoverable at runtime and
/// audio playing scripts can instantiate audio sources
/// </summary>
public class SoundDefinitions : MonoBehaviour
{
    public AudioClip[] FootStep;

    public AudioClip[] WingFlap;
}
