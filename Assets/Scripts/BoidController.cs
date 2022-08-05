using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    public GameObject boidGroup;
    public int boidAmount;
    GameObject[] boids;

    // Initialize array of boids
    void Start()
    {
        boids = new GameObject[boidGroup.transform.childCount];
        for (int i = 0; i < boids.Length; i++)
        {
            boids[i] = boidGroup.transform.GetChild(i).gameObject;
        }

        InitBoidPositions();
    }

    // Set the boids on the edges of the scene
    void InitBoidPositions()
    {
        int xPos = -9;
        for (int i = 0; i < boids.Length; i++)
        {
            if (i % 2 == 0)
            {
                xPos = 9;
            } else {
                xPos = -9;
            }

            boids[i].transform.position = new Vector3 (xPos, Random.Range(0.2f, 7f), Random.Range(-9f, 9f));
        }
    }
}
