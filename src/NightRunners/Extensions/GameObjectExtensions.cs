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
