using UnityEngine;

namespace Suntail
{
    public class LightCulling : MonoBehaviour
    {
        //빛과 관련된 함수 빛이 조사하는것을 카메라가 담을때 어떻게 받을것인지 
        //그림자는 어떻게 받을 것인지 경정하는 함수
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private float shadowCullingDistance = 15f;
        [SerializeField] private float lightCullingDistance = 30f;
        private Light _light;
        public bool enableShadows = false;

        private void Awake()
        {
            _light = GetComponent<Light>();
        }

        private void Update()
        {
            float cameraDistance = Vector3.Distance(playerCamera.transform.position, gameObject.transform.position);

            if (cameraDistance <= shadowCullingDistance && enableShadows)
            {
                _light.shadows = LightShadows.Soft;
            }
            else
            {
                _light.shadows = LightShadows.None;
            }

            if (cameraDistance <= lightCullingDistance)
            {
                _light.enabled = true;
            }
            else
            {
                _light.enabled = false;
            }
        }
    }
}