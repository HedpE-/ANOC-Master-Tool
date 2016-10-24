/*
 * Created by SharpDevelop.
 * User: Caramelos
 * Date: 30-07-2016
 * Time: 16:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace appCore.Logs
{
	/// <summary>
	/// Description of LogEnumerator.
	/// </summary>
	public class LogEnumerator<T> : IEnumerator<T> where T : Log
	{
//		protected LogsCollection<T> _collection; //enumerated collection
		protected int index; //current index
		protected T _current; //current enumerated object in the collection

		// Default constructor
		public LogEnumerator()
		{
			//nothing
		}

		// Paramaterized constructor which takes
		// the collection which this enumerator will enumerate
//		public LogEnumerator(LogsCollection<T> collection)
//		{
//			_collection = collection;
//			index = -1;
//			_current = default(T);
//		}

		// Current Enumerated object in the inner collection
		public virtual T Current
		{
			get
			{
				return _current;
			}
		}

		// Explicit non-generic interface implementation for IEnumerator
		// (extended and required by IEnumerator<T>)
		object IEnumerator.Current
		{
			get
			{
				return _current;
			}
		}

		// Dispose method
		public virtual void Dispose()
		{
			_collection = null;
			_current = default(T);
			index = -1;
		}

		// Move to next element in the inner collection
		public virtual bool MoveNext()
		{
			//make sure we are within the bounds of the collection
			if (++index >= _collection.Count)
			{
				//if not return false
				return false;
			}
			else
			{
				//if we are, then set the current element
				//to the next object in the collection
				_current = _collection[index];
			}
			//return true
			return true;
		}

		// Reset the enumerator
		public virtual void Reset()
		{
			_current = default(T); //reset current object
			index = -1;
		}
	}
}
