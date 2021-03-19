using HunterCombatMR.AnimationEngine.Models;
using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Utilities
{
    public static class PlayerActionComboUtils
    {
        #region Public Methods

        public static ComboAction GetNextAvailableAction(PlayerStateController playerInfo,
            MoveSet weaponMoveset,
            PlayerBufferInformation bufferInformation)
        {
            if (playerInfo.State.Equals(PlayerState.Dead))
                return null;

            if (playerInfo.CurrentAction == null)
            {
                if ((!weaponMoveset.NeutralRoutes.Any(x => x.ComboAction.PlayerStateRequired.Equals(playerInfo.State) 
                        && x.StatesCancellableFrom.Contains(playerInfo.ActionState))))
                    return playerInfo.CurrentAction;

                var movesAvailable = new Dictionary<ComboRoute, int>();

                foreach (var route in weaponMoveset.NeutralRoutes.Where(x => x.ComboAction.PlayerStateRequired.Equals(playerInfo.State)
                    && x.StatesCancellableFrom.Contains(playerInfo.ActionState)))
                {
                    if (bufferInformation.BufferedComboInputs.Any(x => x.Input.Equals(route.Input)) && !route.InputHold)
                    {
                        movesAvailable.Add(route,
                            bufferInformation.BufferedComboInputs.OrderBy(x => x.FramesSinceBuffered).First(x => x.Input.Equals(route.Input)).FramesSinceBuffered);
                    } else if (bufferInformation.HeldComboInputs[route.Input] > route.InputHoldFrames && route.InputHold)
                    {
                        movesAvailable.Add(route,
                            bufferInformation.BufferedComboInputs.OrderBy(x => x.FramesSinceBuffered).First(x => x.Input.Equals(route.Input)).FramesSinceBuffered);
                    }
                }

                var orderedMoves = movesAvailable.OrderBy(x => x.Value);

                if (movesAvailable.Any(x => x.Key.InputHold))
                    return orderedMoves.First(x => x.Key.InputHold).Key.ComboAction;
                else if (movesAvailable.Any())
                    return orderedMoves.First().Key.ComboAction;
            }

            return playerInfo.CurrentAction;
        }

        #endregion Public Methods
    }
}