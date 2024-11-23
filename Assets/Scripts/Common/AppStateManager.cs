using System;
using System.Collections;

namespace com.modesto.notificationhandler
{
    /// <summary>
    /// Keep track of current application state and send event to its listener when state changes
    /// </summary>
    public class AppStateManager : GameInitBehaviour
    {
        //event to notify application state change by passing previous and current state
        private Action<ApplicationState, ApplicationState> _stateChangeEvent;

        private ApplicationState _previousState;
        private ApplicationState _currentState;

        public void ChangeApplicationState(ApplicationState currentState)
        {
            //if same state do not trigger events
            if (_previousState == currentState)
                return;

            _previousState = _currentState;
            _currentState = currentState;
            _stateChangeEvent?.Invoke(_previousState, _currentState);
        }

        public void AddListener(Action<ApplicationState, ApplicationState> listener)
        {
            _stateChangeEvent += listener;
        }

        public void RemoveListener(Action<ApplicationState, ApplicationState> listener)
        {
            _stateChangeEvent -= listener;
        }

        //use it just to make sure this object is initialized
        public override IEnumerator Initialize()
        {
            yield return null;
        }
    }
}