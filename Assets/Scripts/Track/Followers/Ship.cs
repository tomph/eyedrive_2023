using System;
using Dreamteck.Splines;
using EyegazeCore;
using UnityEngine;
using UnityEngine.Events;

public class Ship : MonoBehaviour
{
   private PreciseFollower _precise;
   private LaneFollowerDeluxe _lanes;
   private GamepadFollower _gamepad;
   private BaseFollower _active;

   private SplineTracer _tracer;
   private bool _inJunction;

   public UnityEvent ON_LAP_COMPLETE = new UnityEvent();

   public Ship Initialise(SplineComputer computer, InputManager input,  double percent)
   {
      _tracer = GetComponent<SplineTracer>();
      
      //break this out into a new component?
      ThreeWayJunction[] junctions = computer.GetComponentsInChildren<ThreeWayJunction>();
      foreach (ThreeWayJunction j in junctions)
      {
         j.ON_JUNCTION_TRIGGERED.AddListener(OnJunctionEntered);
         j.ON_JUNCTION_EXIT.AddListener(OnJunctionExited);
      }
      
      if (_precise == null)
      {
         _precise = GetComponent<PreciseFollower>();
         _precise.computer = computer;

         _precise.Init(percent);
      }

      if (_lanes == null)
      {
         _lanes = GetComponent<LaneFollowerDeluxe>();
         _lanes.computer = computer;

         _lanes.Init(percent);
      }

      if (_gamepad == null)
      {
         _gamepad = GetComponent<GamepadFollower>();
         _gamepad.computer = computer;

         _gamepad.Init(percent, input);
      }
      

      return this;
   }
   
   private void OnJunctionExited()
   {
      _inJunction = false;
   }

   private void OnJunctionEntered(JunctionAddress junctionAddress)
   {
      Debug.Log("OnJunctionEntered: " + junctionAddress.direction + " / " + _active.intention);
        
      if(junctionAddress.direction == _active.intention)
      {
         //Clear
         if (_tracer.address.elements.Length > 1) _tracer.ExitAddress(1);

         //Assign Route
         _tracer.EnterAddress(junctionAddress.node, junctionAddress.index);

         //Assign Exit
         _tracer.EnterAddress(junctionAddress.exit, 0);
      }

      _active.OnJunctionEntered();
        
      _inJunction = true;
   }

   public void Tick(float delta, ModeType mode)
   {
      if (mode == ModeType.Lanes)
         _lanes.Tick(delta);
      else if(mode == ModeType.Precise || mode == ModeType.Mouse)
         _precise.Tick(delta);
      else if(mode == ModeType.Gamepad)
         _gamepad.Tick(delta);
   }
   
   public void Pause(float delta, ModeType mode)
   {
      if (mode == ModeType.Lanes)
         _lanes.OnPause(delta);
      else 
         _precise.OnPause(delta);
   }

   public double GetPercent(ModeType mode)
   {
      if (mode == ModeType.Lanes)
         return _lanes.projectedPosition;
      else
         return _precise.projectedPosition;
   }

   public void OnModeChange(ModeType mode)
   {
      double percent = _active == null ? 0.0 : _active.clampedPercent;
      
      if (mode == ModeType.Lanes)
      {
         _active = _lanes;
         _lanes.Init(percent);
         
         _lanes.ON_LAP_COMPLETE.RemoveAll();
         _lanes.ON_LAP_COMPLETE.Add(ON_LAP_COMPLETE.Invoke);
      }
      else if(mode == ModeType.Precise || mode == ModeType.Mouse)
      {
         _active = _precise;
         _precise.Init(percent);
         
         _precise.ON_LAP_COMPLETE.RemoveAll();
         _precise.ON_LAP_COMPLETE.Add(ON_LAP_COMPLETE.Invoke);
      }else if (mode == ModeType.Gamepad)
      {
         _active = _gamepad;
         _gamepad.Init(percent);
         
         _gamepad.ON_LAP_COMPLETE.RemoveAll();
         _gamepad.ON_LAP_COMPLETE.Add(ON_LAP_COMPLETE.Invoke);
      }
   }
}
