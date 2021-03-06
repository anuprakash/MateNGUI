﻿using UnityEngine;

namespace M8.NGUI {
    /// <summary>
    /// Simple script that lets you localize a UIWidget using GameLocalize.
    /// </summary>

    [RequireComponent(typeof(UIWidget))]
    [AddComponentMenu("M8/NGUI/Localize")]
    public class GameLocalize : MonoBehaviour {
        /// <summary>
        /// Localization key.
        /// </summary>
        public string key;

        private bool mStarted = false;
        private object[] mParams = null;

        /// <summary>
        /// Set the parameters of the text for use with string format. If this is not null, string.Format is used.
        /// Make sure to call Localize again after changes to params
        /// </summary>
        public object[] parameters { get { return mParams; } set { mParams = value; } }

        /// <summary>
        /// Localize the widget on enable, but only if it has been started already.
        /// </summary>

        void OnEnable() { Localize.instance.localizeCallback += Apply; if(mStarted) Apply(); }
        void OnDisable() { if(Localize.instantiated) Localize.instance.localizeCallback -= Apply; }

        /// <summary>
        /// Localize the widget on start.
        /// </summary>

        void Start() {
            mStarted = true;
            Apply();
        }

        /// <summary>
        /// Force-localize the widget.
        /// </summary>

        public void Apply() {
            UIWidget w = GetComponent<UIWidget>();
            UILabel lbl = w as UILabel;
            UISprite sp = w as UISprite;

            // If no localization key has been specified, use the label's text as the key
            if(string.IsNullOrEmpty(key) && lbl != null) key = lbl.text;

            // If we still don't have a key, use the widget's name
            string val = string.IsNullOrEmpty(key) ? Localize.instance.GetText(w.name) : Localize.instance.GetText(key);

            if(lbl != null) {
                if(mParams != null)
                    val = string.Format(val, mParams);

                // If this is a label used by input, we should localize its default value instead
                UIInput input = NGUITools.FindInParents<UIInput>(lbl.gameObject);
                if(input != null && input.label == lbl) input.defaultText = val;
                else lbl.text = val;
            }
            else if(sp != null) {
                sp.spriteName = val;
                sp.MakePixelPerfect();
            }
        }
    }
}