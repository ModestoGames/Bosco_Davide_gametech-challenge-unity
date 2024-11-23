using System.Collections;
using UnityEngine;

namespace com.modesto.notificationhandler
{
    /// <summary>
    /// Base class to pass to GameInitiator for app init handling
    /// </summary>
    public abstract class GameInitBehaviour : MonoBehaviour
    {
        public abstract IEnumerator Initialize();
    }
}