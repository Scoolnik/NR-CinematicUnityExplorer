﻿using UniverseLib.Input;
using UniverseLib.UI;
using UniverseLib.UI.Models;
#if UNHOLLOWER
using UnhollowerRuntimeLib;
#endif
#if INTEROP
using Il2CppInterop.Runtime.Injection;
#endif

using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

namespace UnityExplorer.UI.Panels
{
    public class CamPaths : UEPanel
    {
        public CamPaths(UIBase owner) : base(owner)
        {
        }

        public override string Name => "CamPaths";
        public override UIManager.Panels PanelType => UIManager.Panels.CamPaths;
        public override int MinWidth => 700;
        public override int MinHeight => 600;
        public override Vector2 DefaultAnchorMin => new(0.4f, 0.4f);
        public override Vector2 DefaultAnchorMax => new(0.6f, 0.6f);
        public override bool NavButtonWanted => true;
        public override bool ShouldSaveActiveState => true;
		public List<CatmullRom.PathControlPoint> controlPoints = new List<CatmullRom.PathControlPoint>();
        List<GameObject> UINodes = new List<GameObject>();
        bool closedLoop;

        // ~~~~~~~~ UI construction / callbacks ~~~~~~~~

        protected override void ConstructPanelContent()
        {   
            GameObject horiGroup = UIFactory.CreateHorizontalGroup(ContentRoot, "MainOptions", false, false, true, true, 3,
                default, new Color(1, 1, 1, 0), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(horiGroup, minHeight: 25, flexibleWidth: 9999);

            ButtonRef startButton = UIFactory.CreateButton(horiGroup, "Start", "Start CamPath");
            UIFactory.SetLayoutElement(startButton.GameObject, minWidth: 150, minHeight: 25, flexibleWidth: 9999);
            startButton.OnClick += StartButton_OnClick;

            ButtonRef pauseContinueButton = UIFactory.CreateButton(horiGroup, "Pause/Continue", "Pause/Continue");
            UIFactory.SetLayoutElement(pauseContinueButton.GameObject, minWidth: 150, minHeight: 25, flexibleWidth: 9999);
            pauseContinueButton.OnClick += PauseContinue_OnClick;

            ButtonRef stopButton = UIFactory.CreateButton(horiGroup, "Stop", "Stop CamPath");
            UIFactory.SetLayoutElement(stopButton.GameObject, minWidth: 150, minHeight: 25, flexibleWidth: 9999);
            stopButton.OnClick += Stop_OnClick;

            ButtonRef AddNode = UIFactory.CreateButton(horiGroup, "AddCamNode", "Add Cam node");
            UIFactory.SetLayoutElement(AddNode.GameObject, minWidth: 150, minHeight: 25, flexibleWidth: 9999);
            AddNode.OnClick += AddNode_OnClick;

            ButtonRef DeletePath = UIFactory.CreateButton(horiGroup, "DeletePath", "Delete Path");
            UIFactory.SetLayoutElement(DeletePath.GameObject, minWidth: 150, minHeight: 25, flexibleWidth: 9999);
            DeletePath.OnClick += () => {controlPoints.Clear(); UpdateListNodes();};

            Toggle closedLoopToggle = new Toggle();
            GameObject toggleObj = UIFactory.CreateToggle(horiGroup, "Close path in a loop", out closedLoopToggle, out Text toggleText);
            UIFactory.SetLayoutElement(toggleObj, minHeight: 25, flexibleWidth: 9999);
            closedLoopToggle.onValueChanged.AddListener((isClosedLoop) => {closedLoop = isClosedLoop;});
            closedLoopToggle.isOn = false;
            toggleText.text = "Close path in a loop";
        }

        private void UpdateListNodes(){
            //Refresh list
            foreach (var comp in UINodes){
                UnityEngine.Object.Destroy(comp);
                //UIElements.Remove(comp);
            }

            foreach (var point in controlPoints) {
                DrawNodeOptions(point);
            }
        }

        private void DrawNodeOptions(CatmullRom.PathControlPoint point){
            GameObject horiGroup = UIFactory.CreateHorizontalGroup(ContentRoot, "LightOptions", true, false, true, false, 4,
                default, new Color(1, 1, 1, 0), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(horiGroup, minHeight: 25, flexibleWidth: 9999);
            UINodes.Add(horiGroup);

            //Move to Camera
            ButtonRef moveToCameraButton = UIFactory.CreateButton(horiGroup, "Move Node to Camera", "Move Node to Camera");
            UIFactory.SetLayoutElement(moveToCameraButton.GameObject, minWidth: 100, minHeight: 25, flexibleWidth: 9999);
            moveToCameraButton.OnClick += () => {point.position = FreeCamPanel.ourCamera.transform.position; point.rotation = FreeCamPanel.ourCamera.transform.rotation;};

            ButtonRef copyFovButton = UIFactory.CreateButton(horiGroup, "Copy Camera FoV", "Copy Camera FoV");
            UIFactory.SetLayoutElement(copyFovButton.GameObject, minWidth: 100, minHeight: 25, flexibleWidth: 9999);
            copyFovButton.OnClick += () => {point.fov = FreeCamPanel.ourCamera.fieldOfView;};

            ButtonRef moveToPointButton = UIFactory.CreateButton(horiGroup, "Move Cam to Node", "Move Cam to Node");
            UIFactory.SetLayoutElement(moveToPointButton.GameObject, minWidth: 100, minHeight: 25, flexibleWidth: 9999);
            moveToPointButton.OnClick += () => {FreeCamPanel.ourCamera.transform.position = point.position; FreeCamPanel.ourCamera.transform.rotation = point.rotation; FreeCamPanel.ourCamera.fieldOfView = point.fov;};

            ButtonRef destroyButton = UIFactory.CreateButton(horiGroup, "Delete", "Delete");
            UIFactory.SetLayoutElement(destroyButton.GameObject, minWidth: 80, minHeight: 25, flexibleWidth: 9999);
            destroyButton.OnClick += () => {controlPoints.Remove(point); UpdateListNodes();};

            //Frames

            //ShowPos and rot?
        }

        void PauseContinue_OnClick(){
            if(ExplorerCore.CameraPathsManager.playingPath)
                ExplorerCore.CameraPathsManager.Pause();
            else
                ExplorerCore.CameraPathsManager.Continue();
        }

        void Stop_OnClick(){
            CatmullRom.PathControlPoint point = controlPoints[0];
            ExplorerCore.CameraPathsManager.Stop();

            FreeCamPanel.ourCamera.transform.position = point.position;
            FreeCamPanel.ourCamera.transform.rotation = point.rotation;
            FreeCamPanel.ourCamera.fieldOfView = point.fov;
        }

        void AddSpacer(int height)
        {
            GameObject obj = UIFactory.CreateUIObject("Spacer", ContentRoot);
            UIFactory.SetLayoutElement(obj, minHeight: height, flexibleHeight: 0);
        }

        void StartButton_OnClick()
        {
            int resolution = 500;
            //float normalExtrusion = 0;
            //float tangentExtrusion = 0;

            if(ExplorerCore.CameraPathsManager == null)
                ExplorerCore.CameraPathsManager = new CatmullRom(controlPoints.ToArray(), resolution, closedLoop);
            else{
                ExplorerCore.CameraPathsManager.Update(controlPoints.ToArray());
                ExplorerCore.CameraPathsManager.Update(resolution, closedLoop);
            }
            
            ExplorerCore.CameraPathsManager.StartPath();
        }

        void AddNode_OnClick(){
            Camera freeCam = FreeCamPanel.ourCamera;
            CatmullRom.PathControlPoint point = new CatmullRom.PathControlPoint(freeCam.transform.position, freeCam.transform.rotation, freeCam.fieldOfView);
            controlPoints.Add(point);

            UpdateListNodes();
        }
    }
}
