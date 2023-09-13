using System;
using System.Collections;
using _Scripts.Enums;
using TMPro;
using UnityEngine;
namespace _Scripts.UI
{
    public class TutorialDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tutorialTitle;
        
        [SerializeField] private float secondsBeforeHide = 2f;

        [SerializeField] private float transitionSpeed = 1f;

        [SerializeField] private bool startOnAwake = false;
        [SerializeField] private float startOnAwakeDelay = 1f;
        [SerializeField] private string startOnAwakeTitle;

        private CanvasGroup _canvasGroup;

        private Coroutine _showCanvasOnAwakeRoutine, _hideCanvasRoutine;
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            if (startOnAwake)
            {
                _showCanvasOnAwakeRoutine = StartCoroutine(ShowCanvasOnAwake());
                tutorialTitle.text = startOnAwakeTitle;
            }
        }

        public void ShowTutorialCanvas()
        {
            LeanTween.alphaCanvas(_canvasGroup, 1f, transitionSpeed);
        }

        public void HideTutorialCanvas()
        {
            if(_hideCanvasRoutine is not null)
                return;
            
            if(_showCanvasOnAwakeRoutine is not null)
                StopCoroutine(_showCanvasOnAwakeRoutine);
            
            _hideCanvasRoutine = StartCoroutine(HideCanvas());
        }

        public void SetTutorialTitle(string title)
        {
            tutorialTitle.text = title;
        }
        
        private IEnumerator HideCanvas()
        {
            yield return new WaitForSecondsRealtime(secondsBeforeHide);
            LeanTween.alphaCanvas(_canvasGroup, 0f, transitionSpeed);

            _hideCanvasRoutine = null;
        }

        private IEnumerator ShowCanvasOnAwake()
        {
            yield return new WaitForSecondsRealtime(startOnAwakeDelay);
            LeanTween.alphaCanvas(_canvasGroup, 1f, transitionSpeed);
        }
    }
}
