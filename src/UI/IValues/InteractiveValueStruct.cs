﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityExplorer.UI.CacheObject;
using UnityExplorer.UI.Utility;

namespace UnityExplorer.UI.IValues
{
    public class InteractiveValueStruct : InteractiveValue
    {
        #region Struct cache / wrapper

        public class StructInfo
        {
            public bool IsSupported;
            public FieldInfo[] Fields;

            public StructInfo(bool isSupported, FieldInfo[] fields)
            {
                IsSupported = isSupported;
                Fields = fields;
            }

            public void SetValue(object instance, string value, int fieldIndex)
            {
                try
                {
                    var field = Fields[fieldIndex];

                    object val;
                    if (field.FieldType == typeof(string))
                        val = value;
                    else
                        val = ReflectionUtility.GetMethodInfo(field.FieldType, "Parse", ArgumentUtility.ParseArgs)
                                .Invoke(null, new object[] { value });

                    field.SetValue(instance, val);
                }
                catch (FormatException)   { ExplorerCore.LogWarning($"Invalid argument '{value}'!"); }
                catch (ArgumentException) { ExplorerCore.LogWarning($"Invalid argument '{value}'!"); }
                catch (OverflowException) { ExplorerCore.LogWarning($"Invalid argument '{value}'!"); }
                catch (Exception ex)
                {
                    ExplorerCore.Log("Excepting setting value '" + value + "'! " + ex);
                }
            }

            public string GetValue(object instance, int fieldIndex)
            {
                return Fields[fieldIndex].GetValue(instance)?.ToString() ?? "";
            }
        }

        private static readonly Dictionary<string, StructInfo> typeSupportCache = new Dictionary<string, StructInfo>();

        private const BindingFlags INSTANCE_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        private const string SYSTEM_VOID = "System.Void";

        public static bool SupportsType(Type type)
        {
            if (!type.IsValueType || string.IsNullOrEmpty(type.AssemblyQualifiedName) || type.FullName == SYSTEM_VOID)
                return false;

            if (typeSupportCache.TryGetValue(type.AssemblyQualifiedName, out var info))
                return info.IsSupported;

            var supported = true;

            var fields = type.GetFields(INSTANCE_FLAGS);

            if (fields.Any(it => !it.FieldType.IsPrimitive && it.FieldType != typeof(string)))
            {
                supported = false;
                info = new StructInfo(supported, null);
            }
            else
            {
                supported = true;
                info = new StructInfo(supported, fields);
            }

            typeSupportCache.Add(type.AssemblyQualifiedName, info);

            return supported;
        }

        #endregion

        public object RefInstance;

        public StructInfo CurrentInfo;
        private Type lastStructType;

        private ButtonRef applyButton;
        private readonly List<GameObject> fieldRows = new List<GameObject>();
        private readonly List<InputFieldRef> inputFields = new List<InputFieldRef>();
        private readonly List<Text> labels = new List<Text>();

        public override void OnBorrowed(CacheObjectBase owner)
        {
            base.OnBorrowed(owner);

            applyButton.Button.gameObject.SetActive(owner.CanWrite);
        }

        // Setting value from owner to this

        public override void SetValue(object value)
        {
            RefInstance = value;

            var type = RefInstance.GetType();

            if (type != lastStructType)
            {
                CurrentInfo = typeSupportCache[type.AssemblyQualifiedName];
                SetupUIForType();
                lastStructType = type;
            }

            for (int i = 0; i < CurrentInfo.Fields.Length; i++)
            {
                inputFields[i].Text = CurrentInfo.GetValue(RefInstance, i);
            }
        }

        private void OnApplyClicked()
        {
            try
            {
                for (int i = 0; i < CurrentInfo.Fields.Length; i++)
                {
                    CurrentInfo.SetValue(RefInstance, inputFields[i].Text, i);
                }

                CurrentOwner.SetUserValue(RefInstance);
            }
            catch (Exception ex)
            {
                ExplorerCore.LogWarning("Exception setting value: " + ex);
            }
        }
        
        // UI Setup for type

        private void SetupUIForType()
        {
            for (int i = 0; i < CurrentInfo.Fields.Length || i <= inputFields.Count; i++)
            {
                if (i >= CurrentInfo.Fields.Length)
                {
                    if (i >= inputFields.Count)
                        break;

                    fieldRows[i].SetActive(false);
                    continue;
                }

                if (i >= inputFields.Count)
                    AddEditorRow();

                fieldRows[i].SetActive(true);

                string label = SignatureHighlighter.Parse(CurrentInfo.Fields[i].FieldType, false);
                label += $" <color={SignatureHighlighter.FIELD_INSTANCE}>{CurrentInfo.Fields[i].Name}</color>:";
                labels[i].text = label;
            }
        }

        private void AddEditorRow()
        {
            var row = UIFactory.CreateUIObject("HoriGroup", UIRoot);
            //row.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            UIFactory.SetLayoutElement(row, minHeight: 25, flexibleWidth: 9999);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(row, false, false, true, true, 8, childAlignment: TextAnchor.MiddleLeft);

            fieldRows.Add(row);

            var label = UIFactory.CreateLabel(row, "Label", "notset", TextAnchor.MiddleRight);
            UIFactory.SetLayoutElement(label.gameObject, minHeight: 25, minWidth: 150, flexibleWidth: 0);
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            labels.Add(label);

            var input = UIFactory.CreateInputField(row, "InputField", "...");
            UIFactory.SetLayoutElement(input.UIRoot, minHeight: 25, minWidth: 100);
            var fitter = input.UIRoot.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            input.InputField.lineType = InputField.LineType.MultiLineNewline;
            inputFields.Add(input);
        }

        // UI Construction

        public override GameObject CreateContent(GameObject parent)
        {
            UIRoot = UIFactory.CreateVerticalGroup(parent, "InteractiveValueStruct", false, false, true, true, 3, new Vector4(4, 4, 4, 4),
                new Color(0.06f, 0.06f, 0.06f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(UIRoot, minHeight: 25, flexibleWidth: 9999);

            applyButton = UIFactory.CreateButton(UIRoot, "ApplyButton", "Apply", new Color(0.2f, 0.27f, 0.2f));
            UIFactory.SetLayoutElement(applyButton.Button.gameObject, minHeight: 25, minWidth: 100);
            applyButton.OnClick += OnApplyClicked;

            return UIRoot;
        }
    }
}
