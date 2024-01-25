using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class FrameRateCounter : MonoBehaviour
    {
        private Text _txt;
        
        private float _frameCount;
        private double _dt;
        private double _fps;
        
        /// <summary>
        /// Updates per seconds.
        /// </summary>
        [SerializeField] private float updateRate = 4.0f;
    
        // Start is called before the first frame update
        void Start()
        {
            _txt = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            _frameCount++;
            _dt += Time.deltaTime;
            
            if (_dt > 1.0/updateRate)
            {
                _fps = _frameCount / _dt ;
                _frameCount = 0;
                _dt -= 1.0/updateRate;
            }
            
            _txt.text = "FPS: " + (Math.Truncate(_fps * 100) / 100);
        }
    }
}
