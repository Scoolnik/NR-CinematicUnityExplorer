using CinematicUnityExplorer.NightRunners.Extensions;
using Il2CppInterop.Runtime;
using UnityEngine;

namespace CinematicUnityExplorer.NightRunners.Utils
{
    public static class RCCUtils
    {
        public static void ToggleRCC(Camera camera, bool enable)
        {
            ToggleRCC_Camera(camera, enable);
            ToggleRCC_UI(enable);
        }

        private static RCC_Camera GetRCC_Camera(Camera camera)
        {
            return camera.GetComponentInParent(Il2CppType.Of<RCC_Camera>()) as RCC_Camera ??
                GameObject.Find("MAIN_CAMERA(Clone)").NullCheck()?.GetComponent<RCC_Camera>();
        }

        private static void ToggleRCC_Camera(Camera camera, bool enable)
        {
            var rcc = GetRCC_Camera(camera);
            if (rcc)
            {
                if (!enable)
                {
                    rcc.gameObject.transform.rotation = Quaternion.identity;
                }
                rcc.enabled = enable;
            }
            else
            {
                Debug.LogWarning("Rcc camera not found");
            }
        }

        private static void ToggleRCC_UI(bool enable)
        {
            var ui = GodConstant.Instance.UI_Data;

            var mainCanvas = ui.gameObject;
            if (mainCanvas)
            {
                mainCanvas.SetActive(enable);
                ui.loading_blackScreen.transform.parent.parent.gameObject.SetActive(enable); //loading screen canvas, includes "now playing" ui
            }
            else
            {
                Debug.LogWarning("Rcc ui canvas not found");
            }
        }
    }
}
