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
               head.ShootOutOfEyes();
            }
            else
            {
                head.StopShooting();
            }
        }
    }
}
