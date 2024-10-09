using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walking : MonoBehaviour
{
    public AudioSource walk;

void Step()
    {
        walk.Play();
    }
}
