using System;
using System.Collections;
using _Scripts.GameManagerSystem;
using _Scripts.Shared.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.Controllers
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(Animator))]
    public class ScreenLoader : MonoBehaviour
    {
        [SerializeField] private Animator spinnerAnimator;
        [SerializeField] private Animator transitionAnimator;

        [SerializeField] private float waitTimeBeforeLoading;
        [SerializeField] private float waitTimeAfterLoading;
        [SerializeField] private float waitTimeForSaving = 5f;

        [SerializeField] private Slider loadingSlider;
        [SerializeField] private SaveDataTrigger saveDataTrigger;

        private Animator _loadingScreenAnimator;

        private Coroutine _loadingScreenRoutine;
        
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
            Action saveDataAction = () =>
            {
                saveDataTrigger.SaveGame(true);
            };

            // TODO: add saving animations
            if(_loadingScreenRoutine is null){
                _loadingScreenRoutine = StartCoroutine(
                    LoadScreenAsync(
                        sceneToLoad, saveDataAction
                    )
                );
            }
        }
        
        public void StartLoadScreenWithoutSaving(string sceneToLoad)
        {
            if(_loadingScreenRoutine is null)
                _loadingScreenRoutine = StartCoroutine(LoadScreenAsync(sceneToLoad));
        }
        
        public void LoadScreenFromSelectedSlot()
        {
            var saveData = SaveDataSystem.Instance.GetPlayerSaveDataSelected();
            var sceneName = saveData.lastSceneName.ToString();

            Debug.Log($"Loading screen from slot with name {saveData.slotName} !");
            StartLoadScreenWithoutSaving(sceneName);
        }

        private IEnumerator LoadScreenAsync(string sceneToLoad, Action onRoutineStarts = null)
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
            onRoutineStarts?.Invoke();
            
            yield return new WaitForSecondsRealtime(waitTimeForSaving);    
            
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
