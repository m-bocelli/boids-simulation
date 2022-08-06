using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    public Boid boidPrefab; // Animal to represent a boid
    public int boidAmount = 5; // Amount of boids (cannot be changed while simulating)
    public float centeringFactor = 0.0001f; // Speed at which boids rebound out of center
    public float repulsionFactor = 0.05f; // Speed at which boids turn away from each other (%)
    public float matchNeighborFactor = 0.05f; // Percentage of neighbors' velocity that will be added to boid
    public int boidSpacing = 5; // Units between boids allowed before avoiding
    
    Boid[] boids;

    // Initialize array of boids
    void Start()
    {
        boids = new Boid[boidAmount];

        for (int i = 0; i < boids.Length; i++)
        {
           boids[i] = Instantiate<Boid>(boidPrefab);
        }

        InitBoidPositions();
    }

    // Move boids each frame based on three rules
    void Update()
    {
        Vector3 offset1, offset2, offset3;

        for (int i = 0; i < boids.Length; i++) {
            offset1 = CohesionOffset(boids[i]);
            offset2 = AvoidOtherBoids(boids[i]);
            offset3 = AlignVelocity(boids[i]);

            boids[i].velocity += offset1 + offset2 + offset3;
            limitBoidVelocity(boids[i]);
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
    Vector3 CohesionOffset(Boid boid)
    {
        Vector3 centerOfBoids = CalculateCenterOfBoids(boid);
        // Boids move 0.1% of the way towards the center each frame
        return (centerOfBoids - boid.transform.position) * centeringFactor;
    }

    Vector3 AvoidOtherBoids(Boid boid)
    {
        Vector3 repulsion = Vector3.zero;

        for (int i = 0; i < boids.Length; i++) {
            if (boids[i] != boid) {
                // If the distance between the boid and other is less than the given spacing, then
                // the boid's position is offset by said distance in the reverse direction
                if (Vector3.Distance(boids[i].transform.position, boid.transform.position) < boidSpacing) {
                    repulsion -= (boids[i].transform.position - boid.transform.position);
                }
            }
        }

        return repulsion * repulsionFactor;
    }

    Vector3 AlignVelocity(Boid boid)
    {
        Vector3 averageVelocity = CalculateAverageVelocity(boid);

        return (averageVelocity - boid.velocity) * matchNeighborFactor;
    }

    Vector3 CalculateAverageVelocity(Boid boid)
    {
        Vector3 averageVelocity = Vector3.zero;

        for (int i = 0; i < boids.Length; i++) {
            if (boids[i] != boid) {
                averageVelocity += boids[i].velocity;
            }
        }
        averageVelocity /= (boids.Length -1);

        return averageVelocity;
    }

    // Calculates the center of the flock of boids, ignoring the passed
    // argument boid's position.
    Vector3 CalculateCenterOfBoids(Boid boid)
    {
        Vector3 centerOfBoids = Vector3.zero;

        for (int i = 0; i < boids.Length; i++) {   
            // Only consider the position of boids which are not the one passed
            if (boids[i] != boid) {
                centerOfBoids += boids[i].transform.position;
            }
        }
        centerOfBoids /= (boids.Length - 1);

        return centerOfBoids;
    }

    void limitBoidVelocity(Boid boid) 
    {
        int speedLimit = 10;
        // Magnitude of a boid's velocity is speed
        if (boid.velocity.magnitude > speedLimit) {
            boid.velocity = boid.velocity.normalized * speedLimit;
        }
    }

}
