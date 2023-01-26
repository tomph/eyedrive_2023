using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;


[System.Serializable]
public class NodeGateConnection
{

    private Node.Connection _source;

    public Intention intention;

   public NodeGateConnection(Node.Connection source)
   {
        _source = source;
   }
}

public enum Intention {Left,Middle,Right}
