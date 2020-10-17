using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosLock : MonoBehaviour
{
    public GameObject Player;

    public void Update()
    {
        transform.position = Player.transform.position;
    }
}
