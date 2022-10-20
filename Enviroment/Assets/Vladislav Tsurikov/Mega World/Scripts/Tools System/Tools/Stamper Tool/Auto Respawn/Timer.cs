#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem.Stamper
{
    public class Timer
    {
        private float _timeRemaining = 10;
        private bool _timerIsRunning = false;
		private bool _startTimer = false;
    
    	public void StartTimer(float delay)
       	{
			if(_startTimer == false)
			{
				_startTimer = true;
				_timerIsRunning = true;
    			_timeRemaining = delay;
			}
       	}

		public void UpdateTimer(Action action)
       	{
       	    if (_timerIsRunning)
       	    {
       	        if (_timeRemaining > 0)
       	        {
       	            _timeRemaining -= Time.deltaTime;
       	        }
       	        else
       	        {
       	            _timeRemaining = 0;
       	            _timerIsRunning = false;
					
					_startTimer = false;
					action.Invoke();
       	        }
       	    }
       	}
    }
}
#endif