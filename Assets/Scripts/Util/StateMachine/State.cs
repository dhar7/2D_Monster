using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T> : MonoBehaviour
{
    public virtual void Enter(T entity) { }

    public abstract void Execute(T entity);

    public virtual void Exit(T entity) { }
}
