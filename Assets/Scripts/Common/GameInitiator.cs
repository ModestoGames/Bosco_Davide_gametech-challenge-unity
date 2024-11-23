using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace com.modesto.notificationhandler
{
    /// <summary>
    /// Main class to handle Unity init from a single entry point
    /// </summary>
    public class GameInitiator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool _enableLog;
        [Range(30, 60)]
        [SerializeField] private int _fps;

        [Header("Components")]
        [SerializeField] private GameInitBehaviour[] _gameInitBehaviours;

        [Header("Events")]
        [SerializeField] private UnityEvent _onInitializationComplete;

        private void Start()
        {
            Debug.unityLogger.logEnabled = _enableLog;
            Application.targetFrameRate = _fps;
            StartCoroutine(InitGame());
        }

        private IEnumerator InitGame()
        {
            //iterate through gameInitBehaviour to init them syncronously
            foreach (var gameInitBehaviour in _gameInitBehaviours)
            {
                yield return gameInitBehaviour.Initialize();
            }

            yield return new WaitForSeconds(0.5f);
            _onInitializationComplete?.Invoke();
        }
    }
}