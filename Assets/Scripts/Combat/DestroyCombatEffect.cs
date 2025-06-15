using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCombatEffect : MonoBehaviour
{
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
