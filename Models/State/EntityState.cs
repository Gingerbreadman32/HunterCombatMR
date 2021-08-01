using System;

namespace HunterCombatMR.Models.State
{
    /// <summary>
    /// The entity state model.
    /// </summary>
    public struct EntityState
    {
        private readonly StateController[] _controllers;
        private readonly StateDef _stateDef;

        public EntityState(StateController[] stateControllers,
            StateDef stateDef)
        {
            Validate(stateControllers, false);

            _controllers = stateControllers;
            _stateDef = stateDef;
        }

        public static bool Validate(StateController[] stateControllers,
            bool ignoreErrors = true)
        {
            try
            {

                if (stateControllers.Length < 1)
                    throw new Exception("State must have at least one state controller!");

                return true;
            }
            catch (Exception ex)
            {
                if (ignoreErrors)
                    return false;

                throw ex;
            }
        }

        public StateController[] Controllers { get => _controllers; }

        public StateDef Definition { get => _stateDef; }
    }
}