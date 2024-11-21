using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameInitiator : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private bool _enableLog;
    [Range(30, 60)]
    [SerializeField] private int _fps;

    [Header("Components")]
    [SerializeField] private GameInitBehaviour[] _gameInitBehaviours;

    [SerializeField] private UnityEvent _onInitializationComplete;

    private void Start()
    {
        Debug.unityLogger.logEnabled = _enableLog;
        StartCoroutine(InitGame());
    }

    private IEnumerator InitGame()
    {
        foreach(var gameInitBehaviour in _gameInitBehaviours)
        {
            yield return gameInitBehaviour.Initialize();
        }

        _onInitializationComplete?.Invoke();
    }
}
