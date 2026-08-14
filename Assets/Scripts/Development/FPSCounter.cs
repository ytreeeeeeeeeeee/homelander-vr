using TMPro;
using UnityEngine;

namespace Development
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] public TMP_Text text;

        private int _fps;

        private void Update()
        {
            _fps = (int) (1f / Time.unscaledDeltaTime);
            text.text = _fps.ToString();
        }
    }
}
