//#define MPTK_PRO
using System;
using System.Collections.Generic;
using MPTK.NAudio.Midi;
namespace MidiPlayerTK
{
    //using MonoProjectOptim;
    using UnityEditor;
    using UnityEngine;

    /// <summary>@brief
    /// Window editor for the setup of MPTK
    /// </summary>
    public class SoundFontSetupWindow : EditorWindow
    {
        private static SoundFontSetupWindow window;

        Vector2 scrollPosBanks = Vector2.zero;
        Vector2 scrollPosSoundFont = Vector2.zero;

        static float widthLeft = 500 + 30;
        static float widthRight; // calculated

        static float heightLeftTop = 300;
        static float heightRightTop = 400;
        static float heightLeftBottom;  // calculated
        static float heightRightBottom; // calculated

        static float itemHeight = 25;
        static float titleHeight = 18; //label title above list
        static float buttonLargeWidth = 180;
        static float buttonMediumWidth = 60;
        static float buttonHeight = 18;
        static float espace = 5;

        static float xpostitlebox = 2;
        static float ypostitlebox = 5;



        public static BuilderInfo LogInfo;
#if MPTK_PRO
        Vector2 scrollPosOptim = Vector2.zero;
#endif
        static public bool KeepAllPatchs = false;
        static public bool KeepAllZones = false;
        static public bool RemoveUnusedWaves = false;
        static public bool LogDetailSoundFont = false;

        List<MPTKGui.StyleItem> columnSF;
        List<MPTKGui.StyleItem> columnBank;

        static int LoadType;
        static int CompressionFormat;
        static MPTKGui.PopupList popupLoadType;
        static MPTKGui.PopupList popupCompressionFormat;

        static List<MPTKGui.StyleItem> listLoadType;
        static  List<MPTKGui.StyleItem> listCompressionFormat;


        // % (ctrl on Windows, cmd on macOS), # (shift), & (alt).
        [MenuItem("MPTK/SoundFont Setup &F", false, 11)]
        public static void Init()
        {
            //Debug.Log("init");
            // Get existing open window or if none, make a new one:
            try
            {
                window = GetWindow<SoundFontSetupWindow>(true, "SoundFont Setup");
                if (window == null) return;
                window.minSize = new Vector2(989, 568);
                LoadType = 1;
                CompressionFormat = 0;
                //Debug.Log(window.position);
            }
            catch (System.Exception/* ex*/)
            {
                //MidiPlayerGlobal.ErrorDetail(ex); 
            }
            //Debug.Log("end init");

        }

        /// <summary>@brief
        /// Reload data
        /// </summary>
        private void OnFocus()
        {
            // Load description of available soundfont
            try
            {
                //Init();

                MidiPlayerGlobal.InitPath();

                //Debug.Log(MidiPlayerGlobal.ImSFCurrent == null ? "ImSFCurrent is null" : "ImSFCurrent:" + MidiPlayerGlobal.ImSFCurrent.SoundFontName);
                //Debug.Log(MidiPlayerGlobal.CurrentMidiSet == null ? "CurrentMidiSet is null" : "CurrentMidiSet" + MidiPlayerGlobal.CurrentMidiSet.ActiveSounFontInfo.Name);
                //Debug.Log(MidiPlayerGlobal.CurrentMidiSet.ActiveSounFontInfo == null ? "ActiveSounFontInfo is null" : MidiPlayerGlobal.CurrentMidiSet.ActiveSounFontInfo.Name);
                ToolsEditor.LoadMidiSet();
                ToolsEditor.CheckMidiSet();
                // cause catch if call when playing (setup open on run mode)
                try
                {
                    if (!Application.isPlaying)
                        AssetDatabase.Refresh();
                }
                catch (Exception)
                {
                }
                // Exec after Refresh, either cause errror
                if (MidiPlayerGlobal.ImSFCurrent == null)
                    MidiPlayerGlobal.LoadCurrentSF();
                //BuildPopup();

            }
            catch (Exception /*ex*/)
            {
                //MidiPlayerGlobal.ErrorDetail(ex);
            }
        }

        void OnGUI()
        {
            try
            {
                if (window == null)
                    Init();

                if (LogInfo == null) LogInfo = new BuilderInfo();
                MidiCommonEditor.LoadSkinAndStyle();

                float startx = 5;
                float starty = 7;

                GUI.Box(new Rect(0, 0, window.position.width, window.position.height), "", MidiCommonEditor.styleWindow);

                GUIContent content = new GUIContent() { text = "Setup SoundFont - Version " + ToolsEditor.version, tooltip = "" };
                EditorGUI.LabelField(new Rect(startx, starty, 500, itemHeight), content, MidiCommonEditor.styleBold);

                content = new GUIContent() { text = "Doc & Contact", tooltip = "Get some help" };

                // Set position of the button
                Rect rect = new Rect(window.position.size.x - buttonLargeWidth - 5, starty, buttonLargeWidth, buttonHeight);
                if (GUI.Button(rect, content))
                    PopupWindow.Show(rect, new AboutMPTK());

                starty += buttonHeight + espace;

                widthRight = window.position.size.x - widthLeft - 2 * espace - startx;
                //widthRight = window.position.size.x / 2f - espace;
                //widthLeft = window.position.size.x / 2f - espace;

                heightLeftBottom = window.position.size.y - heightLeftTop - 3 * espace - starty;
                heightRightBottom = window.position.size.y - heightRightTop - 3 * espace - starty;

                // Display list of soundfont already loaded 
                ShowListSoundFonts(startx, starty, widthLeft, heightLeftTop);

                ShowListBanks(startx + widthLeft + espace, starty, widthRight, heightRightTop);

                ShowExtractOptim(startx + widthLeft + espace, starty + heightRightTop + espace, widthRight, heightRightBottom + espace);

                ShowLogOptim(startx, starty + espace + heightLeftTop, widthLeft, heightLeftBottom + espace);
            }
            catch (ExitGUIException) { }
            catch (Exception /*ex*/)
            {
                //MidiPlayerGlobal.ErrorDetail(ex);   
            }
        }


        /// <summary>@brief
        /// Display, add, remove Soundfont
        /// </summary>
        /// <param name="localstartX"></param>
        /// <param name="localstartY"></param>
        private void ShowListSoundFonts(float startX, float startY, float width, float height)
        {
            try 
            {
                if (columnSF == null)
                {
                    columnSF = new List<MPTKGui.StyleItem>();
                    columnSF.Add(new MPTKGui.StyleItem() { Width = 215, Caption = "SoundFont Name", Offset = 1f });
                    columnSF.Add(new MPTKGui.StyleItem() { Width = 40, Caption = "Patch", Offset = 6f });
                    columnSF.Add(new MPTKGui.StyleItem() { Width = 45, Caption = "Wave", Offset = 15f });
                    columnSF.Add(new MPTKGui.StyleItem() { Width = 60, Caption = "Size", Offset = 25f });
                    columnSF.Add(new MPTKGui.StyleItem() { Width = 85, Caption = "SoundFont", Offset = 5f });
                    columnSF.Add(new MPTKGui.StyleItem() { Width = 50, Caption = "Remove", Offset = -7f });
                }

                Rect zone = new Rect(startX, startY, width, height);
                //GUI.color = new Color(.8f, .8f, .8f, 1f);
                GUI.Box(zone, "", MidiCommonEditor.stylePanel);
                GUI.color = Color.white;
                float localstartX = 0;
                float localstartY = 0;
                GUIContent content;
                if (MidiPlayerGlobal.CurrentMidiSet != null && MidiPlayerGlobal.CurrentMidiSet.SoundFonts != null && MidiPlayerGlobal.CurrentMidiSet.SoundFonts.Count > 0)
                    content = new GUIContent() { text = "SoundFont available", tooltip = "Each SoundFonts contains a set of bank of sound. \nOnly one SoundFont can be active at the same time for the midi player" };
                else
                {
                    content = new GUIContent() { text = "No SoundFont found, click on button 'Add SoundFont'", tooltip = "See the documentation here https://paxstellar.fr/" };
                    MidiPlayerGlobal.ImSFCurrent = null;
                }
                localstartX += xpostitlebox;
                localstartY += ypostitlebox;
                EditorGUI.LabelField(new Rect(startX + localstartX + 5, startY + localstartY, width, titleHeight), content, MidiCommonEditor.styleBold);
                localstartY += titleHeight;

                if (GUI.Button(new Rect(startX + localstartX + espace, startY + localstartY, buttonLargeWidth, buttonHeight), "Download SoundFonts"))
                    Application.OpenURL("https://www.polyphone-soundfonts.com/download-soundfonts");

#if MPTK_PRO
                if (GUI.Button(new Rect(startX + localstartX + 2 * espace + buttonLargeWidth, startY + localstartY, buttonLargeWidth, buttonHeight), "Add SoundFont"))
                {
                    if (Application.isPlaying)
                        EditorUtility.DisplayDialog("Add a SoundFont", "This action is not possible when application is running.", "Ok");
                    else
                    {
                        //if (EditorUtility.DisplayDialog("Import SoundFont", "This action could take time, do you confirm ?", "Ok", "Cancel"))
                        {
                            this.AddSoundFont();
                            LoadType = popupLoadType.SelectedIndex = MidiPlayerGlobal.ImSFCurrent.LoadType;
                            CompressionFormat = popupCompressionFormat.SelectedIndex = MidiPlayerGlobal.ImSFCurrent.CompressionFormat;
                            scrollPosSoundFont = Vector2.zero;
                        }
                    }
                }
#else
                if (GUI.Button(new Rect(startX + localstartX + 2 * espace + buttonLargeWidth, startY + localstartY, buttonLargeWidth, buttonHeight), "Add a SoundFont [PRO]"))
                    PopupWindow.Show(new Rect(startX + localstartX, startY + localstartY, buttonLargeWidth, buttonHeight), new GetFullVersion());
#endif
                if (GUI.Button(new Rect(startX + localstartX + width - 65, startY + localstartY - 18, 35, 35), MPTKGui.IconHelp))
                {
                    //CreateWave createwave = new CreateWave();
                    //string path = System.IO.Path.Combine(MidiPlayerGlobal.MPTK_PathToResources, "unitySample") + ".wav";
                    ////string path = "unitySample.wav";
                    //HiSample sample = new HiSample();
                    ////sample.LoopStart = sample.LoopEnd = 0;
                    //byte[] data = new byte[10000];
                    //for (int i = 0; i < data.Length; i++) data[i] = (byte)255;
                    //sample.SampleRate = 44100;
                    //sample.End = (uint)data.Length/2;
                    //createwave.Build(path, sample, data);
                    Application.OpenURL("https://paxstellar.fr/setup-mptk-add-soundfonts-v2/");
                }

                localstartY += buttonHeight + espace;

                // Draw title list box
                GUI.Box(new Rect(startX + localstartX + espace, startY + localstartY, width - 35, itemHeight), "", MidiCommonEditor.styleListTitle);
                float boxX = startX + localstartX + espace;
                foreach (MPTKGui.StyleItem column in columnSF)
                {
                    GUI.Label(new Rect(boxX + column.Offset, startY + localstartY, column.Width, itemHeight), column.Caption, MidiCommonEditor.styleLabelLeft);
                    boxX += column.Width;
                }

                localstartY += itemHeight + espace;

                if (MidiPlayerGlobal.CurrentMidiSet != null && MidiPlayerGlobal.CurrentMidiSet.SoundFonts != null && MidiPlayerGlobal.CurrentMidiSet.SoundFonts.Count > 0)
                {

                    Rect listVisibleRect = new Rect(startX + localstartX, startY + localstartY - 6, width - 10, height - localstartY);
                    Rect listContentRect = new Rect(0, 0, width - 35, MidiPlayerGlobal.CurrentMidiSet.SoundFonts.Count * itemHeight + 5);

                    scrollPosSoundFont = GUI.BeginScrollView(listVisibleRect, scrollPosSoundFont, listContentRect, false, false);
                    float boxY = 0;

                    // Loop on each soundfont
                    for (int i = 0; i < MidiPlayerGlobal.CurrentMidiSet.SoundFonts.Count; i++)
                    {
                        SoundFontInfo sf = MidiPlayerGlobal.CurrentMidiSet.SoundFonts[i];
                        bool selected = (MidiPlayerGlobal.ImSFCurrent != null && sf.Name == MidiPlayerGlobal.CurrentMidiSet.ActiveSounFontInfo.Name);

                        // Draw a row
                        GUI.Box(new Rect(espace, boxY, width - 35, itemHeight), "", selected ? MidiCommonEditor.styleListRowSelected : MidiCommonEditor.styleListRow);

                        // Start content position (from the visible rect)
                        boxX = espace;

                        // col 1 - name
                        float colw = columnSF[0].Width;
                        EditorGUI.LabelField(new Rect(boxX + 1, boxY + 2, colw, itemHeight - 5), sf.Name, MidiCommonEditor.styleLabelLeft);
                        boxX += colw;

                        // col 2 - patch count
                        colw = columnSF[1].Width;
                        EditorGUI.LabelField(new Rect(boxX, boxY + 3, colw, itemHeight - 7), sf.PatchCount.ToString(), MidiCommonEditor.styleLabelRight);
                        boxX += colw;

                        // col 3 - wave count
                        colw = columnSF[2].Width;
                        EditorGUI.LabelField(new Rect(boxX, boxY + 3, colw, itemHeight - 7), sf.WaveCount.ToString(), MidiCommonEditor.styleLabelRight);
                        boxX += colw;

                        // col 4 - size
                        colw = columnSF[3].Width;
                        string sizew = (sf.WaveSize < 1000000) ?
                             Math.Round((double)sf.WaveSize / 1000d).ToString() + " Ko" :
                             Math.Round((double)sf.WaveSize / 1000000d).ToString() + " Mo";
                        EditorGUI.LabelField(new Rect(boxX, boxY + 3, colw, itemHeight - 7), sizew, MidiCommonEditor.styleLabelRight);
                        boxX += colw;

                        string textselect = "Select";
                        if (MidiPlayerGlobal.ImSFCurrent != null && sf.Name == MidiPlayerGlobal.CurrentMidiSet.ActiveSounFontInfo.Name)
                            textselect = "Default";// GUI.color = ToolsEditor.ButtonColor;

                        // col 5 - select and remove buttons
                        colw = columnSF[4].Width;
                        boxX += 10;
                        if (GUI.Button(new Rect(boxX, boxY + 3, buttonMediumWidth, buttonHeight), textselect))
                        {
#if MPTK_PRO
                            this.SelectSf(i);
                            if (MidiPlayerGlobal.ImSFCurrent != null)
                            {
                                LoadType = popupLoadType.SelectedIndex = MidiPlayerGlobal.ImSFCurrent.LoadType;
                                CompressionFormat = popupCompressionFormat.SelectedIndex = MidiPlayerGlobal.ImSFCurrent.CompressionFormat;
                            }
                            LogInfo = new BuilderInfo();
#else
                            PopupWindow.Show(new Rect(boxX, boxY + 3, buttonMediumWidth, buttonHeight), new GetFullVersion());
#endif
                        }
                        boxX += colw;

                        colw = columnSF[5].Width;
                        if (GUI.Button(new Rect(boxX, boxY + 3, 30, buttonHeight), new GUIContent(MPTKGui.IconDelete, "Remove SoundFont and Samples associated")))
                        {
#if MPTK_PRO
                            if (Application.isPlaying)
                                EditorUtility.DisplayDialog("Remove a SoundFont", "This action is not possible when application is running.", "Ok");
                            else
                            {
                                if (this.DeleteSf(i))
                                {
                                    if (MidiPlayerGlobal.CurrentMidiSet.SoundFonts.Count > 0)
                                    {
                                        this.SelectSf(0);
                                        LoadType = popupLoadType.SelectedIndex = MidiPlayerGlobal.ImSFCurrent.LoadType;
                                        CompressionFormat = popupCompressionFormat.SelectedIndex = MidiPlayerGlobal.ImSFCurrent.CompressionFormat;
                                    }
                                    else
                                        MidiPlayerGlobal.ImSFCurrent = null;
                                }
                            }
#else
                            PopupWindow.Show(new Rect(boxX, boxY + 3, 30, buttonHeight), new GetFullVersion());
#endif
                        }
                        boxX += colw;

                        boxY += itemHeight - 1;

                    }
                    GUI.EndScrollView();
                }
            }
            catch (ExitGUIException) { }
            catch (Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex); 
            }
        }

        private void ShowListBanks(float startX, float startY, float width, float height)
        {
            try
            {
                if (columnBank == null)
                {
                    columnBank = new List<MPTKGui.StyleItem>();
                    columnBank.Add(new MPTKGui.StyleItem() { Width = 150, Caption = "Bank number", Offset = 1f });
                    columnBank.Add(new MPTKGui.StyleItem() { Width = 60,Caption = "View",Offset = -3f });
                    columnBank.Add(new MPTKGui.StyleItem() { Width = 80,Caption = "Keep",Offset = -10f });
                    columnBank.Add(new MPTKGui.StyleItem() { Width = 77,Caption = "Instrument",Offset = -25f });
                    columnBank.Add(new MPTKGui.StyleItem() { Width = 77,Caption = "Drum",Offset = -6f });
                }

                Rect zone = new Rect(startX, startY, width, height);
                //GUI.color = new Color(.8f, .8f, .8f, 1f);
                GUI.Box(zone, "", MidiCommonEditor.stylePanel);
                //GUI.color = Color.white;
                float localstartX = 0;
                float localstartY = 0;
                if (MidiPlayerGlobal.ImSFCurrent != null && MidiPlayerGlobal.ImSFCurrent.Banks != null)
                {
                    GUIContent content = new GUIContent() { text = "Banks available in SoundFont " + MidiPlayerGlobal.ImSFCurrent.SoundFontName, tooltip = "Each bank contains a set of patchs (instrument).\nOnly two banks can be active at the same time : default sound (piano, ...) and drum kit (percussive)" };
                    localstartX += xpostitlebox;
                    localstartY += ypostitlebox;
                    EditorGUI.LabelField(new Rect(startX + localstartX + 5, startY + localstartY, width, titleHeight), content, MidiCommonEditor.styleBold);
                    localstartY += titleHeight;

                    // Save selection of banks
                    float btw = 25;
                    if (GUI.Button(new Rect(startX + localstartX + espace, startY + localstartY, btw, buttonHeight), new GUIContent(MPTKGui.IconSave, "Save banks configuration")))
                    {
#if MPTK_PRO
                        if (Application.isPlaying)
                            EditorUtility.DisplayDialog("Save Bank Configuration", "This action is not possible when application is running.", "Ok");
                        else
                            SaveBanksConfig();
#endif
                    }

                    btw = 75;
                    float buttonX = startX + localstartX + btw + 4 * espace;
                    EditorGUI.LabelField(new Rect(buttonX, startY + localstartY, btw, buttonHeight), "Keep banks:", MidiCommonEditor.styleLabelLeft);
                    buttonX += btw;

                    if (GUI.Button(new Rect(buttonX, startY + localstartY, btw, buttonHeight), new GUIContent("All", "Select all banks to be kept in the SoundFont")))
                    {
                        if (MidiPlayerGlobal.ImSFCurrent != null) MidiPlayerGlobal.ImSFCurrent.SelectAllBanks();
                    }
                    buttonX += btw + espace;

                    if (GUI.Button(new Rect(buttonX, startY + localstartY, btw, buttonHeight), new GUIContent("None", "Unselect all banks to be kept in the SoundFont")))
                    {
                        if (MidiPlayerGlobal.ImSFCurrent != null) MidiPlayerGlobal.ImSFCurrent.UnSelectAllBanks();
                    }
                    buttonX += btw + espace;

                    if (GUI.Button(new Rect(buttonX, startY + localstartY, btw, buttonHeight), new GUIContent("Inverse", "Inverse selection of banks to be kept in the SoundFont")))
                    {
                        if (MidiPlayerGlobal.ImSFCurrent != null) MidiPlayerGlobal.ImSFCurrent.InverseSelectedBanks();
                    }
                    buttonX += btw + espace;

                    localstartY += buttonHeight + espace;

                    // Draw title list box
                    GUI.Box(new Rect(startX + localstartX + espace, startY + localstartY, width - 35, itemHeight), "", MidiCommonEditor.styleListTitle);
                    float boxX = startX + localstartX + espace;
                    foreach (MPTKGui.StyleItem column in columnBank)
                    {
                        GUI.Label(new Rect(boxX + column.Offset, startY + localstartY, column.Width, itemHeight), column.Caption, MidiCommonEditor.styleLabelLeft);
                        boxX += column.Width;
                    }

                    localstartY += itemHeight + espace;

                    // Count available banks
                    int countBank = 0;
                    foreach (ImBank bank in MidiPlayerGlobal.ImSFCurrent.Banks)
                        if (bank != null) countBank++;
                    Rect listVisibleRect = new Rect(startX + localstartX, startY + localstartY - 6, width - 10, height - localstartY);
                    Rect listContentRect = new Rect(0, 0, width - 35, countBank * itemHeight + 5);

                    scrollPosBanks = GUI.BeginScrollView(listVisibleRect, scrollPosBanks, listContentRect, false, false);

                    float boxY = 0;
                    if (MidiPlayerGlobal.ImSFCurrent != null)
                    {
                        foreach (ImBank bank in MidiPlayerGlobal.ImSFCurrent.Banks)
                        {
                            if (bank != null)
                            {
                                GUI.Box(new Rect(5, boxY, width - 35, itemHeight), "", MidiCommonEditor.styleListRow);

                                GUI.color = Color.white;

                                // Start content position (from the visible rect)
                                boxX = espace;

                                // col 0 - bank and patch count
                                float colw = columnBank[0].Width;
                                GUI.Label(new Rect(boxX + 1, boxY, colw, itemHeight), string.Format("Bank [{0,3:000}] Patch:{1,4}", bank.BankNumber, bank.PatchCount), MidiCommonEditor.styleLabelLeft);
                                boxX += colw;

                                // col 1 - bt view list of patchs
                                colw = columnBank[1].Width;
                                Rect btrect = new Rect(boxX, boxY + 3, 30, buttonHeight);
                                if (GUI.Button(btrect, new GUIContent(MPTKGui.IconEye, "See the detail of this bank")))
                                    PopupWindow.Show(btrect, new PopupListPatchs("Patch", false, bank.GetDescription()));
                                boxX += colw;

                                // col 2 - select bank to keep
                                colw = columnBank[2].Width;
                                Rect rect = new Rect(boxX, boxY + 4, colw, buttonHeight);
                                bool newSelect = GUI.Toggle(rect, MidiPlayerGlobal.ImSFCurrent.BankSelected[bank.BankNumber], new GUIContent("", "Keep or remove this bank"), MidiCommonEditor.styleToggle);
                                if (newSelect != MidiPlayerGlobal.ImSFCurrent.BankSelected[bank.BankNumber])
                                {
#if MPTK_PRO
                                    MidiPlayerGlobal.ImSFCurrent.BankSelected[bank.BankNumber] = newSelect;
#else
                                    PopupWindow.Show(rect, new GetFullVersion());
#endif
                                }
                                boxX += colw;

                                // col 3 - set default bank for instrument
                                colw = columnBank[3].Width;
                                bool curSelect = MidiPlayerGlobal.ImSFCurrent.DefaultBankNumber == bank.BankNumber;
                                newSelect = GUI.Toggle(new Rect(boxX, boxY + 4, colw, buttonHeight), curSelect, new GUIContent("", "Select this bank as default for playing all instruments except drum"), MidiCommonEditor.styleToggle);
                                if (newSelect != curSelect)
                                    MidiPlayerGlobal.ImSFCurrent.DefaultBankNumber = newSelect ? bank.BankNumber : -1;
                                boxX += btw + espace;

                                // col 4 - set default bank for Drum
                                colw = columnBank[4].Width;
                                curSelect = MidiPlayerGlobal.ImSFCurrent.DrumKitBankNumber == bank.BankNumber;
                                newSelect = GUI.Toggle(new Rect(boxX, boxY + 4, colw, buttonHeight), curSelect, new GUIContent("", "Select this bank as default for playing drum hit (Channel=9)"), MidiCommonEditor.styleToggle);
                                if (newSelect != curSelect)
                                    MidiPlayerGlobal.ImSFCurrent.DrumKitBankNumber = newSelect ? bank.BankNumber : -1;
                                boxX += btw + espace;

                                boxY += itemHeight - 1;
                            }
                        }
                    }

                    GUI.EndScrollView();
                }
                else
                    EditorGUI.LabelField(new Rect(startX + xpostitlebox, startY + ypostitlebox, 300, itemHeight), "No SoundFont selected", MidiCommonEditor.styleBold);
            }
            catch (ExitGUIException) { }
            catch (Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }

#if MPTK_PRO
        private void SaveTypeBank()
        {
            this.SetTypeBank(LoadType, CompressionFormat);
            this.SaveCurrentIMSF(true);
            AssetDatabase.Refresh();
        }

        private bool SaveBanksConfig()
        {
            string infocheck = this.CheckAndSetBank();
            if (string.IsNullOrEmpty(infocheck))
            {
                // Save MPTK SoundFont : xml only
                this.SaveCurrentIMSF(true);
                AssetDatabase.Refresh();
                return true;
            }
            else
                EditorUtility.DisplayDialog("Save Selected Bank", infocheck, "Ok");
            return false;
        }
#endif

        /// <summary>@brief
        /// Display optimization
        /// </summary>
        /// <param name="localstartX"></param>
        /// <param name="localstartY"></param>
        private void ShowExtractOptim(float localstartX, float localstartY, float width, float height)
        {
            try
            {
                // Begin area display settings
                // --------------------------
                GUILayout.BeginArea(new Rect(localstartX, localstartY, width, height), MidiCommonEditor.stylePanel);

                //Rect zone = new Rect(localstartX, localstartY, width, height);
                ////GUI.color = new Color(.8f, .8f, .8f, 1f);
                //GUI.Box(zone, "", MidiCommonEditor.stylePanel);
                ////GUI.color = Color.white;
                //int lineHeight = 35;
                string tooltip = "Remove all banks and Presets not used in the Midi file list";

                GUIContent content;
                if (MidiPlayerGlobal.ImSFCurrent != null)
                {
                    content = new GUIContent() { text = $"Extract Patchs & Waves from the SoundFont '{MidiPlayerGlobal.ImSFCurrent.SoundFontName}'", tooltip = "" };
                    GUILayout.Label(content, MidiCommonEditor.styleBold);

                    GUILayout.Space(20);

                    GUILayout.BeginHorizontal();
                    RemoveUnusedWaves = GUILayout.Toggle(RemoveUnusedWaves,
                        new GUIContent("Remove unused waves", "If check, keep only waves used by your midi files and/or in the selected banks"), MidiCommonEditor.styleToggle);
                    LogDetailSoundFont = GUILayout.Toggle(LogDetailSoundFont,
                        new GUIContent("Log SoundFont Detail", "If check, keep only waves used by your midi files"), MidiCommonEditor.styleToggle);
                    GUILayout.EndHorizontal();

                    GUILayout.Space(20);

                    GUILayout.BeginHorizontal();

                    if (listLoadType == null)
                    {
                        listLoadType = new List<MPTKGui.StyleItem>();
                        foreach (string loadType in Enum.GetNames(typeof(AudioClipLoadType)))
                            listLoadType.Add(new MPTKGui.StyleItem(loadType, true));
                    }
                    MPTKGui.ComboBox(ref popupLoadType, "Audio Load Type: {Label}", listLoadType, LoadType, delegate (int index) { LoadType = index; });

                    if (listCompressionFormat == null)
                    {
                        listCompressionFormat = new List<MPTKGui.StyleItem>();
                        foreach (string loadType in Enum.GetNames(typeof(AudioCompressionFormat)))
                            listCompressionFormat.Add(new MPTKGui.StyleItem(loadType, true));
                    }

                    MPTKGui.ComboBox(ref popupCompressionFormat, "Audio Compression Format: {Label}", listCompressionFormat, CompressionFormat, delegate (int index) { CompressionFormat = index; });
                    GUILayout.EndHorizontal();

                    content = new GUIContent() { text = "Setup recommended for Maestro: 'Compressed In Memory' and 'PCM'", tooltip = "However, feel free to check other format for specific devices" };
                    GUILayout.Label(content);

                    if (CompressionFormat != MidiPlayerGlobal.ImSFCurrent.CompressionFormat || LoadType != MidiPlayerGlobal.ImSFCurrent.LoadType)
                        GUILayout.Label("You need to extract to apply the changes.", MidiCommonEditor.styleAlertRed);

                    GUILayout.Space(20);

                    GUILayout.Label("Extract Patches and Samples from the SoundFont:", MidiCommonEditor.styleBold);

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(new GUIContent("Extract Only Required", "Your list of Midi files will be scanned to identify patches and samples needed."), GUILayout.Height(35)))
                    {
                        if (Application.isPlaying)
                            EditorUtility.DisplayDialog("Optimization", "This action is not possible when application is running.", "Ok");
                        else
                        {
#if MPTK_PRO
                            if (SaveBanksConfig())
                            {
                                KeepAllPatchs = false;
                                KeepAllZones = false;
                                this.OptimizeSoundFont();// LogInfo, KeepAllPatchs, KeepAllZones, RemoveUnusedWaves);
                                SaveTypeBank();
                            }
#else
                            PopupWindow.Show(GUILayoutUtility.GetLastRect(), new GetFullVersion());
#endif
                        }
                    }

                    if (GUILayout.Button(new GUIContent("Extract All", "Extract all patchs and waves from the Soundfont for the selected banks."), GUILayout.Height(35)))
                    {
                        if (Application.isPlaying)
                            EditorUtility.DisplayDialog("Extraction", "This action is not possible when application is running.", "Ok");
                        else
                        {
#if MPTK_PRO
                            if (SaveBanksConfig())
                            {
                                KeepAllPatchs = true;
                                KeepAllZones = true;
                                this.OptimizeSoundFont();// (LogInfo, KeepAllPatchs, KeepAllZones, RemoveUnusedWaves);
                                SaveTypeBank();
                            }
#else
                            PopupWindow.Show(GUILayoutUtility.GetLastRect(), new GetFullVersion());
#endif
                        }
                    }
                    GUILayout.EndHorizontal();

                }
                else
                {
                    content = new GUIContent() { text = "No SoundFont selected", tooltip = tooltip };
                    GUILayout.Label(content, MidiCommonEditor.styleBold);
                }
                GUILayout.EndArea();

            }
            catch (ExitGUIException) { }
            catch (Exception ex)
            {
                //Debug.Log(ex.Message);
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }

        /// <summary>@brief
        /// Display optimization log
        /// </summary>
        /// <param name="localstartX"></param>
        /// <param name="localstartY"></param>
        private void ShowLogOptim(float localstartX, float localstartY, float width, float height)
        {
            try
            {
                Rect zone = new Rect(localstartX, localstartY, width, height);
                //GUI.color = new Color(.8f, .8f, .8f, 1f);
                GUI.Box(zone, "", MidiCommonEditor.stylePanel);
                //GUI.color = Color.white;
                float posx = localstartX;
                GUI.Label(new Rect(posx + espace, localstartY + espace, 40, buttonHeight), new GUIContent("Logs:"), MidiCommonEditor.styleBold);
                posx += 40;
                float btw = 32f;
                if (GUI.Button(new Rect(posx + espace, localstartY + espace, btw, btw), new GUIContent(MPTKGui.IconSave, "Save Log")))
                {
                    // Save log file
                    if (LogInfo != null)
                    {
                        string filenamelog = string.Format("SoundFontSetupLog {0} {1} .txt", MidiPlayerGlobal.ImSFCurrent.SoundFontName, DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss"));
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(System.IO.Path.Combine(Application.persistentDataPath, filenamelog)))
                            foreach (string line in LogInfo.Infos)
                                file.WriteLine(line);
                    }
                }

                if (GUI.Button(new Rect(posx + 2f * espace + btw, localstartY + espace, btw, btw), new GUIContent(MPTKGui.IconFolders, "Open Logs Folder")))
                {
                    Application.OpenURL("file://" + Application.persistentDataPath);
                }

                if (GUI.Button(new Rect(posx + 3f * espace + 2f * btw, localstartY + espace, btw, btw), new GUIContent(MPTKGui.IconDelete, "Clear Logs")))
                {
                    LogInfo = new BuilderInfo();
                }
#if MPTK_PRO
                bool displayAlert = MidiPlayerGlobal.CurrentMidiSet.ActiveSounFontInfo != null && MidiPlayerGlobal.CurrentMidiSet.ActiveSounFontInfo.PatchCount == 0;
                float heightLine = MidiCommonEditor.styleRichText.lineHeight;// * 1.2f;
                int countLine = (LogInfo != null ? LogInfo.Count : 0) + (displayAlert ? 7 : 0);
                if (countLine > 0)
                {
                    Rect listVisibleRect = new Rect(espace, localstartY + buttonHeight + espace, width - 10, height - 25);
                    Rect listContentRect = new Rect(0, 0, width * 2, (countLine + 3) * heightLine + 5);

                    scrollPosOptim = GUI.BeginScrollView(listVisibleRect, scrollPosOptim, listContentRect);
                    //Debug.Log(scrollPosOptim);
                    GUI.color = Color.white;
                    float labelY = heightLine;
                    if (LogInfo != null)
                        foreach (string s in LogInfo.Infos)
                        {
                            EditorGUI.LabelField(new Rect(localstartX, labelY, width * 2, heightLine), s, MidiCommonEditor.styleRichText);
                            labelY += heightLine;
                        }

                    if (displayAlert)
                    {
                        float xpos = 0;
                        float ypos = labelY;
                        float fHeight = MidiCommonEditor.styleAlertRed.fontSize;
                        float fwidth = width * 2;
                        EditorGUI.LabelField(new Rect(xpos, ypos, fwidth, fHeight), "No patchs and samples has been yet extracted from", MidiCommonEditor.styleAlertRed);
                        ypos += fHeight;
                        EditorGUI.LabelField(new Rect(xpos, ypos, fwidth, fHeight), $"the Soundfont '{MidiPlayerGlobal.CurrentMidiSet.ActiveSounFontInfo.Name}'", MidiCommonEditor.styleAlertRed);
                        ypos += 2 * fHeight;
                        EditorGUI.LabelField(new Rect(xpos, ypos, fwidth, fHeight), "On the top-right panel keep the default selection or:", MidiCommonEditor.styleAlertRed);
                        ypos += fHeight;
                        EditorGUI.LabelField(new Rect(xpos, ypos, fwidth, fHeight), "   - Select banks you want to keep.", MidiCommonEditor.styleAlertRed);
                        ypos += fHeight;
                        EditorGUI.LabelField(new Rect(xpos, ypos, fwidth, fHeight), "   - Change default bank for instruments and drums kit.", MidiCommonEditor.styleAlertRed);
                        ypos += 2 * fHeight;
                        EditorGUI.LabelField(new Rect(xpos, ypos, fwidth, fHeight), "On the bottom-right panel, click on buttons:", MidiCommonEditor.styleAlertRed);
                        ypos += fHeight;
                        EditorGUI.LabelField(new Rect(xpos, ypos, fwidth, fHeight), "   'Extract Only Required' to keep only required patchs.", MidiCommonEditor.styleAlertRed);
                        ypos += fHeight;
                        EditorGUI.LabelField(new Rect(xpos, ypos, fwidth, fHeight), "      or", MidiCommonEditor.styleAlertRed);
                        ypos += fHeight;
                        EditorGUI.LabelField(new Rect(xpos, ypos, fwidth, fHeight), "   'Extract All' to keep all samples and patchs from selected banks.", MidiCommonEditor.styleAlertRed);
                        ypos += fHeight;
                        scrollPosOptim = new Vector2(0, 1000);
                    }

                    GUI.EndScrollView();
                }
#endif
            }
            catch (Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }
    }
}