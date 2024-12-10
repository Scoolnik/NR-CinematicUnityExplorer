using CinematicUnityExplorer.Adapters.NightRunners.Extensions;

#if BIE
using Rewired;

#else
using Il2CppRewired;
#endif

namespace CinematicUnityExplorer.Adapters.NightRunners.Utils
{
    public static class RCCUtils
    {
        public static GameObject GetPlayerCar()
        {
            return GodConstant.Instance.playerCar.gameObject;
        }

        public static void ToggleCameraController(GameObject container, bool enable)
        {
            if (IsPlayerCameraScene())
            {
                TogglePersonController(container, enable);
            }
            else
            {
                ToggleRCC_Camera(container, enable);
            }
        }

        public static void ToggleGameUI(bool enable)
        {
            ToggleRCC_UI(enable);
        }

        public static GameObject GetCameraContainer(Camera camera)
        {
            var scene = GodConstant.Instance.scene_currentType;
            if (scene == GodConstant.Scene_currentType.GARAGE)
            {
                return GameObject.Find("homeGarage_player(Clone)");
            }
            else if (scene == GodConstant.Scene_currentType.MEETSPOT)
            {
                return GameObject.Find("walkScene_player(Clone)");
            }
            else
            {
                return camera.GetComponentInParent(Il2CppType.Of<RCC_Camera>()).gameObject.NullCheck() ?? GameObject.Find("MAIN_CAMERA(Clone)");
            }
        }

        public static void SetDefaultInputEnabled(bool enable)
        {
            try
            {
                ReInput.controllers.Keyboard.enabled = enable;
                var joysticks = ReInput.controllers.GetJoysticks();
                foreach (var joystick in joysticks)
                {
                    joystick.enabled = enable;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
            }
        }

        private static RCC_Camera GetRCC_Camera(GameObject container)
        {
            return container.NullCheck()?.GetComponent<RCC_Camera>();
        }

        private static void TogglePersonController(GameObject container, bool enable)
        {
            var controller = container.GetComponent<FPEFirstPersonController>();
            GodConstant.Instance.UI_Data.ui_showWarning = !enable; //prevents user interactions on meetspot
            if (controller)
            {
                controller.enabled = enable;
                if (controller.homegarage)
                {
                    controller.homegarage.noInput = !enable;
                }
                var vCamera = controller.transform.Find("vCamera");
                if (vCamera) 
                { 
                    vCamera.transform.localPosition = enable ? new Vector3(0, 0.85f, 0) : Vector3.zero; //direct camera GO parent
                    vCamera.transform.localRotation = Quaternion.identity;
                }
            } 
            else
            {
                Debug.LogWarning("FPEFirstPersonController not found ");
            }
        }

        private static void ToggleRCC_Camera(GameObject container, bool enable)
        {
            var rcc = GetRCC_Camera(container);
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

        private static bool IsPlayerCameraScene()
        {
            var scene = GodConstant.Instance.scene_currentType;
            return scene == GodConstant.Scene_currentType.GARAGE || scene == GodConstant.Scene_currentType.MEETSPOT;
        }
    }
}
