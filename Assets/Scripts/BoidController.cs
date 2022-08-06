using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    public int boidAmount = 5;
    public Boid animalPrefab;
    Boid[] boids;

    // Initialize array of boids
    void Start()
    {
        boids = new Boid[boidAmount];

        for (int i = 0; i < boids.Length; i++)
        {
           boids[i] = Instantiate<Boid>(animalPrefab);
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
