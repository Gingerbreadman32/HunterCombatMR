using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.AnimationEngine.Services
{
    public static class ModContentLoadingService
    {
        #region Public Methods

        public static IDictionary<string, Texture2D> GetTexturesFromPath(string path)
            => HunterCombatMR.Instance.VariableTextures.Where(x => x.Key.StartsWith(path)).ToDictionary(x => x.Key, y => y.Value);

        #endregion Public Methods
    }
}