// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Common;
using LeopotamGroup.Events;
using LeopotamGroup.Scripting;
using LeopotamGroup.SystemUi.Actions;
using LeopotamGroup.SystemUi.Markup;
using UnityEngine;
using UnityEngine.UI;

namespace LeopotamGroup.EditorHelpers {
    internal abstract class DeveloperConsoleBase : UnitySingletonBase {
        const string MarkupSchema = "LeopotamGroup/EditorHelpers/DevConsole";

        const string MarkupTheme = "LeopotamGroup/EditorHelpers/DevConsoleTheme";

        const string MarkupScrollViewName = "logScroll";

        const string MarkupLogTextName = "logText";

        const string MarkupInputName = "input";

        const int MaxLines = 30;

        const float CloseButtonWidth = 128f;

        ScriptVm _vm = new ScriptVm ();

        MarkupContainer _markup;

        InputField _inputField;

        Text _logText;

        RectTransform _logScroll;

        bool _isVisible;

        string[] _logLines = new string[MaxLines];

        int _linesCount;

        int _onDevConsoleId;

        protected override void OnConstruct () {
            base.OnConstruct ();
            _vm.ShowLineInfo (false);
            OnRegisterFunctions (_vm);
            _onDevConsoleId = "DeveloperConsole".GetHashCode ();
            _markup = MarkupContainer.CreateMarkup (MarkupSchema);
            _markup.AttachTheme (Resources.Load<MarkupTheme> (MarkupTheme));
            _markup.CreateVisuals ();
            _markup.GetCanvas ().sortingOrder = short.MaxValue;
            _markup.gameObject.SetActive (false);

            _logScroll = _markup.GetNamedNode (MarkupScrollViewName).GetComponent<ScrollRect> ().content;
            _logScroll.pivot = new Vector2 (0.5f, 0f);
            _logText = _markup.GetNamedNode (MarkupLogTextName).GetComponent<Text> ();
            _logText.horizontalOverflow = HorizontalWrapMode.Overflow;
            _logText.verticalOverflow = VerticalWrapMode.Overflow;

            var inputTr = _markup.GetNamedNode (MarkupInputName);
            var offset = inputTr.offsetMax;
            offset.x = -CloseButtonWidth;
            inputTr.offsetMax = offset;
            _inputField = inputTr.GetComponent<InputField> ();

            Singleton.Get<UnityEventBus> ().Subscribe<UiInputEndActionData> (OnInputEnd);
            Singleton.Get<UnityEventBus> ().Subscribe<UiClickActionData> (OnClose);
        }

        bool OnInputEnd (UiInputEndActionData arg) {
            if (arg.GroupId == _onDevConsoleId && Input.GetButton ("Submit")) {
                ExecuteCommand (arg.Value);
                _inputField.text = "";
                _inputField.ActivateInputField ();
            }
            return false;
        }

        bool OnClose (UiClickActionData arg) {
            Show (false);
            return false;
        }

        void ExecuteCommand (string value) {
            if (!string.IsNullOrEmpty (value)) {
                var err = _vm.Load (string.Format ("function _devConsoleMain(){{return {0};}}", value));
                if (!string.IsNullOrEmpty (err)) {
                    AppendLine (LogType.Warning, err);
                    return;
                }
                ScriptVar result;
                err = _vm.CallFunction ("_devConsoleMain", out result);
                if (!string.IsNullOrEmpty (err)) {
                    AppendLine (LogType.Warning, err);
                    return;
                }
                AppendLine (LogType.Log, result.AsString);
            }
        }

        protected void AppendLine (LogType type, string line) {
            if (string.IsNullOrEmpty (line)) {
                return;
            }
            if (line.IndexOf ('\n') != -1) {
                var lines = line.Split ('\n');
                for (int i = 0; i < lines.Length; i++) {
                    AppendLine (type, lines[i]);
                }
                return;
            }
            if (type != LogType.Log) {
                line = string.Format ("> <color=\"{0}\">{1}</color>", type == LogType.Warning ? "yellow" : "red", line);
            } else {
                line = string.Format ("> {0}", line);
            }
            if (_linesCount == MaxLines - 1) {
                _linesCount--;
                System.Array.Copy (_logLines, 1, _logLines, 0, _linesCount);
            }
            _logLines[_linesCount++] = line;
            _logText.text = string.Join ("\n", _logLines, 0, _linesCount);
            var size = _logScroll.sizeDelta;
            size.y = _logText.preferredHeight;
            _logScroll.sizeDelta = size;
        }

        /// <summary>
        /// Can be overrided for custom functions processing at console.
        /// </summary>
        /// <param name="vm">Script engine instance.</param>
        protected virtual void OnRegisterFunctions (ScriptVm vm) {
            vm.RegisterHostFunction ("version", OnVersion);
        }

        ScriptVar OnVersion (ScriptVm vm) {
            return new ScriptVar (Application.version);
        }

        /// <summary>
        /// Show / hide console.
        /// </summary>
        /// <param name="state"></param>
        public void Show (bool state) {
            if (state != _isVisible) {
                _isVisible = state;
                _markup.gameObject.SetActive (_isVisible);
                if (_isVisible) {
                    _inputField.ActivateInputField ();
                }
            }
        }
    }
}