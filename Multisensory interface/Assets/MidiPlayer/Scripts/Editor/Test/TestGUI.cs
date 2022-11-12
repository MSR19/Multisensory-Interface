using System;
using System.Collections.Generic;
using System.IO;
using MPTK.NAudio.Midi;
//using MEC;
namespace MidiPlayerTK
{
    using MEC;
    //using MonoProjectOptim;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;


    // ensure class initializer is called whenever scripts recompile
    [ExecuteInEditMode, InitializeOnLoadAttribute]
    public class TestGUI : EditorWindow
    {
        private static TestGUI window;

        int currentPage;
        bool followEvent;
        int displayTime;
        int displayOther;
        int displayOne;

        // SerializeField is used to ensure the view state is written to the window 
        // layout file. This means that the state survives restarting Unity as long as the window
        // is not closed. If the attribute is omitted then the state is still serialized/deserialized.
        [SerializeField] TreeViewState m_TreeViewState;

        //The TreeView is not serializable, so it should be reconstructed from the tree data.
        //SimpleTreeView m_SimpleTreeView;

        MPTKGui.PopupList popupCommand;
        MPTKGui.PopupList popupMulti;
        MPTKGui.PopupList popupMultiShort;
        List<MPTKGui.StyleItem> ItemsCommand;
        List<MPTKGui.StyleItem> ItemsHuge;
        List<MPTKGui.StyleItem> ItemShort;

        AnimationCurve curveX = AnimationCurve.EaseInOut(0, -10, 1, 10);
        bool isPlaying;

        //[MenuItem("Tools/Editor GUI &T", false, 5)]
        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            window = CreateWindow<TestGUI>("Editor Icons");
            window.ShowUtility();
            //window.titleContent = new GUIContent("Test");
            window.minSize = new Vector2(300, 600);
        }


        private void OnEnable()
        {
            Debug.Log($"OnEnable");
            ItemsCommand = new List<MPTKGui.StyleItem>();
            ItemsCommand.Add(new MPTKGui.StyleItem("Note On", true));
            ItemsCommand.Add(new MPTKGui.StyleItem("Note Off", true));
            ItemsCommand.Add(new MPTKGui.StyleItem("Control Change", true));
            ItemsCommand.Add(new MPTKGui.StyleItem("Patch Change", true));
            ItemsCommand.Add(new MPTKGui.StyleItem("Meta", true));
            ItemsCommand.Add(new MPTKGui.StyleItem("Touch", true));
            ItemsCommand.Add(new MPTKGui.StyleItem("Others", true));

            ItemShort = new List<MPTKGui.StyleItem>();
            ItemShort.Add(new MPTKGui.StyleItem("xxxxx", false));
            ItemShort.Add(new MPTKGui.StyleItem("yyyyyyyyyyyyyyy", true));

            ItemsHuge = new List<MPTKGui.StyleItem>();
            ItemsHuge.Add(new MPTKGui.StyleItem("xxxxx", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("yyyyyyy", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("zzz", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("xxxxx", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("yyyyyyy", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("zzz", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("xxxxx", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("yyyyyyy", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("zzz", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("xxxxx", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("yyyyyyy", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("zzz", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("xxxxx", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("yyyyyyy", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("zzz", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("zzz", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("xxxxx", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("yyyyyyy", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("zzz", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("xxxxx", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("yyyyyyy", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("zzz", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("xxxxx", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("yyyyyyy", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("zzz", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("xxxxx", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("yyyyyyy", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("zzz", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("zzz", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("xxxxx", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("yyyyyyy", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("zzz", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("xxxxx", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("yyyyyyy", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("zzz", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("xxxxx", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("yyyyyyy", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("zzz", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("xxxxx", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("yyyyyyy", true));
            ItemsHuge.Add(new MPTKGui.StyleItem("zzz", true));
        }

        public IEnumerator<float> ThreadCorePlay()
        {
            while (isPlaying)
            {
                Debug.Log($" {Routine.LocalTime} {curveX.Evaluate(Routine.LocalTime)} ");
                yield return 0;
            }
        }

        void OnGUI()
        {
            try
            {
                MidiCommonEditor.LoadSkinAndStyle();

                GUILayout.BeginHorizontal();

                float alignHorizontal = 24;
                followEvent = GUILayout.Toggle(followEvent, "Follow", GUILayout.Height(alignHorizontal));

                //Rect lastEventDraw = Rect.zero;
                //if (Event.current.type == EventType.Repaint)
                //    lastEventDraw = GUILayoutUtility.GetLastRect();

                MPTKGui.ComboBox(ref popupCommand, "{Index} Display Mono: {Label}", ItemsCommand, displayOther,
                    delegate (int index)
                    {
                        Debug.Log($"Action {index}");
                        foreach (MPTKGui.StyleItem item in ItemsCommand)
                            Debug.Log($"{item.Caption} {item.Visible} {item.Selected}");
                    });

                MPTKGui.ComboBox(ref popupMulti, "Display Huge - {Count}", ItemsHuge, -1,
                    delegate (int index)
                    {
                        Debug.Log($"Action {index}");
                        foreach (MPTKGui.StyleItem item in ItemsHuge)
                            Debug.Log($"{item.Caption} {item.Visible} {item.Selected}");
                    });

                MPTKGui.ComboBox(ref popupMultiShort, "Short {*}", ItemShort, -1,
                    delegate (int index)
                    {
                        Debug.Log($"Action {index}");
                        foreach (MPTKGui.StyleItem item in ItemShort)
                            Debug.Log($"{item.Caption} {item.Visible} {item.Selected}");
                    },
                    null, 200f);

                if (GUILayout.Button("aaa")) currentPage = 0;
                if (GUILayout.Button("bbb")) currentPage = 0;
                if (GUILayout.Button(MPTKGui.IconFirst)) currentPage = 0;
                if (GUILayout.Button(MPTKGui.IconPrevious)) currentPage--;
                GUILayout.Label($"Page {currentPage} / ?", MidiCommonEditor.styleLabelCenter, GUILayout.Height(alignHorizontal));
                if (GUILayout.Button(MPTKGui.IconNext)) currentPage++;
                if (GUILayout.Button(MPTKGui.IconLast)) currentPage = 100;

                GUILayout.EndHorizontal();

                GUILayout.Space(40);
                GUILayout.BeginHorizontal();

                // https://docs.unity3d.com/ScriptReference/EditorGUI.CurveField.html
                curveX = EditorGUI.CurveField(GUILayoutUtility.GetRect(300, 100), "Animation on X", curveX);
                curveX.postWrapMode = WrapMode.PingPong;

                if (GUILayout.Button("Generate Curve"))
                    if (Selection.activeGameObject)
                    {
                        //    // Get GameObject selected in hierarchy
                        //    FollowAnimationCurve comp = Selection.activeGameObject.GetComponent<FollowAnimationCurve>();
                        //    comp.SetCurves(curveX);
                    }
                bool changePlaying = GUILayout.Toggle(isPlaying, "Play");
                if (isPlaying != changePlaying)
                {
                    isPlaying = changePlaying;
                    if (isPlaying)
                        Routine.RunCoroutine(ThreadCorePlay(), Application.isPlaying ? Segment.RealtimeUpdate : Segment.EditorUpdate);

                }
                GUILayout.EndHorizontal();

                //m_SimpleTreeView.OnGUI(new Rect(0, 50, position.width, position.height));
            }
            catch (ExitGUIException ex)
            {
                Debug.Log(ex.Message);

            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                //MidiPlayerGlobal.ErrorDetail(ex);
            }
        }
    }

    // FollowAnimationCurve.cs
    // using UnityEngine; 
    // using System.Collections;

    // namespace MidiPlayerTK
    //    {

    //        public class FollowAnimationCurve : MonoBehaviour
    //        {
    //            public AnimationCurve curveX;

    //            public void SetCurves(AnimationCurve xC)
    //            {
    //                curveX = xC;
    //            }

    //            void Update()
    //            {
    //                transform.position = new Vector3(curveX.Evaluate(Time.time), transform.position.y, transform.position.z);
    //            }
    //        }
    //    }


    //    public MultiColumnTreeView(TreeViewState state,
    //                        MultiColumnHeader multicolumnHeader,
    //                        TreeModel<MyTreeElement> model)
    //                        : base(state, multicolumnHeader, model)
    //    {
    //        // Custom setup
    //        rowHeight = 20;
    //        columnIndexForTreeFoldouts = 2;
    //        showAlternatingRowBackgrounds = true;
    //        showBorder = true;
    //        customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f;
    //        extraSpaceBeforeIconAndLabel = kToggleWidth;
    //        multicolumnHeader.sortingChanged += OnSortingChanged;

    //        Reload();
    //    }

    //}
    //class SimpleTreeView : TreeView
    //{
    //    public SimpleTreeView(TreeViewState treeViewState)
    //        : base(treeViewState)
    //    {
    //        Reload();
    //    }

    //    protected override TreeViewItem BuildRoot()
    //    {
    //        // BuildRoot is called every time Reload is called to ensure that TreeViewItems 
    //        // are created from data. Here we create a fixed set of items. In a real world example,
    //        // a data model should be passed into the TreeView and the items created from the model.

    //        // This section illustrates that IDs should be unique. The root item is required to 
    //        // have a depth of -1, and the rest of the items increment from that.
    //        var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
    //        var allItems = new List<TreeViewItem>
    //    {
    //        new TreeViewItem {id = 1, depth = 0, displayName = "Animals"},
    //        new TreeViewItem {id = 2, depth = 1, displayName = "Mammals"},
    //        new TreeViewItem {id = 3, depth = 2, displayName = "Tiger"},
    //        new TreeViewItem {id = 4, depth = 2, displayName = "Elephant"},
    //        new TreeViewItem {id = 5, depth = 2, displayName = "Okapi"},
    //        new TreeViewItem {id = 6, depth = 2, displayName = "Armadillo"},
    //        new TreeViewItem {id = 7, depth = 1, displayName = "Reptiles"},
    //        new TreeViewItem {id = 8, depth = 2, displayName = "Crocodile"},
    //        new TreeViewItem {id = 9, depth = 2, displayName = "Lizard"},
    //    };

    //        // Utility method that initializes the TreeViewItem.children and .parent for all items.
    //        SetupParentsAndChildrenFromDepths(root, allItems);

    //        // Return root of the tree
    //        return root;
    //    }
    //}
    //[Serializable]
    ////The TreeElement data class is extended to hold extra data, which you can show and edit in the front-end TreeView.
    //internal class MyTreeElement : TreeElement
    //{
    //    public float floatValue1, floatValue2, floatValue3;
    //    public Material material;
    //    public string text = "";
    //    public bool enabled = true;

    //    public MyTreeElement(string name, int depth, int id) : base(name, depth, id)
    //    {
    //        floatValue1 = Random.value;
    //        floatValue2 = Random.value;
    //        floatValue3 = Random.value;
    //    }
    //}
    //[CreateAssetMenu(fileName = "TreeDataAsset", menuName = "Tree Asset", order = 1)]
    //public class MyTreeAsset : ScriptableObject
    //{
    //    [SerializeField] List<MyTreeElement> m_TreeElements = new List<MyTreeElement>();

    //    internal List<MyTreeElement> treeElements
    //    {
    //        get { return m_TreeElements; }
    //        set { m_TreeElements = value; }
    //    }
    //}

}