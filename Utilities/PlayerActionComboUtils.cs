using HunterCombatMR.AttackEngine.Models;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Models.Player;
using HunterCombatMR.Services.Systems;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Utilities
{
    public static class PlayerActionComboUtils
    {
        public static ComboAction GetNextAvailableAction(PlayerStateController playerInfo,
            MoveSet weaponMoveset,
            InputSystem bufferInformation)
        {
            if (playerInfo.State.Equals(EntityWorldStatus.Dead))
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
                    if (bufferInformation.BufferedInputs.Any(x => x.Input.Equals(route.Input)) && !route.InputHold)
                    {
                        movesAvailable.Add(route,
                            bufferInformation.BufferedInputs.OrderBy(x => x.FramesSinceBuffered).First(x => x.Input.Equals(route.Input)).FramesSinceBuffered);
                        continue;
                    }
                }

                var orderedMoves = movesAvailable.OrderBy(x => x.Value);

                if (movesAvailable.Any(x => x.Key.InputHold))
                {
                    return orderedMoves.First(x => x.Key.InputHold).Key.ComboAction;
                }

                if (movesAvailable.Any())
                {
                    return orderedMoves.First().Key.ComboAction;
                }
            }

            return playerInfo.CurrentAction;
        }
    }
}