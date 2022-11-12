using System;
using System.Collections.Generic;
using System.IO;
using MPTK.NAudio.Midi;
namespace MidiPlayerTK
{
    //using MonoProjectOptim;
    using UnityEditor;
    using UnityEngine;

    /// <summary>@brief
    /// Window editor for the setup of MPTK
    /// </summary>
    public partial class MidiFileSetupWindow : EditorWindow
    {
        static private List<string> infoEvents;
        static public int PageToDisplay = 0;
        const int MAXLINEPAGE = 100;
        static bool withMeta = false, withNoteOn = false, withNoteOff = false, withPatchChange = false;
        static bool withControlChange = false, withAfterTouch = false, withOthers = false, withDisplayStat = false;
        static int IndexEditItem;

        /// <summary>@brief
        /// Display analyse of midifile
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        private void ShowMidiAnalyse(float startX, float startY, float width, float height)
        {
            try
            {
                GUI.Box(new Rect(startX, startY, width, height), "", MidiCommonEditor.stylePanel);

                float posx = startX + ESPACE;
                float posy = startY + ESPACE;
                int toggleLargeWidth = 70;
                int toggleSmallWidth = 55;

                if (GUI.Button(new Rect(posx, posy, BUTTON_SHORT_WIDTH, BUTTON_HEIGHT), "All"))
                {
                    withMeta = withNoteOn = withNoteOff = withControlChange = withPatchChange = withAfterTouch = withOthers = withDisplayStat = true;
                    ReadEvents();
                }
                posx += BUTTON_SHORT_WIDTH + ESPACE;

                if (GUI.Button(new Rect(posx, posy, BUTTON_SHORT_WIDTH, BUTTON_HEIGHT), "None"))
                {
                    withMeta = withNoteOn = withNoteOff = withControlChange = withPatchChange = withAfterTouch = withOthers = withDisplayStat = false;
                    ReadEvents();
                }
                posx += BUTTON_SHORT_WIDTH + ESPACE;

                bool filter = GUI.Toggle(new Rect(posx, posy, toggleSmallWidth, BUTTON_HEIGHT), withDisplayStat, "Stat", MidiCommonEditor.styleToggle);
                if (filter != withDisplayStat)
                {
                    withDisplayStat = filter;
                    ReadEvents();
                }
                posx += toggleSmallWidth + ESPACE;

                filter = GUI.Toggle(new Rect(posx, posy, toggleLargeWidth, BUTTON_HEIGHT), withNoteOn, "Note On", MidiCommonEditor.styleToggle);
                if (filter != withNoteOn)
                {
                    withNoteOn = filter;
                    ReadEvents();
                }
                posx += toggleLargeWidth + ESPACE;

                filter = GUI.Toggle(new Rect(posx, posy, toggleLargeWidth, BUTTON_HEIGHT), withNoteOff, "Note Off", MidiCommonEditor.styleToggle);
                if (filter != withNoteOff)
                {
                    withNoteOff = filter;
                    ReadEvents();
                }
                posx += toggleLargeWidth + ESPACE;

                filter = GUI.Toggle(new Rect(posx, posy, toggleLargeWidth, BUTTON_HEIGHT), withControlChange, "Control", MidiCommonEditor.styleToggle);
                if (filter != withControlChange)
                {
                    withControlChange = filter;
                    ReadEvents();
                }
                posx += toggleLargeWidth + ESPACE;

                filter = GUI.Toggle(new Rect(posx, posy, toggleSmallWidth, BUTTON_HEIGHT), withPatchChange, "Patch", MidiCommonEditor.styleToggle);
                if (filter != withPatchChange)
                {
                    withPatchChange = filter;
                    ReadEvents();
                }
                posx += toggleSmallWidth + ESPACE;

                filter = GUI.Toggle(new Rect(posx, posy, toggleSmallWidth, BUTTON_HEIGHT), withMeta, "Meta", MidiCommonEditor.styleToggle);
                if (filter != withMeta)
                {
                    withMeta = filter;
                    ReadEvents();
                }
                posx += toggleSmallWidth + ESPACE;

                filter = GUI.Toggle(new Rect(posx, posy, toggleSmallWidth, BUTTON_HEIGHT), withAfterTouch, "Touch", MidiCommonEditor.styleToggle);
                if (filter != withAfterTouch)
                {
                    withAfterTouch = filter;
                    ReadEvents();
                }
                posx += toggleSmallWidth + ESPACE;

                filter = GUI.Toggle(new Rect(posx, posy, toggleSmallWidth, BUTTON_HEIGHT), withOthers, "Others", MidiCommonEditor.styleToggle);
                if (filter != withOthers)
                {
                    withOthers = filter;
                    ReadEvents();
                }
                posx += toggleSmallWidth + ESPACE;


                if (infoEvents != null && infoEvents.Count > 0)
                {

                    if (PageToDisplay < 0) PageToDisplay = 0;
                    if (PageToDisplay * MAXLINEPAGE > infoEvents.Count) PageToDisplay = infoEvents.Count / MAXLINEPAGE;

                    string infoToDisplay = "";
                    for (int i = PageToDisplay * MAXLINEPAGE; i < (PageToDisplay + 1) * MAXLINEPAGE && i < infoEvents.Count; i++)
                        infoToDisplay += infoEvents[i] + "\n";

                    posx = startX + ESPACE;
                    posy = startY + ESPACE + BUTTON_HEIGHT + ESPACE;

                    if (GUI.Button(new Rect(posx, posy, BUTTON_SHORT_WIDTH, BUTTON_HEIGHT), MPTKGui.IconFirst)) PageToDisplay = 0;

                    posx += BUTTON_SHORT_WIDTH + ESPACE;
                    if (GUI.Button(new Rect(posx, posy, BUTTON_SHORT_WIDTH, BUTTON_HEIGHT), MPTKGui.IconPrevious)) PageToDisplay--;

                    posx += BUTTON_SHORT_WIDTH + ESPACE;
                    GUI.Label(new Rect(posx, posy, BUTTON_WIDTH / 2, BUTTON_HEIGHT),
                        "Page " + (PageToDisplay + 1).ToString() + " / " + (infoEvents.Count / MAXLINEPAGE + 1).ToString(), MidiCommonEditor.styleLabelCenter);

                    posx += BUTTON_WIDTH / 2 + ESPACE;
                    if (GUI.Button(new Rect(posx, posy, BUTTON_SHORT_WIDTH, BUTTON_HEIGHT), MPTKGui.IconNext)) PageToDisplay++;

                    posx += BUTTON_SHORT_WIDTH + ESPACE;
                    if (GUI.Button(new Rect(posx, posy, BUTTON_SHORT_WIDTH, BUTTON_HEIGHT), MPTKGui.IconLast)) PageToDisplay = infoEvents.Count / MAXLINEPAGE;

                    float wList = widthRight - 6 * ESPACE;
                    Rect listVisibleRect = new Rect(
                        startX + ESPACE,
                        startY + 2 * BUTTON_HEIGHT + 3 * ESPACE,
                        wList + 4 * ESPACE,
                        heightList - 2 * BUTTON_HEIGHT - 4 * ESPACE);

                    Rect listContentRect = new Rect(
                        0,
                        0,
                        wList,
                        MAXLINEPAGE * MidiCommonEditor.styleRichTextBorder.lineHeight + ESPACE);

                    Rect fondRect = new Rect(
                        startX + ESPACE,
                        startY + 2 * BUTTON_HEIGHT + 3 * ESPACE,
                        wList - 0 * ESPACE,
                        heightList - 2 * BUTTON_HEIGHT - 4 * ESPACE);

                    GUI.Box(fondRect, "", MidiCommonEditor.stylePanel);

                    scrollPosAnalyze = GUI.BeginScrollView(listVisibleRect, scrollPosAnalyze, listContentRect);
                    GUI.Box(listContentRect, infoToDisplay, MidiCommonEditor.styleLabelFontCourier);

                    GUI.EndScrollView();
                }
                else
                {
                    GUIContent content = new GUIContent() { text = "No Midi file selected.", tooltip = "" };
                    EditorGUI.LabelField(new Rect(startX + xpostitlebox + 5, startY + ypostitlebox + 30, 300, itemHeight), content, MidiCommonEditor.styleBold);
                }
            }
            catch (Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }


        static private void DisplayStat()
        {
            if (IndexEditItem >= 0 && IndexEditItem < MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count)
            {
                string pathMidiFile = MidiPlayerGlobal.CurrentMidiSet.MidiFiles[IndexEditItem];
                MidiLoad midifile = new MidiLoad();
                midifile.KeepNoteOff = withNoteOff;
                midifile.MPTK_EnableChangeTempo = true;
                midifile.MPTK_Load(pathMidiFile);
                if (midifile != null)
                {
                    // Using dictionnary would be better than array but the purpose here is further
                    // to demonstrate how computing statistics from a MIDI file in editor mode.
                    int[,] stat_note = new int[16, 128];
                    int[] stat_channel = new int[16];

                    try
                    {
                        // Calculate notes count by channel
                        foreach (TrackMidiEvent trackEvent in midifile.MPTK_MidiEvents)
                        {
                            // In editor mode, only the basic structure of MIDI is available (not MPTKEvent)
                            if (trackEvent.Event.CommandCode == MidiCommandCode.NoteOn)
                            {
                                NoteOnEvent noteon = (NoteOnEvent)trackEvent.Event;
                                // Channel with NAudio start at 1 but start at 0 with MPTK
                                stat_note[noteon.Channel - 1, noteon.NoteNumber]++;
                            }
                        }
                    }
                    catch (Exception ex) { Debug.LogWarning(ex); }

                    infoEvents.Add("");
                    try
                    {
                        // Display notes count and calculate count by channel
                        for (int channel = 0; channel < 16; channel++)
                            for (int note = 0; note < 128; note++)
                                if (stat_note[channel, note] > 0)
                                {
                                    stat_channel[channel]++;
                                    infoEvents.Add($"Channel:{channel} note:{note} count:{stat_note[channel, note]}");
                                }
                    }
                    catch (Exception ex) { Debug.LogWarning(ex); }

                    // Display count by channel
                    infoEvents.Add("");
                    for (int channel = 0; channel < 16; channel++)
                        if (stat_channel[channel] > 0)
                            infoEvents.Add($"Total notes for channel:{channel} count:{stat_channel[channel]}");
                }
            }
        }

        static private void ReadEvents()
        {
            infoEvents = new List<string>();
            if (IndexEditItem >= 0 && IndexEditItem < MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count)
            {
                try
                {
                    string midifile = MidiPlayerGlobal.CurrentMidiSet.MidiFiles[IndexEditItem];
                    infoEvents.Add("");
                    infoEvents.Add("MIDI file scanned: " + midifile);
                    infoEvents.Add("MIDI DB index: " + IndexEditItem);
                    infoEvents.AddRange(MidiScan.GeneralInfo(midifile, withNoteOn, withNoteOff, withControlChange, withPatchChange, withAfterTouch, withMeta, withOthers));
                }
                catch (Exception ex) { Debug.LogWarning(ex); }

                if (withDisplayStat)
                    DisplayStat();
            }
            PageToDisplay = 0;
            scrollPosAnalyze = Vector2.zero;
        }

    }
}