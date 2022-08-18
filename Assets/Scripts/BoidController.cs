using UnityEngine;

public class BoidController : MonoBehaviour
{
    [Header("Init Vars")]
    public Boid boidPrefab; // Animal to represent a boid
    public int boidAmount = 5; // Amount of boids (cannot be changed while simulating)

    [Header("Behavior Vars")]
    public float centeringFactor = 0.005f; // Speed at which boids approach the center of all boids
    public float repulsionFactor = 0.05f; // Speed at which boids turn away from each other
    public float matchingFactor = 0.05f; // Percentage of neighbors' velocity that will be added to boid
    public int boidSpacing = 5; // Units between boids allowed before avoiding
    //public int boidViewRange = 10; // How far each boid can see

    [Header("Flight Boundaries")]
    public Vector3 maxBoundaries;
    public Vector3 minBoundaries;

    Boid[] boids; // Array of boids
    delegate Vector3 AverageDelegate(Boid boid); // Delegate used to get averages of vector properties of the boids

    // Animation vars
    Animator animator;
    int isPerchedHash;
    int cycleOffsetHash;

    void Start()
    {
        boids = new Boid[boidAmount];
        // Initialize array of boids
        for (int i = 0; i < boids.Length; i++)
        {
            boids[i] = Instantiate<Boid>(boidPrefab);
            boids[i].perchingTimer = Random.Range(1, 4);
        }
        // Initialize animator vars
        isPerchedHash = Animator.StringToHash("isPerched");
        cycleOffsetHash = Animator.StringToHash("cycleOffset");

        InitBoidPositions();
    }

    void HandleBoidAnimations(Boid boid)
    {
        animator = boid.GetComponent<Animator>();

        //animator.SetFloat(cycleOffsetHash, Random.Range(0f, 1f));

        if(boid.isPerching){
            animator.SetBool(isPerchedHash, true);
        } else {
            animator.SetBool(isPerchedHash, false);
        }
    }

    // Set the boids to a random position within the bounds of the plane
    void InitBoidPositions()
    {
        foreach (Boid boid in boids) {
            boid.transform.position = new Vector3 (Random.Range(minBoundaries.x + 1f, maxBoundaries.x - 1f), 
                                                        Random.Range(minBoundaries.y + 1f, maxBoundaries.y -1f), 
                                                        Random.Range(minBoundaries.z + 1f, maxBoundaries.z - 1f));
            boid.velocity = new Vector3 (Random.Range(2f, 5f), Random.Range(2f, 5f), Random.Range(2f, 5f));
        }
    }

    void HandleFlockAttributes()
    {
        centeringFactor = Mathf.Clamp(centeringFactor, 0f, 0.01f);
        repulsionFactor = Mathf.Clamp(repulsionFactor, 0.05f, 0.1f);
        matchingFactor = Mathf.Clamp(matchingFactor, 0f, 0.05f);
    }

    // Move boids each frame based on rules
    void Update()
    {
        Vector3 offset1, offset2, offset3, offset4;
        HandleFlockAttributes();
        
        foreach (Boid boid in boids) {
            HandleBoidAnimations(boid);

            if (boid.isPerching) {
                // If boid is perching start timer and do not apply velocities
                if(boid.perchingTimer > 0){
                    boid.perchingTimer -= Time.deltaTime;
                    continue;
                } else {
                    // Once timer has ran out, give new timer length and rejoin flock
                    boid.isPerching = false;
                    boid.perchingTimer = Random.Range(1, 4);
                }
            }
            // Gather velocity offsets from rule methods below Update()
            offset1 = FindFlockCenter(boid);
            offset2 = AvoidOtherBoids(boid);
            offset3 = AlignVelocity(boid);
            offset4 = KeepInBounds(boid);
            // Apply offsets to the boid's current velocity
            boid.velocity += (offset1 + offset2 + offset3 + offset4);
            // Limit speed
            LimitBoidVelocity(boid);
            // Move boid based on its velocity
            boid.transform.position += boid.velocity * Time.deltaTime;
            // Rotate boid smoothly (through interpolation) towards its movement dirtection
            boid.transform.rotation = Quaternion.Slerp(boid.transform.rotation, Quaternion.LookRotation(boid.velocity.normalized), Time.deltaTime * 5f);
        }
        
    }

    // Boids fly towards the center position of other boids
    Vector3 FindFlockCenter(Boid boid)
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

        foreach (Boid otherBoid in boids) {
            if (otherBoid != boid) {
                // If the distance between the boid and other is less than the given spacing, then
                // the boid's position is offset in the reverse direction.
                if (Vector3.Distance(otherBoid.transform.position, boid.transform.position) < boidSpacing) {
                    repulsion += boid.transform.position - otherBoid.transform.position;
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
        return (averageVelocity - boid.velocity) * matchingFactor;
    }

    // Auxillary function for the calculation of the average center of the flock
    // for rule 1 and average velocities for rule 3.
    Vector3 CalculateAverageOfVectors(Boid boid, AverageDelegate vectorValue)
    {
        Vector3 average = Vector3.zero;
        int numOtherBoids = 0;

        foreach (Boid b in boids) {
            // Current boid's vector is excluded from the operation so each boid
            // has its own perspective of the flock.
            if (b != boid && !b.isPerching) {
                average += vectorValue(b);
                numOtherBoids++;
            }
        }

        if (numOtherBoids != 0) {
            average /= (numOtherBoids);
        }

        return average;
    }

    // Boid's speed is limited to better simulate real animal speeds
    void LimitBoidVelocity(Boid boid) 
    {
        int speedLimit = 10;
        // Magnitude of a boid's velocity is speed
        if (boid.velocity.magnitude > speedLimit) {
            boid.velocity = boid.velocity.normalized * speedLimit;
        }
    }

    // Boid flock is limited to arbitrary boundaries in order to better spectate
    Vector3 KeepInBounds(Boid boid)
    {
        float groundLevel = minBoundaries.y - 0.05f;
        int turnFactor = 1;
        Vector3 velocityOffset = Vector3.zero;
        Vector3 perchPosition = boid.transform.position;

        if (boid.transform.position.x < minBoundaries.x) {
            velocityOffset.x = turnFactor;
        } else if (boid.transform.position.x > maxBoundaries.x) {
            velocityOffset.x = -turnFactor;
        } else if (boid.transform.position.y < minBoundaries.y) {
            velocityOffset.y = turnFactor;
        } else if (boid.transform.position.y > maxBoundaries.y) {
            velocityOffset.y = -turnFactor;
        } else if (boid.transform.position.z < minBoundaries.z) {
            velocityOffset.z = turnFactor;
        } else if (boid.transform.position.z > maxBoundaries.z) {
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

    // Methods for changing vars via UI
    public void SetCenteringFactor(float centeringFactor) 
    {
        this.centeringFactor = centeringFactor;
    }

    public void SetRepulsionFactor(float repulsionFactor)
    {
        this.repulsionFactor = repulsionFactor;
    }

    public void SetMatchingFactor(float matchingFactor)
    {
        this.matchingFactor = matchingFactor;
    }
}
