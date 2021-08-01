using HunterCombatMR.Enumerations;
using System;

namespace HunterCombatMR.Models.State
{
    public struct StateController
    {
        public StateController(StateControllerType type,
            object[] parameters,
            StateTrigger[][] triggers,
            int persistency,
            bool noHitPause)
        {
            Validate(triggers, persistency, false);

            Type = type;
            Triggers = triggers;
            Persistency = persistency;
            IgnoreHitPause = noHitPause;
            Parameters = parameters;
        }

        /// <summary>
        /// Whether or not the controller will be checked during hit pause.
        /// </summary>
        public bool IgnoreHitPause { get; }

        /// <summary>
        /// The amount of times triggers need to fire within the lifetime of the state to activate. 0 will indicate it can only be called once.
        /// </summary>
        public int Persistency { get; }

        /// <summary>
        /// Array of triggers that will activate this controller.
        /// Triggers within the same row must all be true in order to count as true.
        /// Triggers under row 0 must be true for all other rows.
        /// </summary>
        public StateTrigger[][] Triggers { get; }

        /// <summary>
        /// The logic this controller will perform upon a trigger being hit. Must be within defined types.
        /// </summary>
        public StateControllerType Type { get; }

        /// <summary>
        /// The parameters being passed to the state controller.
        /// </summary>
        public object[] Parameters { get; }

        public static bool Validate(StateTrigger[][] triggers,
                                            int persistency,
            bool ignoreErrors = true)
        {
            try
            {
                if (triggers.Length <= 0)
                    throw new Exception("State controller must have at least one trigger!");

                if (persistency < 0)
                    throw new Exception("Controller persistency must be above 0!");

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