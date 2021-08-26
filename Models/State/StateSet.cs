using HunterCombatMR.Constants;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HunterCombatMR.Models.State
{
    /// <summary>
    /// A set of states associated with a particular entity type or facet of an entity.
    /// </summary>
    /// <remarks>
    /// Must have a state equal to the default <see cref="StateNumberConstants.Default"/>.
    /// State <see cref="StateNumberConstants.Default"/> is the default state and will be what the entity is sent to if it is told to end without a state to change to.
    /// </remarks>
    public struct StateSet
    {
        private readonly IReadOnlyDictionary<int, EntityState> _states;

        public StateSet(IDictionary<int, EntityState> states)
        {
            Validate(states, false);

            _states = new ReadOnlyDictionary<int, EntityState>(states);
        }

        public IReadOnlyDictionary<int, EntityState> States { get => _states; }

        public static bool Validate(IDictionary<int, EntityState> states,
            bool ignoreErrors = true)
        {
            try
            {
                if (!states.Any(x => x.Key.Equals(StateNumberConstants.Default)))
                    throw new Exception($"State set must have a state number {StateNumberConstants.Default}!");

                return true;
            }
            catch (Exception ex)
            {
                if (ignoreErrors)
                    return false;

                throw ex;
            }
        }
    }
}