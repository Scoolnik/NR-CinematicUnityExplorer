﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinematicUnityExplorer.NightRunners.Utils
{
    public static class RecordUtils
    {
        public static RCC_Recorder.Mode GetMode()
        {
            return RCC_SceneManager.instance.recorder.mode;
        }

        public static void StartRecord()
        {
            RCC.StartStopRecord();
        }

        public static void StopRecord()
        {
            RCC.StartStopRecord();
        }

        public static void StartReplay()
        {
            //Crutch. GO created with 'new' keyword intentionally, cause this prop is checked inside replay code logic and it should not be null
            RCC_SceneManager.instance.recorder.carController.externalControllerAI = new RCC_AICarController();
            RCC.StartStopReplay();
        }

        public static void StopReplay()
        {
            RCC_SceneManager.instance.recorder.carController.externalControllerAI = null;
            RCC.StartStopReplay();
        }
    }
}
