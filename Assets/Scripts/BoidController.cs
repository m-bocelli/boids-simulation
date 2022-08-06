using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    public int boidAmount = 5;
    public Boid animalPrefab;
    public int boidSpacing = 5;
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

        // Debug
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

        Vector3 offset1;
        Vector3 offset2;

        for (int i = 0; i < boids.Length; i++) {
            offset1 = Rule1(boids[i]);
            offset2 = Rule2(boids[i]);

            boids[i].velocity += offset1 + offset2;
            boids[i].transform.position += boids[i].velocity * Time.deltaTime;
            // Rotate boid smoothly (through interpolation) towards its movement dirtection
            boids[i].transform.rotation = Quaternion.Slerp(boids[i].transform.rotation, Quaternion.LookRotation(boids[i].velocity.normalized), Time.deltaTime * 5f);
        }
        
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

    // Boids fly towards the center position of neighboring boids
    Vector3 Rule1(Boid boid)
    {
        Vector3 centerOfBoids = CalculateCenterOfBoids(boid);
        // Boids move 0.1% of the way towards the center each frame
        return (centerOfBoids - boid.transform.position) / 1000;
    }

    Vector3 Rule2(Boid boid)
    {
        Vector3 moveAwayDistance = Vector3.zero;

        for (int i = 0; i < boids.Length; i++) {
            if (boids[i] != boid) {
                // If the distance between the boid and other is less than the given spacing, then
                // the boid's position is offset by said distance in the reverse direction
                if (Mathf.Abs(Vector3.Distance(boids[i].transform.position, boid.transform.position)) < boidSpacing) {
                    moveAwayDistance -= boids[i].transform.position - boid.transform.position;
                }
            }
        }

        return moveAwayDistance;
    }

    // Calculates the center of the flock of boids, ignoring the passed
    // argument boid's position.
    Vector3 CalculateCenterOfBoids(Boid boid)
    {
        Vector3 totalPositionOfBoids = Vector3.zero;

        for(int i = 0; i < boids.Length; i++)
        {   
            // Only consider the position of boids which are not the one passed
            if (boids[i] != boid) {
                totalPositionOfBoids += boids[i].transform.position;
            }
        }

        Vector3 centerOfBoids = totalPositionOfBoids / (boids.Length - 1);
        return centerOfBoids;
    }

}
