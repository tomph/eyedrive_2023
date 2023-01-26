using System.Collections.Generic;
using System;

public class Signal
{
	private List<SignalTarget> signalTargets = new List<SignalTarget>();

	public Signal()
	{
	}

    public void Add(Action Action)
    {
        if (!ContainsTarget(Action))
        {
            SignalTarget st = new SignalTarget(Action, false);
            signalTargets.Add(st);
        }
    }

    public void AddOnce(Action Action)
	{
		if(!ContainsTarget(Action))
		{
			SignalTarget st = new SignalTarget(Action, true);	
			signalTargets.Add(st);
		}
	}

	public void Remove(Action Action)
    {
		for (int i = 0; i < signalTargets.Count; i++) 
		{
			SignalTarget target = signalTargets[i];

			if(Action == target.action)
			{
				signalTargets.Remove(target);
			}
		}
	}

	public void RemoveAll()
	{
		signalTargets = new List<SignalTarget>();
	}

	public void Dispatch()
	{
		for (int i = 0; i < signalTargets.Count; i++) 
		{
			SignalTarget target = signalTargets[i];
            target.action.Invoke();
			if(target.isOneShot == true) signalTargets.Remove(target);
		}
    }

    bool ContainsTarget(Action Action)
    {
        foreach (SignalTarget st in signalTargets)
        {
            if (st.action == Action)
            {
                return true;
            }
        }

        return false;
    }
}

public class Signal<T>
{
    private List<SignalTarget<T>> signalTargets = new List<SignalTarget<T>>();

    public Signal()
    {
    }

    public void Add(Action<T> Action)
    {
        if (!ContainsTarget(Action))
        {
            SignalTarget<T> st = new SignalTarget<T>(Action, false);
            signalTargets.Add(st);
        }
    }

    public void Remove(Action<T> Action)
    {
        for (int i = 0; i < signalTargets.Count; i++)
        {
            SignalTarget<T> target = signalTargets[i];

            if (Action == target.action)
            {
                signalTargets.Remove(target);
            }
        }
    }

    public void RemoveAll()
    {
        signalTargets = new List<SignalTarget<T>>();
    }

    bool ContainsTarget(Action<T> Action)
    {
        foreach (SignalTarget<T> st in signalTargets)
        {
            if (st.action == Action)
            {
                return true;
            }
        }

        return false;
    }

    public void Dispatch(T value)
    {
        for (int i = 0; i < signalTargets.Count; i++)
        {
            SignalTarget<T> target = signalTargets[i];
            target.action.Invoke(value);
            if (target.isOneShot == true) signalTargets.Remove(target);

        }
    }

    internal void AddOnce(object onOrbHit)
    {
        throw new NotImplementedException();
    }
}

public class SignalTarget
{
    public bool isOneShot = false;
    public Action action;

    public SignalTarget(Action action, bool OneShot)
    {
        this.action = action;
        isOneShot = OneShot;
    }

}

public class SignalTarget<T> : SignalTarget
{
    public new Action<T> action;

    public SignalTarget(Action<T> action, bool OneShot)
        : base(null, OneShot)
    {
        this.action = action;
    }

}

