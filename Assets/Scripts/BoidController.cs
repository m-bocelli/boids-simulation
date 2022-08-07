using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    public Boid boidPrefab; // Animal to represent a boid
    public int boidAmount = 5; // Amount of boids (cannot be changed while simulating)
    public float centeringFactor = 0.005f; // Speed at which boids approach the center of all boids
    public float repulsionFactor = 0.05f; // Speed at which boids turn away from each other
    public float matchNeighborFactor = 0.05f; // Percentage of neighbors' velocity that will be added to boid
    public int boidSpacing = 5; // Units between boids allowed before avoiding
    public int boidViewRange = 10; // How far each boid can see

    Boid[] boids; // Array of boids
    delegate Vector3 AverageDelegate(Boid boid); // Delegate used to averages of vector attributes of the boids
    Animator animator;
    int isPerchedHash;

    // Initialize array of boids
    void Start()
    {
        boids = new Boid[boidAmount];

        for (int i = 0; i < boids.Length; i++)
        {
            boids[i] = Instantiate<Boid>(boidPrefab);
            boids[i].perchingTimer = Random.Range(1f, 5f);
        }

        isPerchedHash = Animator.StringToHash("isPerched");

        InitBoidPositions();
    }

    void HandleBoidAnimations(Boid boid)
    {
        animator = boid.GetComponent<Animator>();
        if(boid.isPerching){
            animator.SetBool(isPerchedHash, true);
        } else {
            animator.SetBool(isPerchedHash, false);
        }
    }

    // Set the boids to a random position within the bounds of the plane
    void InitBoidPositions()
    {
        for (int i = 0; i < boids.Length; i++) {
            boids[i].transform.position = new Vector3 (Random.Range(-9f,9f), Random.Range(0.2f, 7f), Random.Range(-9f, 9f));
            boids[i].velocity = new Vector3 (Random.Range(1f, 3f), Random.Range(1f, 3f), Random.Range(1f, 3f));
        }
    }

    // Move boids each frame based on three rules
    void Update()
    {
        Vector3 offset1, offset2, offset3, offset4;

        for (int i = 0; i < boids.Length; i++) {
            HandleBoidAnimations(boids[i]);
            if (boids[i].isPerching) {
                // If boid is perching start timer and do not apply velocities
                if(boids[i].perchingTimer > 0){
                    boids[i].perchingTimer -= Time.deltaTime;
                    continue;
                } else {
                    // Once timer has ran out, give new timer length and rejoin flock
                    boids[i].isPerching = false;
                    boids[i].perchingTimer = Random.Range(1f, 5f);
                }
            }
            // Gather velocity offsets from rule methods below Update()
            offset1 = CenterOffset(boids[i]);
            offset2 = AvoidOtherBoids(boids[i]);
            offset3 = AlignVelocity(boids[i]);
            offset4 = KeepInBounds(boids[i]);
            // Apply offsets to the boid's current velocity
            boids[i].velocity += offset1 + offset2 + offset3 + offset4;
            // Limit speed
            LimitBoidVelocity(boids[i]);
            // Move boid based on its velocity
            boids[i].transform.position += boids[i].velocity * Time.deltaTime;
            // Rotate boid smoothly (through interpolation) towards its movement dirtection
            boids[i].transform.rotation = Quaternion.Slerp(boids[i].transform.rotation, Quaternion.LookRotation(boids[i].velocity.normalized), Time.deltaTime * 5f);
        }
        
    }

    // Boids fly towards the center position of other boids
    Vector3 CenterOffset(Boid boid)
    {
        // Average of the positions of all other boids is calculated
        Vector3 centerOfBoids = CalculateAverageOfVectors(boid, boid => boid.transform.position);
        // Boids move a fraction of the way towards the center each frame
        return (centerOfBoids - boid.transform.position) * centeringFactor;
    }

    // Boids move away from other boids when too close (distance adjusted by boidSpacing var)
    Vector3 AvoidOtherBoids(Boid boid)
    {
        // Vector to be applied to boids velocity indicating how to avoid the closest boid
        Vector3 repulsion = Vector3.zero;

        for (int i = 0; i < boids.Length; i++) {
            if (boids[i] != boid) {
                // If the distance between the boid and other is less than the given spacing, then
                // the boid's position is offset in the reverse direction.
                if (Vector3.Distance(boids[i].transform.position, boid.transform.position) < boidSpacing) {
                    repulsion += boid.transform.position - boids[i].transform.position;
                }
            }
        }
        return repulsion * repulsionFactor;
    }

    // Boid's velocity is slightly adjusted in order to align it with its neighbors
    Vector3 AlignVelocity(Boid boid)
    {
        // Average of velocity vectors of all other boids is calculated
        Vector3 averageVelocity = CalculateAverageOfVectors(boid, boid => boid.velocity);
        return (averageVelocity - boid.velocity) * matchNeighborFactor;
    }

    // Auxillary function for the calculation of the average center of the flock
    // for rule 1 and average velocities for rule 3.
    Vector3 CalculateAverageOfVectors(Boid boid, AverageDelegate vectorValue)
    {
        Vector3 average = Vector3.zero;

        foreach (Boid b in boids) {
            // Current boid's vector is excluded from the operation so each boid
            // has its own perspective of the flock.
            if (b.transform.position != boid.transform.position && !b.isPerching) {
                average += vectorValue(b);
            }
        }
        average /= (boids.Length -1);
        return average;
    }

    // Boid's speed is limited to better simulate real animal speeds
    void LimitBoidVelocity(Boid boid) 
    {
        int speedLimit = 13;
        // Magnitude of a boid's velocity is speed
        if (boid.velocity.magnitude > speedLimit) {
            boid.velocity = boid.velocity.normalized * speedLimit;
        }
    }

    // Boid flock is limited to arbitrary boundaries in order to better spectate
    Vector3 KeepInBounds(Boid boid)
    {
        int xMin = -10, xMax = 10, yMin = 1, yMax = 10, zMin = -10, zMax = 10;
        float groundLevel = 0.5f;
        int turnFactor = 2;
        Vector3 velocityOffset = Vector3.zero;
        Vector3 perchPosition = boid.transform.position;


        if (boid.transform.position.x < xMin) {
            velocityOffset.x = turnFactor;
        } else if (boid.transform.position.x > xMax) {
            velocityOffset.x = -turnFactor;
        } else if (boid.transform.position.y < yMin) {
            velocityOffset.y = turnFactor;
        } else if (boid.transform.position.y > yMax) {
            velocityOffset.y = -turnFactor;
        } else if (boid.transform.position.z < zMin) {
            velocityOffset.z = turnFactor;
        } else if (boid.transform.position.z > zMax) {
            velocityOffset.z = -turnFactor;
        }
        // Once a boid reaches the ground it will begin to perch by locking its transform at ground level
        if (boid.transform.position.y < groundLevel) {
            perchPosition.y = groundLevel;
            boid.transform.position = perchPosition;
            boid.isPerching = true;
        }
        // Offset by which the boid's velocity will be redirected when running into a boundary
        return velocityOffset;
    }

}
