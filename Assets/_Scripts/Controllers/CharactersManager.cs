using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Characters;
using _Scripts.Enums;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace _Scripts.Controllers
{
    public class CharactersManager
    {
        public GameObject ActiveCharacterGameObject { private set; get; }
        
        public Character ActiveCharacter { private set; get; }
        public Animator ActiveAnimator { private set; get; }
        public ParticleSystem ActiveParticleSystem { private set; get; }
        public SpriteRenderer ActiveSpriteRenderer { private set; get; }

        
        List<GameObject> _charactersGameObjects = new List<GameObject>();
        int _characterIndex = 0;

        public CharactersManager(GameObject[] charactersModels, int startingCharacterIndex)
        {
            SetCharacterIndex(startingCharacterIndex);
            _charactersGameObjects = new List<GameObject>(charactersModels); 
            
            SetActiveCharacter(startingCharacterIndex);
            DisableOtherCharacters();
        }

        void SetCharacterIndex(int index)
        {
            _characterIndex = index is < 0 or > 5 ? 0 : index;
        }

        void SetActiveCharacter(int characterIndex)
        {
            ActiveCharacterGameObject = _charactersGameObjects[characterIndex];
            SetActiveCharacterComponents();
        }

        void SetActiveCharacterComponents()
        {
            ActiveCharacter = ActiveCharacterGameObject.GetComponent<Character>();
            
            ActiveAnimator = ActiveCharacter.GetComponent<Animator>();
            ActiveParticleSystem = ActiveCharacter.GetParticleSystem();
            ActiveSpriteRenderer = ActiveCharacter.GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Disable all character but not the one that is currently as character start index.
        /// </summary>
        void DisableOtherCharacters()
        {
            for (var i = 0; i < _charactersGameObjects.Count; i++)
            {
                if (i == _characterIndex)
                {
                    continue;
                }
                
                _charactersGameObjects[i].SetActive(false);
            }
        }
        


        public void ChangeCharacterTo(float input)
        {
            //Verificamos si hay solo un personaje, si hay un solo personaje, characterIndex siempre es 0
            if (_charactersGameObjects.Count == 1)
            {
                MoveIndexCharacter(0);
                return;
            }
            
            if (input == -1) //Cambiamos a la izquierda
            {
                //Si sobre pasamos el index, reiniciamos
                if (_characterIndex >= _charactersGameObjects.Count - 1)
                {
                    _characterIndex = 0;
                }
                else //sino, sumamos
                {
                    //Suma 1 al index del personaje actual
                    _characterIndex++; 
                }
            }

            if (input == 1) //Cambiamos a la derecha
            {
                //Si sobre pasamos el index, reiniciamos
                if (_characterIndex <= 0)
                {
                    _characterIndex = _charactersGameObjects.Count - 1;
                }
                else //sino, sumamos
                {
                    //Suma 1 al index del personaje actual
                    _characterIndex--; 
                }
                
            }
            
            MoveIndexCharacter(_characterIndex);
        }
        
        public void MoveIndexCharacter(int index)
        {
            //Guarda la posicion del personaje actual
            Transform charTrans = ActiveCharacter.transform;
                
            //Deshabilita el personaje actial
            ActiveCharacterGameObject.SetActive(false);
            //Cambia el personaje actual
            ActiveCharacterGameObject = _charactersGameObjects[index];
            //Habilita el nuevo personaje y le da su posicion
            ActiveCharacterGameObject.SetActive(true);
            ActiveCharacter = ActiveCharacterGameObject.GetComponent<Character>();

            // SetCurrentCharacterComponents();

            ActiveCharacter.transform.SetPositionAndRotation(charTrans.position, charTrans.rotation);

            //Le decimos a los suscriptores de actualizar el jugador
            //En este caso a OnScreenControls, HUDManager o Flying Game Manager
            //_hudManager.UpdateCharacterSpecsOnHUD();

            // cameraManager.FollowAt(currentCharacter.transform);
        }
        
        /**
        public bool TryToAddItemToInventory(ItemData itemData)
        {
            //si no se pudo agregar, return
            if (InventoryManager.Instance.AddItem(itemData))
            {
                return false;
            }
         
            _hudManager.UpdateItemsOnInventory();
            return true;
        }
        **/
        
        
        
        public void performOnPause(bool isPaused)
        {
            // TODO Make with Observer Patterns
        }

        public void ChangeCharacter(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
                
            //Para cambiar de personaje
            var sideToSwitch = context.ReadValue<float>();
                
            ChangeCharacterTo(sideToSwitch);
        }

        #region GettersAndSetters

        
        
        #endregion

        
    }
}
