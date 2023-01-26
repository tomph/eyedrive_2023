using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

[ExecuteInEditMode]
public sealed class NodeGate : Node
{
    public List<NodeGateConnection> _gateConnections = new List<NodeGateConnection>();

    public new void UpdateConnectedComputers(SplineComputer excludeComputer = null)
    {
        base.UpdateConnectedComputers(excludeComputer);
        Build();
    }

    public void Build()
    {
        _gateConnections.Clear();

        Node.Connection[] connections = GetConnections();
        foreach(Node.Connection nc in connections)
        {
            _gateConnections.Add(new NodeGateConnection(nc));
        }

    }

    public bool hasIntention(Intention i)
    {
        foreach (NodeGateConnection c in _gateConnections) if (c.intention == i) return true;
        return false;
    }

    public int GetConnectionIndex(Intention intention)
    {
        for (int i = 0; i < _gateConnections.Count; i++) if (_gateConnections[i].intention == intention) return i;
        return 0;
    }
}
