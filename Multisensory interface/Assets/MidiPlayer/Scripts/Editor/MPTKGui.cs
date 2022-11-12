//#define MPTK_PRO
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MidiPlayerTK
{
    public class MPTKGui
    {
        // https://github.com/halak/unity-editor-icons
        // https://github.com/nukadelic/UnityEditorIcons
        static private Texture iconComboBox; static public Texture IconComboBox { get { if (iconComboBox == null) iconComboBox = EditorGUIUtility.IconContent("d_icon dropdown").image; return iconComboBox; } }
        static private Texture iconFirst; static public Texture IconFirst { get { if (iconFirst == null) iconFirst = EditorGUIUtility.IconContent("d_Animation.FirstKey").image; return iconFirst; } }
        static private Texture iconPrevious; static public Texture IconPrevious { get { if (iconPrevious == null) iconPrevious = EditorGUIUtility.IconContent("d_Animation.PrevKey").image; return iconPrevious; } }
        static private Texture iconNext; static public Texture IconNext { get { if (iconNext == null) iconNext = EditorGUIUtility.IconContent("d_Animation.NextKey").image; return iconNext; } }
        static private Texture iconLast; static public Texture IconLast { get { if (iconLast == null) iconLast = EditorGUIUtility.IconContent("d_Animation.LastKey").image; return iconLast; } }
        static private Texture iconHelp; static public Texture IconHelp { get { if (iconHelp == null) iconHelp = Resources.Load<Texture2D>("Textures/question-mark"); return iconHelp; } }
        static private Texture iconEye; static public Texture IconEye { get { if (iconEye == null) iconEye = Resources.Load<Texture2D>("Textures/Eye"); return iconEye; } }
        static private Texture iconSave; static public Texture IconSave { get { if (iconSave == null) iconSave = Resources.Load<Texture2D>("Textures/Save_24x24"); return iconSave; } }
        static private Texture iconFolders; static public Texture IconFolders { get { if (iconFolders == null) iconFolders = EditorGUIUtility.IconContent("Folder On Icon").image; return iconFolders; } }
        static private Texture iconDelete; static public Texture IconDelete { get { if (iconDelete == null) iconDelete = Resources.Load<Texture2D>("Textures/Delete_32x32"); return iconDelete; } }
        static private Texture iconClose; static public Texture IconClose { get { if (iconClose == null) iconClose = EditorGUIUtility.IconContent("winbtn_win_close").image; return iconClose; } }


        public static Color ButtonColor = new Color(.7f, .9f, .7f, 1f);

        public static GUIStyle LabelListPlayed { get { return GUI.skin.GetStyle("LabelListPlayed"); } }
        public static GUIStyle LabelListSelected { get { return GUI.skin.GetStyle("LabelListSelected"); } }
        public static GUIStyle LabelListNormal { get { return GUI.skin.GetStyle("LabelListNormal"); } }
        public static GUIStyle ButtonCombo { get { return GUI.skin.GetStyle("ButtonCombo"); } }
        public static GUIStyle ButtonHighLight { get { return GUI.skin.GetStyle("ButtonHighLight"); } }
        public static GUIStyle button { get { return GUI.skin.GetStyle("button"); } }
        public static GUIStyle LabelGray { get { return GUI.skin.GetStyle("LabelGray"); } }


        public static GUIStyle BuildStyle(GUIStyle inheritedStyle = null, int fontSize = 10, bool wrapText = false,
                                        FontStyle fontStyle = FontStyle.Normal, TextAnchor anchor = TextAnchor.MiddleLeft)
        {
            GUIStyle style = inheritedStyle == null ? new GUIStyle() : new GUIStyle(inheritedStyle);
            style.alignment = anchor;
            style.fontSize = fontSize;
            style.fontStyle = fontStyle;
            style.wordWrap = wrapText;
            style.clipping = TextClipping.Overflow;
            return style;
        }
        public static GUIStyle ColorStyle(GUIStyle style, Color fontColor, Texture2D backColor = null)
        {
            style.focused.textColor = fontColor;
            style.normal.textColor = fontColor;
            style.focused.background = backColor != null ? backColor : style.onNormal.background;
            return style;

        }
        public static GUIStyle LabelBoldCentered
        {
            get
            {
                if (labelBoldCentered == null)
                {
                    labelBoldCentered = new GUIStyle(GUI.skin.GetStyle("label"));
                    labelBoldCentered.wordWrap = true;
                    labelBoldCentered.fontStyle = FontStyle.Bold;
                    labelBoldCentered.alignment = TextAnchor.MiddleCenter;
                }
                return labelBoldCentered;
            }
        }
        static GUIStyle labelBoldCentered;

        static public void ComboBox(ref PopupList p_popup, string title, List<StyleItem> items, int selectedIndex, Action<int> action,
            GUIStyle style = null, float widthPopup = 0, params GUILayoutOption[] option)
        {
            //Debug.Log(Event.current);
            if (p_popup == null)
            {
                //Debug.Log("BuildPopup popupLoadType");
                p_popup = new PopupList("", items, selectedIndex < 0);
                p_popup.SelectedIndex = selectedIndex;
                p_popup.OnSelect = action;
            }


            if (selectedIndex >= 0)
            {
                // Mono selection
                title = title.Replace("{Label}", p_popup.SelectedLabel).Replace("{Index}", p_popup.SelectedIndex.ToString());
            }
            else
            {
                // Multi selection
                if (title.Contains("{Count}"))
                {
                    string count = $"{p_popup.SelectedCount}/{p_popup.TotalCount}";
                    title = title.Replace("{Count}", count);
                }
                if (title.Contains("{*}"))
                {
                    title = title.Replace("{*}", p_popup.SelectedCount != p_popup.TotalCount ? "*" : "");
                }
            }

            if (style == null)
                // Style for the combo button
                style = MPTKGui.ButtonCombo;
            //else
            //    Debug.Log($"ComboBox style.contentOffset {title} {style.contentOffset}");

            GUILayout.Label(new GUIContent(title, MPTKGui.IconComboBox), style, option);

            if (Event.current.type == EventType.Repaint)
            {
                p_popup.RectPopup = GUILayoutUtility.GetLastRect();
                if (widthPopup != 0)
                    p_popup.RectPopup.width = widthPopup;
                //Debug.Log($"GetLastRect {title} {p_popup.RectActivation}");
                p_popup.RectPopup.x += style.contentOffset.x;
            }
            if (Event.current.type == EventType.MouseDown)
            {
                //Debug.Log($"MouseDown style.contentOffset {title} {style.contentOffset}");
                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition - style.contentOffset))
                {
                    //Debug.Log($"Show PopupWindow {p_popup.RectActivation}");
                    try { PopupWindow.Show(p_popup.RectPopup, p_popup); }
                    catch (ExitGUIException) { } // Unity bug ?
                }
            }
        }

        public class StyleItem
        {
            public string Caption;
            public bool Visible;
            public bool Selected;
            public float Width;
            public float Offset;
            public string Tooltip;
            public bool Hidden;
            public GUIStyle Style;
            /// <summary>
            /// If defined, a popup filter is displayed to filter the list
            /// </summary>
            public PopupList ItemPopup;
            public float ItemPopupWidth;
            private List<StyleItem> itemPopupContent;
            public List<StyleItem> ItemPopupContent
            {
                get => itemPopupContent;
                set { itemPopupContent = value; }
            }
            public StyleItem()
            {
                Visible = true;
                Style = MPTKGui.LabelListNormal;
            }

            public StyleItem(string label, bool visible = true, bool selected = false, GUIStyle style = null)
            {
                Caption = label;
                Visible = visible;
                Selected = selected;
                Style = style == null ? MPTKGui.LabelListNormal : style;
            }

        }

        public static Texture2D SetColor(Texture2D tex2, Color32 color)
        {
            var fillColorArray = tex2.GetPixels32();
            for (var i = 0; i < fillColorArray.Length; ++i)
                fillColorArray[i] = color;
            tex2.SetPixels32(fillColorArray);
            tex2.Apply();
            return tex2;
        }

        public static Texture2D MakeTex(int width, int height, Color textureColor, RectOffset border, Color bordercolor)
        {
            int widthInner = width;
            width += border.left;
            width += border.right;

            Color[] pix = new Color[width * (height + border.top + border.bottom)];

            for (int i = 0; i < pix.Length; i++)
            {
                if (i < (border.bottom * width))
                    pix[i] = bordercolor;
                else if (i >= ((border.bottom * width) + (height * width)))  //Border Top
                    pix[i] = bordercolor;
                else
                { //Center of Texture

                    if ((i % width) < border.left) // Border left
                        pix[i] = bordercolor;
                    else if ((i % width) >= (border.left + widthInner)) //Border right
                        pix[i] = bordercolor;
                    else
                        pix[i] = textureColor;    //Color texture
                }
            }

            Texture2D result = new Texture2D(width, height + border.top + border.bottom);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        public class PopupList : PopupWindowContent
        {
            public int SelectedIndex
            {
                get => selectedIndex;
                set
                {
                    selectedIndex = value;
                    selectedLabel = listItem != null && value >= 0 && value < listItem.Count ? listItem[value].Caption : "";
                }
            }

            public Rect RectPopup;
            public string SelectedLabel { get => selectedLabel; }
            public int SelectedCount { get => selectedCount; }
            public int TotalCount { get => totalCount; }
            public bool MultiSelection { get => multiSelection; }

            public Action<int> OnSelect;

            private Vector2 scroller;
            private List<StyleItem> listItem;
            private GUIStyle styleLabel;
            private GUIStyle styleboldLabel;
            private int selectedCount;
            private int totalCount;
            private int selectedIndex;
            private string selectedLabel;
            private bool multiSelection;


            public override Vector2 GetWindowSize()
            {
                float winHeight, winWidth;
                winHeight = listItem.Count * (EditorStyles.label.lineHeight + 2f) + 2f;
                if (MultiSelection)
                    winHeight += EditorStyles.miniButtonMid.fixedHeight + 4f;
                //Debug.Log($"EditorStyles.miniButtonMid.fixedHeight={EditorStyles.miniButtonMid.fixedHeight} lineHeight:{EditorStyles.miniButtonMid.lineHeight}");
                //Debug.Log($"EditorStyles.label.fixedHeight={ EditorStyles.label.fixedHeight} lineHeight:{EditorStyles.label.lineHeight}");
                //Debug.Log($"EditorStyles.toggle.fixedHeight={EditorStyles.toggle.fixedHeight} lineHeight:{EditorStyles.toggle.lineHeight}");
                //Debug.Log($"EditorStyles.boldLabel.fixedHeight={EditorStyles.boldLabel.fixedHeight} lineHeight:{EditorStyles.boldLabel.lineHeight}");
                winWidth = RectPopup.width;
                //Debug.Log($"GetWindowSize {winWidth} {winHeight} {Data.Count} {MultiSelection}");
                return new Vector2(winWidth, winHeight);
            }

            public PopupList(string title, List<MPTKGui.StyleItem> listItem, bool multiSelect = false)
            {
                multiSelection = multiSelect;
                this.listItem = listItem;
                styleLabel = EditorStyles.label;
                styleboldLabel = EditorStyles.boldLabel;
                Count();
            }

            void Count()
            {
                selectedCount = 0;
                foreach (MPTKGui.StyleItem item in listItem) if (item.Selected) selectedCount++;
                totalCount = listItem.Count;
            }

            public override void OnGUI(Rect rect)
            {
                try
                {
                    if (MultiSelection)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("All", EditorStyles.miniButtonMid))
                        {
                            foreach (MPTKGui.StyleItem item in listItem)
                                item.Selected = true;
                            ChangeSelection(-1);
                        }
                        if (GUILayout.Button("None", EditorStyles.miniButtonMid))
                        {
                            foreach (MPTKGui.StyleItem item in listItem)
                                item.Selected = false;
                            ChangeSelection(-2);
                        }
                        GUILayout.Space(15);
                        if (GUILayout.Button(MPTKGui.IconClose, EditorStyles.miniButtonMid))
                            editorWindow.Close();
                        //if (Event.current.type == EventType.Repaint) Debug.Log($"Button {GUILayoutUtility.GetLastRect()}");
                        GUILayout.EndHorizontal();
                    }

                    scroller = GUILayout.BeginScrollView(scroller, false, false);


                    for (int index = 0; index < listItem.Count; index++)
                    {
                        MPTKGui.StyleItem item = listItem[index];
                        if (MultiSelection)
                        {
                            bool select = GUILayout.Toggle(item.Selected, item.Caption);
                            //if (Event.current.type == EventType.Repaint) Debug.Log($"Toggle {GUILayoutUtility.GetLastRect()}");
                            if (select != item.Selected)
                            {
                                item.Selected = select;
                                ChangeSelection(index);
                            }
                        }
                        else
                        {
                            GUIStyle styleRow = index == SelectedIndex && !MultiSelection ? styleboldLabel : styleLabel;
                            GUILayout.Label(item.Caption, styleRow);

                            //if (Event.current.type == EventType.Repaint) Debug.Log($"Label {GUILayoutUtility.GetLastRect()}");
                            if (Event.current.type == EventType.MouseDown)
                                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                                {
                                    ChangeSelection(index);
                                }
                        }
                    }
                    GUILayout.EndScrollView();
                }
                catch (System.Exception ex)
                {
                    MidiPlayerGlobal.ErrorDetail(ex);
                }
            }

            private void ChangeSelection(int index)
            {
                SelectedIndex = index; // update also SelectedLabel
                Count();
                //Debug.Log($"Selected {SelectedIndex} '{SelectedLabel}'");
                if (OnSelect != null) OnSelect(index);
                if (!MultiSelection)
                    editorWindow.Close();
            }
        }
    }
}
