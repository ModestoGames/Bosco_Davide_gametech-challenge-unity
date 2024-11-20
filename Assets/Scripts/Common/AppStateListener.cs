using System.Collections;
using UnityEngine;

namespace com.modesto.notificationhandler
{
    /// <summary>
    /// Base class to implement components which updates based on application states.
    /// </summary>
    public abstract class AppStateListener : GameInitBehaviour
    {
        [SerializeField] protected AppStateManager _appStateManager;

        public override IEnumerator Initialize()
        {
            _appStateManager.AddListener(AppStateChanged);
            yield return OnInitialize();
        }

        protected abstract IEnumerator OnInitialize();

        private void OnDisable()
        {
            _appStateManager.RemoveListener(AppStateChanged);
        }

        public abstract void AppStateChanged(ApplicationState previousState, ApplicationState currentState);
    }
}