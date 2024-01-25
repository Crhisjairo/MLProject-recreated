using System.Collections;
using Cinemachine;
using UnityEngine;

namespace _Scripts.Controllers
{
    public class CameraController : MonoBehaviour
    {
        private CinemachineBrain _brainCam;
        [SerializeField] private float defaultFocusTime = 4f;
        [SerializeField] private int topPriority = 99;
        
        private float _focusTime;
        
        private void Awake()
        {
            _brainCam = GetComponent<CinemachineBrain>();
            _focusTime = defaultFocusTime;
        }

        public void SetFocusTime(float time)
        {
            _focusTime = time;
        }
        
        public void FollowTemporary(CinemachineVirtualCamera virtualCamera)
        {
            StartCoroutine(StartFocusTemporary(virtualCamera));
        }

        private IEnumerator StartFocusTemporary(CinemachineVirtualCamera virtualCamera)
        {
            Time.timeScale = 0;
            var initialPriority = virtualCamera.Priority;
            
            virtualCamera.enabled = true;
            virtualCamera.Priority = topPriority;
            
            yield return new WaitForSecondsRealtime(_focusTime);

            virtualCamera.Priority = initialPriority;
            virtualCamera.enabled = false;
            
            Time.timeScale = 1;
            
            _focusTime = defaultFocusTime;
        }
    }
}