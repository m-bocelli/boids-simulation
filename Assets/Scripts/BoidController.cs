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

        for (int i = 0; i < boids.Length; i++) {
            // Value should be slightly different for each boid
            print(CalculateCenterOfBoids(boids[i]));
        }
    }

    // Move boids each frame based on three rules
    void Update()
    {
        // for each boid
        //  store results of three rule methods in three vectors
        //  apply vector results to velocity of current boid
        //  apply movement each time step based on updated velocity
        
    }

    // Set the boids on the edges of the scene
    void InitBoidPositions()
    {
        int xPos = -9;
        for (int i = 0; i < boids.Length; i++) {
            if (i % 2 == 0) {
                xPos = 9;
            } else {
                xPos = -9;
            }
            boids[i].transform.position = new Vector3 (xPos, Random.Range(0.2f, 7f), Random.Range(-9f, 9f));
        }
    }

    // Calculates the center of the flock of boids, ignoring the passed
    // argument boid's position.
    Vector3 CalculateCenterOfBoids(Boid boid)
    {
        Vector3 totalPositionOfBoids = Vector3.zero;

        for(int i = 0; i < boids.Length; i++)
        {   
            // Only consider the position of boids which are not the one passed
            if (boids[i].transform.position != boid.transform.position) {
                totalPositionOfBoids += boids[i].transform.position;
            }
        }

        Vector3 centerOfBoids = totalPositionOfBoids / (boids.Length - 1);
        return centerOfBoids;
    }

}
