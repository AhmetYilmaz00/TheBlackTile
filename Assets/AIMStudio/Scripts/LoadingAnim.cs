using UnityEngine;

namespace AIMStudio.Scripts
{
    public class LoadingAnim : MonoBehaviour
    {
        public void Update()
        {
            transform.Rotate(0f, 0f, Time.deltaTime * -100f);
        }
    }
}