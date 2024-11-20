using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameInitBehaviour : MonoBehaviour
{
    public abstract IEnumerator Initialize();
}
