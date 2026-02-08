using UnityEngine;

public interface IObservable<T>
{
    public void AddObservable(T observable);

    public void RemoveObservable(T observable);

    
}
