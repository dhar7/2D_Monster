using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    T owner;
    public State<T> CurrentState { get; private set; }

    public Stack<State<T>> StateStack { get; private set; }

    public StateMachine(T owner)
    {
        this.owner = owner;
        StateStack = new Stack<State<T>>();
    }

    public void HandleUpdate()
    {
        CurrentState.Execute(owner);
    }

    public void ChangeState(State<T> newState)
    {
        if (CurrentState != null)
        {
            CurrentState.Exit(owner);
            StateStack.Pop();
        }

        CurrentState = newState;
        StateStack.Push(CurrentState);
        CurrentState.Enter(owner);
    }

    public void PushState(State<T> newState)
    {
        CurrentState = newState;
        StateStack.Push(CurrentState);
        CurrentState.Enter(owner);
    }

    public void PopState()
    {
        var popedState = StateStack.Pop();
        popedState.Exit(owner);

        CurrentState = StateStack.Peek();
    }
}
