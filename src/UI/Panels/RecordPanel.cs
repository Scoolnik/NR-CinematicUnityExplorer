using CinematicUnityExplorer.NightRunners.Utils;
using UnityExplorer.UI;
using UnityExplorer.UI.Panels;
using UniverseLib.UI;

namespace CinematicUnityExplorer.UI.Panels
{
    public class RecordPanel : UEPanel
    {
        public override UIManager.Panels PanelType => UIManager.Panels.Record;
        public override string Name => "Record";
        public override int MinWidth => 325;
        public override int MinHeight => 200;
        public override Vector2 DefaultAnchorMin => new(0.4f, 0.4f);
        public override Vector2 DefaultAnchorMax => new(0.6f, 0.6f);

        public RecordPanel(UIBase owner) : base(owner)
        {
        }

        protected override void ConstructPanelContent()
        {
            UIFactory.SetLayoutElement(
                UIFactory.CreateLabel(ContentRoot, "Warning", "Experimental, only player's car will be recorded", TextAnchor.MiddleCenter).gameObject, 
                minWidth: 150, minHeight: 25, flexibleWidth: 9999
            );

            var recordButton = UIFactory.CreateButton(ContentRoot, "ToggleRecordButton", "Start Record");
            UIFactory.SetLayoutElement(recordButton.GameObject, minWidth: 150, minHeight: 25, flexibleWidth: 9999);
            recordButton.OnClick += () =>
            {
                var mode = RecordUtils.GetMode();
                string nextActionName;
                if (mode == RCC_Recorder.Mode.Record)
                {
                    nextActionName = "Start";
                }
                else
                {
                    UIManager.ShowMenu = false;
                    nextActionName = "Stop";
                }
                recordButton.ButtonText.text = $"{nextActionName} Record";
                RecordUtils.StartRecord();
            };

            var replayButton = UIFactory.CreateButton(ContentRoot, "ToggleReplayButton", "Replay");
            UIFactory.SetLayoutElement(replayButton.GameObject, minWidth: 150, minHeight: 25, flexibleWidth: 9999);
            replayButton.OnClick += () =>
            {
                var mode = RecordUtils.GetMode();
                if (mode == RCC_Recorder.Mode.Play)
                {
                    RecordUtils.StopReplay();
                }
                else
                {
                    UIManager.ShowMenu = false;
                    RecordUtils.StartReplay();
                }
            };
        }
    }
}
