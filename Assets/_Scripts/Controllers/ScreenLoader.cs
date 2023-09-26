using System;
using System.Collections;
using _Scripts.Enums;
using _Scripts.GameManagerSystem;
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
        [SerializeField] private Animator transitionAnimator;

        [SerializeField] private float waitTimeBeforeLoading = 0;
        [SerializeField] private float waitTimeAfterLoading = 0;

        [SerializeField] private Slider loadingSlider;
        [SerializeField] private SaveGameTrigger saveGameTrigger;

        private Animator _loadingScreenAnimator;

        private void Awake()
        {
            SetComponents();
        }

        private void Start()
        {
            spinnerAnimator.enabled = false;
            transitionAnimator.enabled = false;
            
            DontDestroyOnLoad(gameObject);
        }

        private void SetComponents()
        {
            _loadingScreenAnimator = GetComponent<Animator>();
        }

        public void StartLoadScreen(string sceneToLoad)
        {
            // TODO: add saving animations
            StartCoroutine(LoadScreenAsync(sceneToLoad));
            saveGameTrigger.SaveGame();
        }

        public void StartLoadScreenWithoutSaving(string sceneToLoad)
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
            transitionAnimator.enabled = true;
            
            _loadingScreenAnimator.Play(TransitionsAnimations.FadeIn.ToString());
        }
        
        private void FinishLoadScreenAnimations()
        {
            spinnerAnimator.enabled = false;
            transitionAnimator.enabled = false;
        }
        
        private void OnCompleteLoading(AsyncOperation loading)
        {
            _loadingScreenAnimator.Play(TransitionsAnimations.FadeOut.ToString());
        }
    }
}
