using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinematicUnityExplorer.NightRunners.Extensions
{
    public static class GameObjectExtensions
    {
        public static T NullCheck<T>(this T unityObject) where T : UnityEngine.Object
        {
            return unityObject ? unityObject : null;
        }
    }
}
