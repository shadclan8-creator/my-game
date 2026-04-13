using System;
using UnityEngine;

namespace TimesBaddestCat.Foundation
{
    /// <summary>
    /// Base class for all game events in the event system.
    /// Events are used for decoupled communication between systems.
    /// </summary>
    public abstract class GameEventBase
    {
        /// <summary>
        /// Timestamp when the event was created.
        /// </summary>
        public float Timestamp { get; private set; }

        /// <summary>
        /// Optional source of the event (e.g., "Player", "Enemy", "System").
        /// </summary>
        public string Source { get; set; }

        protected GameEventBase()
        {
            Timestamp = Time.time;
        }

        public override string ToString()
        {
            return $"[{GetType().Name}] Source: {Source}, Time: {Timestamp:F2}s";
        }
    }

    /// <summary>
    /// Generic event with a value payload.
    /// </summary>
    public class GameEvent<T> : GameEventBase
    {
        /// <summary>
        /// The value payload of this event.
        /// </summary>
        public T Value { get; set; }

        public GameEvent(T value) : base()
        {
            Value = value;
        }
    }
}
