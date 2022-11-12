using System;
using System.Collections.Generic;
using System.IO;
using MPTK.NAudio.Midi;
namespace MidiPlayerTK
{
    //using MonoProjectOptim;
    using UnityEditor;
    using UnityEditor.Compilation;
    using UnityEngine;

    /// <summary>@brief
    /// Window editor for the setup of MPTK
    /// </summary>
    [ExecuteInEditMode, InitializeOnLoadAttribute]
    public partial class MidiFileSetupWindow : EditorWindow
    {
        private MidiEditorLib MidiPlayerEditor;

        private static MidiFileSetupWindow window;

        static Vector2 scrollPosMidiFile = Vector2.zero;
        static Vector2 scrollPosAnalyze = Vector2.zero;

        static float widthLeft = 500;
        static float widthRight;

        static float heightList;
        static float titleHeight = 18; //label title above list

        static int itemHeight = 25;
        const int BUTTON_WIDTH = 150;
        const int BUTTON_SHORT_WIDTH = 50;
        const int BUTTON_HEIGHT = 18;
        const float ESPACE = 5;

        static float xpostitlebox = 2;
        static float ypostitlebox = 5;

        static public CustomStyle myStyle;

        private bool autoPlay = false;

        List<MPTKGui.StyleItem> ColumnFiles;
        private static Rect listMidiVisibleRect;


        // % (ctrl on Windows, cmd on macOS), # (shift), & (alt).
        [MenuItem("MPTK/Midi File Setup &M", false, 10)]
        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            try
            {
                //if (window != null)
                {
                    //window = ScriptableObject.CreateInstance(typeof(MidiFileSetupWindow)) as MidiFileSetupWindow;
                    window = GetWindow<MidiFileSetupWindow>(true, "Midi File Setup");
                    if (window == null) return;
                    window.minSize = new Vector2(828, 100);
                    window.Show();
                    window.titleContent = new GUIContent("Midi File Setup");
                    //Debug.Log($"Init {window.position} name:{window.name}");
                }
            }
            catch (Exception /*ex*/)
            {
                //MidiPlayerGlobal.ErrorDetail(ex);
            }
        }

        private void Awake()
        {
            //Debug.Log($"Awake");
            CompilationPipeline.compilationStarted += CompileStarted;
            MidiPlayerEditor = new MidiEditorLib("MidiEditorPlayer");
            IndexEditItem = -1;
            InitPlayer();
        }

        private void CompileStarted(object obj)
        {
            // Don't appreciate recompilation when window is open
            Close(); // call OnDestroy
        }

        private void OnEnable()
        {
            //Debug.Log($"OnEnable");
            EditorApplication.playModeStateChanged += LogPlayModeState;
        }
        private void LogPlayModeState(PlayModeStateChange state)
        {
            //Debug.Log(">>> LogPlayModeState MidiSequencerWindow" + state);
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                Close(); // call OnDestroy
            }
            //Debug.Log("<<< LogPlayModeState MidiSequencerWindow" + state);
        }

        //        private void OnLostFocus()
        //        {
        //#if UNITY_2017_1_OR_NEWER
        //            // Trig an  error before v2017...
        //            if (Application.isPlaying)
        //            {
        //                window.Close();
        //            }
        //#endif
        //        }

        void OnDestroy()
        {
            EditorApplication.playModeStateChanged -= LogPlayModeState;
            if (MidiPlayerEditor != null) //strangely, this property can be null when window is close
                MidiPlayerEditor.DestroyMidiObject();
            //else
            //    Debug.LogWarning("MidiPlayerEditor is null");
        }

        private void OnFocus()
        {
            // Load description of available soundfont
            try
            {
                //Debug.Log("OnFocus");
                Init();
                MidiPlayerGlobal.InitPath();
                ToolsEditor.LoadMidiSet();
                ToolsEditor.CheckMidiSet();
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }

        void OnGUI()
        {
            try
            {
                if (window == null)
                {
                    Init();
                }
                MidiCommonEditor.LoadSkinAndStyle();
                float startx = 5;
                float starty = 7;
                //Log.Write("test");

                //if (myStyle == null)                    myStyle = new CustomStyle();
                GUI.Box(new Rect(0, 0, window.position.width, window.position.height), "", MidiCommonEditor.styleWindow);

                GUIContent content = new GUIContent() { text = "Setup Midi files - Version " + ToolsEditor.version, tooltip = "" };
                EditorGUI.LabelField(new Rect(startx, starty, 500, itemHeight), content, MidiCommonEditor.styleBold);

                content = new GUIContent() { text = "Doc & Contact", tooltip = "Get some help" };
                Rect rect = new Rect(window.position.size.x - BUTTON_WIDTH - 5, starty, BUTTON_WIDTH, BUTTON_HEIGHT);
                if (GUI.Button(rect, content))
                    PopupWindow.Show(rect, new AboutMPTK());

                starty += BUTTON_HEIGHT + ESPACE;

                widthRight = window.position.size.x - widthLeft - 2 * ESPACE - startx;
                heightList = window.position.size.y - 3 * ESPACE - starty;

                ShowListMidiFiles(startx, starty, widthLeft, heightList);
                if (!autoPlay)
                    ShowMidiAnalyse(startx + widthLeft + ESPACE, starty, widthRight, heightList);
                else
                    ShowMidiPlayer(startx + widthLeft + ESPACE, starty, widthRight, heightList);

            }
            catch (ExitGUIException) { }
            catch (Exception /*ex*/)
            {
                //         MidiPlayerGlobal.ErrorDetail(ex);
            }
        }

        /// <summary>@brief
        /// Display, add, remove Midi file
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        private void ShowListMidiFiles(float startX, float startY, float width, float height)
        {
            try
            {
                Event e = Event.current;
                if (e.type == EventType.KeyDown && IndexEditItem >= 0)
                {
                    //Debug.Log("Ev.KeyDown: " + e);
                    if (e.keyCode == KeyCode.Space || e.keyCode == KeyCode.DownArrow || e.keyCode == KeyCode.UpArrow || e.keyCode == KeyCode.End || e.keyCode == KeyCode.Home)
                    {
                        int selected_index = IndexEditItem;
                        if (e.keyCode == KeyCode.Space)
                        {
                            autoPlay = !autoPlay;
                            if (!autoPlay)
                                MidiPlayerEditor.MidiPlayer.MPTK_Stop();
                            else
                                PlayMidiFileSelected(IndexEditItem);
                        }
                        if (e.keyCode == KeyCode.End)
                            selected_index = MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count - 1;

                        if (e.keyCode == KeyCode.Home)
                            selected_index = 0;

                        if (e.keyCode == KeyCode.DownArrow)
                        {
                            selected_index++;
                            if (selected_index >= MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count)
                                selected_index = 0;
                        }

                        if (e.keyCode == KeyCode.UpArrow)
                        {
                            selected_index--;
                            if (selected_index < 0)
                                selected_index = MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count - 1;
                        }

                        if (selected_index != -1 && selected_index != IndexEditItem)
                        {
                            IndexEditItem = selected_index;
                            if (autoPlay)
                                PlayMidiFileSelected(IndexEditItem);
                        }
                        SetMidiSelectedVisible();
                        ReadEvents();
                        GUI.changed = true;
                        Repaint();
                    }
                }
                if (ColumnFiles == null)
                {
                    ColumnFiles = new List<MPTKGui.StyleItem>();
                    ColumnFiles.Add(new MPTKGui.StyleItem() { Width = 60, Caption = "Index", Offset = 0f });
                    ColumnFiles.Add(new MPTKGui.StyleItem() { Width = 406, Caption = "Midi Name", Offset = -1f });
                    //ColumnFiles.Add(new ToolsGUI.DefineColumn() { Width = 70, Caption = "Read", PositionCaption = 0f });
                    //ColumnFiles.Add(new ToolsGUI.DefineColumn() { Width = 60, Caption = "Remove", PositionCaption = -9f });
                }

                GUI.Box(new Rect(startX, startY, width, height), "", MidiCommonEditor.stylePanel);

                float localstartX = 0;
                float localstartY = 0;

                GUIContent content = new GUIContent()
                {
                    text = MidiPlayerGlobal.CurrentMidiSet.MidiFiles == null || MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count == 0 ?
                                        "No Midi file available" : "Midi files available",
                    tooltip = ""
                };

                localstartX += xpostitlebox;
                localstartY += ypostitlebox;
                GUI.Label(new Rect(startX + localstartX + 5, startY + localstartY, 160, titleHeight), content, MidiCommonEditor.styleBold);

                string searchMidi = EditorGUI.TextField(new Rect(startX + localstartX + 5 + 110 + ESPACE, startY + localstartY - 2, 200, titleHeight), "Search in list:");
                if (!string.IsNullOrEmpty(searchMidi))
                {
                    int index = MidiPlayerGlobal.CurrentMidiSet.MidiFiles.FindIndex(s => s.ToLower().Contains(searchMidi.ToLower()));
                    if (index >= 0)
                    {
                        IndexEditItem = index;
                        SetMidiSelectedVisible();
                        ReadEvents();
                    }
                }

                // Help
                if (GUI.Button(new Rect(startX + localstartX + 5 + 110 + 300 + ESPACE, startY + localstartY - 2, 70, BUTTON_HEIGHT), MPTKGui.IconHelp))
                    Application.OpenURL("https://paxstellar.fr/setup-mptk-add-midi-files-v2/");

                localstartY += titleHeight;

                // Bt remove
                if (IndexEditItem >= 0 && IndexEditItem < MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count)
                    if (GUI.Button(new Rect(startX + localstartX + width - 50, startY + localstartY - 1, 40, BUTTON_HEIGHT), new GUIContent(MPTKGui.IconDelete, $"Remove {MidiPlayerGlobal.CurrentMidiSet.MidiFiles[IndexEditItem]}")))
                    {
                        if (EditorUtility.DisplayDialog(
                            "Remove Midi File",
                            $"Remove {MidiPlayerGlobal.CurrentMidiSet.MidiFiles[IndexEditItem]} ?",
                            "ok", "cancel"))
                        {
                            DeleteResource(MidiLoad.BuildOSPath(MidiPlayerGlobal.CurrentMidiSet.MidiFiles[IndexEditItem]));
                            AssetDatabase.Refresh();
                            ToolsEditor.LoadMidiSet();
                            ToolsEditor.CheckMidiSet();
                            AssetDatabase.Refresh();
                        }
                    }

                float btWidth = 100;
                float posx = startX + localstartX + ESPACE;
                float posy = startY + localstartY;
                if (GUI.Button(new Rect(posx, posy, btWidth, BUTTON_HEIGHT), "Add a Midi File"))
                    AddMidifile();
                posx += btWidth + ESPACE;

                btWidth = 120;
                if (GUI.Button(new Rect(posx, posy, btWidth, BUTTON_HEIGHT), "Add From Folder"))
                    AddMidiFromFolder();
                posx += btWidth + ESPACE;

                btWidth = 100;
                if (GUI.Button(new Rect(posx, posy, btWidth, BUTTON_HEIGHT), "Open Folder"))
                    Application.OpenURL("file://" + PathToDBMidi());
                posx += btWidth + ESPACE;

                btWidth = 100;
                bool togglePlay = GUI.Toggle(new Rect(posx, posy, btWidth, BUTTON_HEIGHT), autoPlay, "Midi Auto Play", MidiCommonEditor.styleToggle);
                //Debug.Log(togglePlay);
                if (togglePlay != autoPlay)
                {
                    autoPlay = togglePlay;

                    if (!autoPlay)
                        MidiPlayerEditor.MidiPlayer.MPTK_Stop();
                    else
                        PlayMidiFileSelected(IndexEditItem);
                }
                //posx += btWidth + 6 * espace;


                localstartY += BUTTON_HEIGHT + ESPACE;

                // Draw title list box
                GUI.Box(new Rect(startX + localstartX + ESPACE, startY + localstartY, width - 35, itemHeight), "", MidiCommonEditor.styleListTitle);
                float boxX = startX + localstartX + ESPACE;
                foreach (MPTKGui.StyleItem column in ColumnFiles)
                {
                    GUI.Label(new Rect(boxX + column.Offset, startY + localstartY, column.Width, itemHeight), column.Caption, MidiCommonEditor.styleListTitle);
                    boxX += column.Width;
                }

                localstartY += itemHeight + ESPACE;

                if (MidiPlayerGlobal.CurrentMidiSet.MidiFiles != null)
                {
                    listMidiVisibleRect = new Rect(startX + localstartX, startY + localstartY - 6, width - 10, height - localstartY);
                    Rect listMidiContentRect = new Rect(0, 0, width - 35, MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count * itemHeight + 5);

                    scrollPosMidiFile = GUI.BeginScrollView(listMidiVisibleRect, scrollPosMidiFile, listMidiContentRect, false, true);
                    //Debug.Log($"scrollPosMidiFile:{scrollPosMidiFile.y} listVisibleRect:{listMidiVisibleRect.height} listContentRect:{listMidiContentRect.height}");
                    float boxY = 0;

                    // Loop on each midi
                    // -----------------
                    for (int i = 0; i < MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count; i++)
                    {
                        boxX = 5;

                        if (GUI.Button(new Rect(ESPACE, boxY, width - 35, itemHeight), MidiPlayerGlobal.CurrentMidiSet.MidiFiles[i], IndexEditItem == i ? MidiCommonEditor.styleListRowSelected : MidiCommonEditor.styleListRow))
                        {
                            IndexEditItem = i;
                            ReadEvents();
                            if (autoPlay)
                                PlayMidiFileSelected(IndexEditItem);

                        }

                        // col 0 - Index
                        float colw = ColumnFiles[0].Width;
                        EditorGUI.LabelField(new Rect(boxX, boxY + 0, colw, itemHeight - 0), i.ToString(), MidiCommonEditor.styleListRowCenter);
                        boxX += colw;

                        // col 1 - Name
                        //colw = columnSF[1].Width;
                        //content = new GUIContent() { text = MidiPlayerGlobal.CurrentMidiSet.MidiFiles[i], tooltip = MidiPlayerGlobal.CurrentMidiSet.MidiFiles[i] };
                        //EditorGUI.LabelField(new Rect(boxX + 5, boxY + 2, colw, itemHeight - 5), content, MidiCommonEditor.styleLabelLeft);
                        //boxX += colw;

                        // col 2 - Select
                        //colw = columnSF[2].Width;
                        //if (GUI.Button(new Rect(boxX, boxY + 3, 30, buttonHeight), new GUIContent(buttonIconView, "Read Midi events")))
                        //{
                        //    IndexEditItem = i;
                        //    ReadEvents();
                        //}
                        //boxX += colw;

                        // col 3 - remove
                        //colw = columnSF[2].Width;
                        //if (GUI.Button(new Rect(boxX, boxY + 3, 30, itemHeight), EditorTools.IconDelete))
                        //{
                        //    DeleteResource(MidiLoad.BuildOSPath(MidiPlayerGlobal.CurrentMidiSet.MidiFiles[i]));
                        //    AssetDatabase.Refresh();
                        //    ToolsEditor.LoadMidiSet();
                        //    ToolsEditor.CheckMidiSet();
                        //    AssetDatabase.Refresh();
                        //}
                        boxX += colw;

                        boxY += itemHeight - 1;
                    }
                    GUI.EndScrollView();
                }
            }
            catch (Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }

        /// <summary>@brief
        /// Add a new Midi file from desktop
        /// </summary>
        private static void AddMidifile()
        {
            try
            {
                string selectedFile = EditorUtility.OpenFilePanelWithFilters(
                    "Open and import Midi file", ToolsEditor.lastDirectoryMidi,
                    new string[] { "Midi files", "mid,midi", "Karoke files", "kar", "All", "*" });
                if (!string.IsNullOrEmpty(selectedFile))
                {
                    // selectedFile contins also the folder 
                    ToolsEditor.lastDirectoryMidi = Path.GetDirectoryName(selectedFile);
                    InsertMidiFIle(selectedFile);
                }
                AssetDatabase.Refresh();
                ToolsEditor.LoadMidiSet();
                ToolsEditor.CheckMidiSet();
                AssetDatabase.Refresh();
                ReadEvents();
            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }


        /// <summary>@brief
        /// Add Midi files from a folder
        /// </summary>
        private static void AddMidiFromFolder()
        {
            try
            {
                string selectedFolder = EditorUtility.OpenFolderPanel("Import Midi from a folder", ToolsEditor.lastDirectoryMidi, "");
                if (!string.IsNullOrEmpty(selectedFolder))
                {
                    ToolsEditor.lastDirectoryMidi = Path.GetDirectoryName(selectedFolder);
                    string[] files = Directory.GetFiles(selectedFolder);
                    foreach (string file in files)
                        if (file.EndsWith(".mid") || file.EndsWith(".midi"))
                            InsertMidiFIle(file);
                }
                AssetDatabase.Refresh();
                ToolsEditor.LoadMidiSet();
                ToolsEditor.CheckMidiSet();
                AssetDatabase.Refresh();
            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }

        private static string PathToDBMidi()
        {
            // Build path to midi folder 
            string pathMidiFile = Path.Combine(Application.dataPath, MidiPlayerGlobal.PathToMidiFile);

            if (!Directory.Exists(pathMidiFile))
                Directory.CreateDirectory(pathMidiFile);
            return pathMidiFile;
        }

        private static void InsertMidiFIle(string selectedFile)
        {
            // Build path to midi folder 
            string pathMidiFile = PathToDBMidi();

            MidiLoad midifile;
            try
            {
                midifile = new MidiLoad();

                bool ok = true;
                using (Stream sfFile = new FileStream(selectedFile, FileMode.Open, FileAccess.Read))
                {
                    byte[] data = new byte[sfFile.Length];
                    sfFile.Read(data, 0, (int)sfFile.Length);
                    ok = midifile.MPTK_Load(data, false);
                }

                if (!ok)
                {
                    EditorUtility.DisplayDialog("Midi Not Loaded", "Try to open " + selectedFile + "\nbut this file seems not a valid midi file", "ok");
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarningFormat("{0} {1}", selectedFile, ex.Message);
                return;
            }

            string filename = Path.GetFileNameWithoutExtension(selectedFile);
            //foreach (char c in filename) Debug.Log(string.Format("{0} {1}", c, (int)c));
            foreach (char i in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(i, '_');
            }
            string filenameToSave = Path.Combine(pathMidiFile, filename + MidiPlayerGlobal.ExtensionMidiFile);

            filenameToSave = filenameToSave.Replace('(', '_');
            filenameToSave = filenameToSave.Replace(')', '_');
            filenameToSave = filenameToSave.Replace('#', '_');
            filenameToSave = filenameToSave.Replace('$', '_');

            // Create a copy of the midi file in MPTK resources
            File.Copy(selectedFile, filenameToSave, true);

            if (MidiPlayerGlobal.CurrentMidiSet.MidiFiles == null)
                MidiPlayerGlobal.CurrentMidiSet.MidiFiles = new List<string>();

            // Add midi file to the list
            string midiname = Path.GetFileNameWithoutExtension(selectedFile);
            if (MidiPlayerGlobal.CurrentMidiSet.MidiFiles.FindIndex(s => s == midiname) < 0)
            {
                MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Add(midiname);
                MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Sort();
                MidiPlayerGlobal.CurrentMidiSet.Save();
            }
            IndexEditItem = MidiPlayerGlobal.CurrentMidiSet.MidiFiles.FindIndex(s => s == midiname);

            SetMidiSelectedVisible();

            Debug.Log($"Midi file '{midiname}' added with success, " +
                      $"Index: {IndexEditItem}, " +
                      $"Duration: {midifile.MPTK_DurationMS / 1000f} second, " +
                      $"Track count:{midifile.MPTK_TrackCount}, " +
                      $"Initial Tempo:{midifile.MPTK_InitialTempo}"
                      );
        }

        private static void SetMidiSelectedVisible()
        {
            if (MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count > 0)
            {
                if (IndexEditItem >= 0 && IndexEditItem < MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count)
                {
                    float contentHeight = MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count * itemHeight;
                    scrollPosMidiFile.y = contentHeight *
                        ((float)IndexEditItem / (float)MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count) - listMidiVisibleRect.height / 2f;
                }
            }
        }

        static private void DeleteResource(string filepath)
        {
            try
            {
                Debug.Log("Delete " + filepath);
                File.Delete(filepath);
                // delete also meta
                string meta = filepath + ".meta";
                Debug.Log("Delete " + meta);
                File.Delete(meta);

            }
            catch (Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }
    }

}