using UnityEngine;

namespace Game.Homelander
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovementController : MonoBehaviour
    {
        [SerializeField] private Head head;
        [SerializeField] private float speed = 5f;

        private Rigidbody _rb;

        private void Awake()
        { 
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 right = head.transform.right.normalized;
            Vector3 forward = head.transform.forward.normalized;
            
            Vector3 move =  right * x + forward * z;
            move = Vector3.ClampMagnitude(move, 1f);
            
            _rb.velocity = move * speed;
        }
    }
}
