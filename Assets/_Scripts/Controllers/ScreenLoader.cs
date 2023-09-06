using System;
using System.Collections;
using _Scripts.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace _Scripts.Controllers
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(Animator))]
    public class ScreenLoader : MonoBehaviour
    {
        [SerializeField] private Animator spinnerAnimator;

        [SerializeField] private float waitTimeBeforeLoading = 0;
        [SerializeField] private float waitTimeAfterLoading = 0;

        [SerializeField] private Slider loadingSlider;

        private Animator _loadingScreenAnimator;

        private void Awake()
        {
            SetComponents();
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void SetComponents()
        {
            _loadingScreenAnimator = GetComponent<Animator>();
        }

        public void StartLoadScreen(string sceneToLoad)
        {
            StartCoroutine(LoadScreenAsync(sceneToLoad));
        }

        private IEnumerator LoadScreenAsync(string sceneToLoad)
        {
            StartLoadScreenAnimations();
            
            yield return new WaitForSecondsRealtime(waitTimeBeforeLoading);
            
            AsyncOperation loading = SceneManager.LoadSceneAsync(sceneToLoad);
            loading.completed += OnCompleteLoading;

            while (!loading.isDone)
            {
                loadingSlider.value = loading.progress;
                
                yield return null;
            }

            yield return new WaitForSecondsRealtime(waitTimeAfterLoading);                                        
            
            FinishLoadScreenAnimations();
            Destroy(gameObject);
        }
        private void StartLoadScreenAnimations()
        {
            spinnerAnimator.enabled = true;
            
            _loadingScreenAnimator.Play(TransitionsAnimations.FadeIn.ToString());
        }
        
        private void FinishLoadScreenAnimations()
        {
            spinnerAnimator.enabled = false;
        }
        
        private void OnCompleteLoading(AsyncOperation loading)
        {
            _loadingScreenAnimator.Play(TransitionsAnimations.FadeOut.ToString());
        }
    }
}