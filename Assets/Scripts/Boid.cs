using UnityEngine;

// For any gameObject which is intended to behave as a Boid, it must utilize this class
public class Boid : MonoBehaviour
{
    // Properties of a boid which are not native to Unity's GameObject
    public Vector3 velocity { get; set; }
    public bool isPerching { get; set; }
    public float perchingTimer { get; set; }
}
