using UnityEngine;

namespace Game.Homelander
{
    public class Homelander : MonoBehaviour
    {
        [SerializeField] private Head head;

        private void Update()
        {
            if (SkyworthVrInput.GetButton(SkyworthVrButton.Confirm))
            {
               Shoot();
            }
            else
            {
                head.StopShooting();
            }
        }

        private void Shoot()
        {
             head.ShootOutOfEyes();
        }
    }
}
