using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    BoidController boidController = FindObjectOfType<BoidController>();

    public void SetCenteringFactor (float newCenteringFactor)
    {
        boidController.centeringFactor = newCenteringFactor;
    }
}
