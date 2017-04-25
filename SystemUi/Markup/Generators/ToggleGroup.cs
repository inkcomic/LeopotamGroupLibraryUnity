// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Math;
using LeopotamGroup.Serialization;
using LeopotamGroup.SystemUi.Widgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LeopotamGroup.SystemUi.Markup.Generators {
    static class ToggleGroupNode {
        static readonly int HashedEmptyCheck = "emptyCheck".GetStableHashCode ();

        /// <summary>
        /// Create "toggleGroup" node. If children supported - GameObject container for them should be returned.
        /// </summary>
        /// <param name="go">Gameobject holder.</param>
        /// <param name="node">Xml node.</param>
        /// <param name="container">Markup container.</param>
        public static GameObject Create (GameObject go, XmlNode node, MarkupContainer container) {
#if UNITY_EDITOR
            go.name = "toggleGroup";
#endif
            var checkGroup = go.AddComponent<ToggleGroup> ();

            var attrValue = node.GetAttribute (HashedEmptyCheck);
            if (string.CompareOrdinal (attrValue, "true") == 0) {
                checkGroup.allowSwitchOff = true;
            }

            MarkupUtils.SetSize (go, node);
            MarkupUtils.SetRotation (go, node);
            MarkupUtils.SetOffset (go, node);
            MarkupUtils.SetMask (go, node);
            MarkupUtils.SetHidden (go, node);

            return go;
        }
    }
}