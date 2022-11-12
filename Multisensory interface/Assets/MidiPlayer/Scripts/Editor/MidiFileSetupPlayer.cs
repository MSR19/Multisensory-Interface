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
    /// Window editor for the setup of MPTK,  
    /// </summary>
    public partial class MidiFileSetupWindow : EditorWindow
    {
        const int MAX_EVENT_PAGE = 250;
        const int WIDTH_BUTTON_PLAYER = 100;
        const int HEIGHT_PLAYER_CMD = 60 + 30;
        //const int HEIGHT_SETTING_DISPLAY = 60;
        const int HEIGHT_TITLE = 25;
        const int AREA_SPACE = 4;

        int SelectedEvent = 0;
        List<MPTKEvent> AllMidiEvents;
        MPTKEvent LastMidiEvent;
        List<List<MPTKEvent>> MidiEventsLoaded;
        MPTKEvent LastEventPlayed;
        int CurrentPage = 0;
        bool FollowEvent = true;
        List<MPTKGui.StyleItem> ColumnEvents;
        Vector2 ScrollerMidiPlayer = Vector2.zero;

        MPTKGui.PopupList PopupDisplayTime;
        int DisplayTime;

        List<MPTKGui.StyleItem> PopupFiltersDisplay;
        List<MPTKGui.StyleItem> PopupFiltersTrack;
        List<MPTKGui.StyleItem> PopupFiltersChannel;
        List<MPTKGui.StyleItem> PopupFiltersCommand;

        private void InitPlayer()
        {
            MidiPlayerEditor.MidiPlayer.MPTK_Volume = 0.5f;
            MidiPlayerEditor.MidiPlayer.MPTK_Speed = 1f;
        }

        private void InitGUI()
        {
            if (PopupFiltersDisplay == null)
            {
                PopupFiltersDisplay = new List<MPTKGui.StyleItem>();
                PopupFiltersDisplay.Add(new MPTKGui.StyleItem("Ticks", true, true));
                PopupFiltersDisplay.Add(new MPTKGui.StyleItem("Seconds", true));
                PopupFiltersDisplay.Add(new MPTKGui.StyleItem("hh:mm:ss", true));

                PopupFiltersTrack = new List<MPTKGui.StyleItem>();
                for (int track = 0; track < 50; track++)
                    PopupFiltersTrack.Add(new MPTKGui.StyleItem($"Tracks {track}", true, true));
            }
            if (PopupFiltersChannel == null)
            {
                PopupFiltersChannel = new List<MPTKGui.StyleItem>();
                for (int channel = 0; channel < 16; channel++)
                    PopupFiltersChannel.Add(new MPTKGui.StyleItem($"Channel {channel}", true, true));

                PopupFiltersCommand = new List<MPTKGui.StyleItem>();
                PopupFiltersCommand.Add(new MPTKGui.StyleItem("Note On", true, true));
                PopupFiltersCommand.Add(new MPTKGui.StyleItem("Note Off", true, true));
                PopupFiltersCommand.Add(new MPTKGui.StyleItem("Control Change", true, true));
                PopupFiltersCommand.Add(new MPTKGui.StyleItem("Patch Change", true, true));
                PopupFiltersCommand.Add(new MPTKGui.StyleItem("Meta", true, true));
                PopupFiltersCommand.Add(new MPTKGui.StyleItem("Touch", true, true));
                PopupFiltersCommand.Add(new MPTKGui.StyleItem("Others", true, true));
            }
            if (ColumnEvents == null)
            {
                //GUIStyle styleRight = MPTKGui.BuildStyle(inheritedStyle: MPTKGui.LabelListNormal,anchor: TextAnchor.MiddleRight);
                ColumnEvents = new List<MPTKGui.StyleItem>();
                ColumnEvents.Add(new MPTKGui.StyleItem() { Width = 60, Caption = "Index", Tooltip = "Unique index of the MIDI event. Some index are missing because by default, MPTK removes NotOff Events." });
                ColumnEvents.Add(new MPTKGui.StyleItem() { Width = 70, Caption = "Tick", Tooltip = "Time in MIDI Tick (part of a Beat) of the Event since the start of playing the midi file. This time is independent of the Tempo or Speed." });
                ColumnEvents.Add(new MPTKGui.StyleItem() { Width = 70, Caption = "Time (s)", Hidden = true, Tooltip = "Real time in seconds of this event from the start of the midi depending the tempo change." });
                ColumnEvents.Add(new MPTKGui.StyleItem() { Width = 120, Caption = "Time", Hidden = true, Tooltip = "Real time in seconds of this event from the start of the midi depending the tempo change." });
                ColumnEvents.Add(new MPTKGui.StyleItem() { Width = 70, Caption = "Track{*}", Offset = 25, ItemPopupContent = PopupFiltersTrack, ItemPopupWidth = 100, Tooltip = "Track index of the event in the midi. It's just a cool way to regroup MIDI events in a ... track. There is any impact on music played. Track 0 is the first track read from the midi file." });
                ColumnEvents.Add(new MPTKGui.StyleItem() { Width = 70, Caption = "Channel{*}", Offset = 25, ItemPopupContent = PopupFiltersChannel, ItemPopupWidth = 100, Tooltip = "Midi channel fom 0 to 15 (9 for drum). Only one instrument can be selected at a time for a chanel." });
                ColumnEvents.Add(new MPTKGui.StyleItem() { Width = 200, Caption = "Command {Count}", Offset = 10, ItemPopupContent = PopupFiltersCommand, Tooltip = "Midi Command. Defined the MIDI action to be done by the synthesizer." });
                ColumnEvents.Add(new MPTKGui.StyleItem() { Width = 180, Caption = "Value", Tooltip = "Contains a value in relation with the Command: Note pitch, instrument selected, control change value, text for Meta command. " });

                
                ColumnEvents.Add(new MPTKGui.StyleItem() { Width = 70, Caption = "Velocity",  Tooltip = "Velocity between 0 and 127." });
                ColumnEvents.Add(new MPTKGui.StyleItem() { Width = 50, Caption = "Duration", Tooltip = "Duration of the note in ticks." }); //9
                ColumnEvents.Add(new MPTKGui.StyleItem() { Width = 70, Caption = "Duration (s)", Hidden = true, Tooltip = "Duration of the note in second." }); //10
                ColumnEvents.Add(new MPTKGui.StyleItem() { Width = 120, Caption = "Duration", Hidden = true, Tooltip = "Duration of the note." }); //11
            }
        }

        private void PlayMidiFileSelected(int indexEditItem)
        {
            if (MidiPlayerGlobal.MPTK_SoundFontIsReady)
            {
                MidiPlayerEditor.MidiPlayer.MPTK_Stop();
                if (indexEditItem >= 0 && indexEditItem < MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count)
                {
                    MidiPlayerEditor.MidiPlayer.MPTK_StartPlayAtFirstNote = true;
                    MidiPlayerEditor.MidiPlayer.MPTK_EnableChangeTempo = true;
                    MidiPlayerEditor.MidiPlayer.MPTK_MidiIndex = indexEditItem;
                    MidiPlayerEditor.MidiPlayer.MPTK_KeepNoteOff = true;
                    MidiPlayerEditor.MidiPlayer.OnEventStartPlayMidi.AddListener(StartPlay);
                    MidiPlayerEditor.MidiPlayer.OnEventNotesMidi.AddListener(MidiReadEvents);
                    MidiPlayerEditor.MidiPlayer.MPTK_Play();
                }
            }
        }

        public void StartPlay(string midiname)
        {
            //Debug.Log("Start Midi " + midiname + " Duration: " + MidiPlayerEditor.MidiPlayer.MPTK_Duration.TotalSeconds + " seconds");
            AllMidiEvents = MidiPlayerEditor.MidiPlayer.MPTK_ReadMidiEvents();
            LastMidiEvent = AllMidiEvents[AllMidiEvents.Count - 1];
            MidiEventsLoaded = new List<List<MPTKEvent>>();
            List<MPTKEvent> pageMidi = null;
            SelectedEvent = 0;
            CurrentPage = 0;
            // Load each MIDI events by page
            foreach (MPTKEvent midiEvent in AllMidiEvents)
            {
                if (pageMidi == null || pageMidi.Count >= MAX_EVENT_PAGE)
                {
                    pageMidi = new List<MPTKEvent>();
                    MidiEventsLoaded.Add(pageMidi);
                }
                pageMidi.Add(midiEvent);
            }
        }

        /// <summary>@brief
        /// Event fired by MidiFilePlayer when midi notes are available. 
        /// Set by Unity Editor in MidiFilePlayer Inspector or by script with OnEventNotesMidi.
        /// </summary>
        public void MidiReadEvents(List<MPTKEvent> midiEvents)
        {
            //List<MPTKEvent> eventsOrdered = events.OrderBy(o => o.Value).ToList();
            LastEventPlayed = midiEvents[midiEvents.Count - 1];
            //lastEventPlayed = MidiPlayerEditor.MidiPlayer.MPTK_LastEventPlayed;
            //Debug.Log(lastEventPlayed.Tick);
            if (FollowEvent)
            {
                // Find page with this event
                if (LastEventPlayed.Tick < MidiEventsLoaded[CurrentPage][0].Tick ||
                    LastEventPlayed.Tick > MidiEventsLoaded[CurrentPage][MidiEventsLoaded[CurrentPage].Count - 1].Tick)
                {
                    // Last pLayed events is on another page
                    for (int page = 0; page < MidiEventsLoaded.Count; page++)
                    {
                        List<MPTKEvent> eventsPage = MidiEventsLoaded[page];
                        if (LastEventPlayed.Tick >= eventsPage[0].Tick && LastEventPlayed.Tick <= eventsPage[eventsPage.Count - 1].Tick)
                        {
                            //Debug.Log($"change to page {page}");
                            CurrentPage = page;
                            break;
                        }
                    }
                }
                //Debug.Log($"{scrollerMidiPlayer}");
                window.Repaint();
            }
        }
        Rect rectPopup;
        /// <summary>@brief
        /// Display analyse of midifile
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        private void ShowMidiPlayer(float startX, float startY, float width, float height)
        {
            try
            {
                InitGUI();

                // Begin area MIDI player commands
                // --------------------------
                GUILayout.BeginArea(new Rect(startX, startY, width, HEIGHT_PLAYER_CMD), MidiCommonEditor.stylePanel);
                float nextAreaY = startY + HEIGHT_PLAYER_CMD + AREA_SPACE;
                if (!MidiPlayerGlobal.MPTK_SoundFontIsReady)
                {
                    GUILayout.Space(20);
                    GUILayout.Label(MidiPlayerGlobal.ErrorNoSoundFont, MPTKGui.LabelBoldCentered);
                }
                else if (IndexEditItem < 0)
                {
                    GUILayout.Space(20);
                    GUILayout.Label("No Midi file selected.", MPTKGui.LabelBoldCentered);
                }
                else
                {

                    // Begin Player control group ----- First line -----
                    GUILayout.BeginHorizontal();

                    MidiPlayerEditor.MidiPlayer.MPTK_Loop = GUILayout.Toggle(MidiPlayerEditor.MidiPlayer.MPTK_Loop, "Looping ");

                    string titleMidi = MidiPlayerEditor.MidiPlayer.MPTK_IsPaused ? "Paused" : MidiPlayerEditor.MidiPlayer.MPTK_IsPlaying ? "Playing" : "Loaded";
                    titleMidi += $": {IndexEditItem} - {MidiPlayerGlobal.CurrentMidiSet.MidiFiles[IndexEditItem]}";
                    GUILayout.Label(titleMidi, MPTKGui.LabelGray);
                    //if (Event.current.type == EventType.Repaint) Debug.Log(GUILayoutUtility.GetLastRect());

                    //GUILayout.FlexibleSpace();

                    GUIStyle styleButton = MidiPlayerEditor.MidiPlayer.MPTK_IsPlaying ? MPTKGui.ButtonHighLight : MPTKGui.button;
                    if (GUILayout.RepeatButton("Play", styleButton, GUILayout.Width(WIDTH_BUTTON_PLAYER)))
                        MidiPlayerEditor.MidiPlayer.MPTK_Play();

                    styleButton = MidiPlayerEditor.MidiPlayer.MPTK_IsPaused ? MPTKGui.ButtonHighLight : MPTKGui.button;
                    if (GUILayout.RepeatButton("Pause", styleButton, GUILayout.Width(WIDTH_BUTTON_PLAYER)))
                        MidiPlayerEditor.MidiPlayer.MPTK_Pause();

                    if (GUILayout.RepeatButton("Stop", GUILayout.Width(WIDTH_BUTTON_PLAYER)))
                        MidiPlayerEditor.MidiPlayer.MPTK_Stop();

                    GUILayout.FlexibleSpace();

                    GUILayout.EndHorizontal();

                    // Sliders ----- Second line -----
                    GUILayout.BeginHorizontal();

                    float volume = MidiPlayerEditor.MidiPlayer.MPTK_Volume;
                    GUILayout.Label("Volume: " + volume.ToString("F2"), GUILayout.Width(80));
                    MidiPlayerEditor.MidiPlayer.MPTK_Volume = GUILayout.HorizontalSlider(volume, 0.0f, 1f);

                    if (DisplayTime == 0)
                    {
                        long tickCurrent = MidiPlayerEditor.MidiPlayer.MPTK_TickCurrent;
                        GUILayout.Label($"     Tick: {tickCurrent:000000} / {LastMidiEvent.Tick:000000}", GUILayout.Width(150));
                        long tick = (long)GUILayout.HorizontalSlider((float)tickCurrent, 0f, (float)MidiPlayerEditor.MidiPlayer.MPTK_TickLast);
                        if (tick != tickCurrent)
                        {
                            if (Event.current.type == EventType.Used)
                            {
                                //Debug.Log("New tick " + midiFilePlayer.MPTK_TickCurrent + " --> " + tick + " " + Event.current.type);
                                MidiPlayerEditor.MidiPlayer.MPTK_TickCurrent = tick;
                            }
                        }
                    }
                    else
                    {
                        double currentPosition = Math.Round(MidiPlayerEditor.MidiPlayer.MPTK_Position / 1000d, 3);
                        double lastPosition = MidiPlayerEditor.MidiPlayer.MPTK_PositionLastNote / 1000d;
                        if (DisplayTime == 1)
                        {
                            GUILayout.Label($"   Time: {currentPosition:F2} / {lastPosition:F2} sec.", GUILayout.Width(180));
                        }
                        else if (DisplayTime == 2)
                        {
                            TimeSpan timePos = TimeSpan.FromSeconds(currentPosition);
                            string playTime = string.Format("{0:00}:{1:00}:{2:00}:{3:000}", timePos.Hours, timePos.Minutes, timePos.Seconds, timePos.Milliseconds);
                            TimeSpan lastPos = TimeSpan.FromSeconds(lastPosition);
                            string lastTime = string.Format("{0:00}:{1:00}:{2:00}:{3:000}", lastPos.Hours, lastPos.Minutes, lastPos.Seconds, lastPos.Milliseconds);
                            GUILayout.Label($"Time: {playTime} / {lastTime}", GUILayout.Width(220));
                        }
                        double newPosition = Math.Round(GUILayout.HorizontalSlider((float)currentPosition, 0f, (float)MidiPlayerEditor.MidiPlayer.MPTK_Duration.TotalSeconds/*, GUILayout.Width(150)*/), 2);
                        if (newPosition != currentPosition)
                        {
                            if (Event.current.type == EventType.Used)
                            {
                                //Debug.Log("New position " + currentPosition + " --> " + newPosition + " " + Event.current.type);
                                MidiPlayerEditor.MidiPlayer.MPTK_Position = newPosition * 1000d;
                            }
                        }
                    }


                    float speed = MidiPlayerEditor.MidiPlayer.MPTK_Speed;
                    GUILayout.Label("   Speed: " + speed.ToString("F2"), GUILayout.ExpandWidth(false));
                    MidiPlayerEditor.MidiPlayer.MPTK_Speed = GUILayout.HorizontalSlider(speed, 0.01f, 10f);

                    GUILayout.EndHorizontal();

                    //GUILayout.EndArea(); // End player command

                    if (MidiEventsLoaded != null)
                    {
                        // Begin area display settings
                        // --------------------------
                        //GUILayout.BeginArea(new Rect(startX, nextAreaY, width, HEIGHT_SETTING_DISPLAY), MidiCommonEditor.stylePanel);
                        //nextAreaY += HEIGHT_SETTING_DISPLAY + AREA_SPACE;

                        // Display settings -- Third line --
                        GUILayout.BeginHorizontal();
                        float alignHorizontal = 24;

                        FollowEvent = GUILayout.Toggle(FollowEvent, "Follow MIDI Events", GUILayout.Width(130), GUILayout.Height(alignHorizontal));

                        // Select display time format
                        MPTKGui.ComboBox(ref PopupDisplayTime, "Display Time: {Label}", PopupFiltersDisplay, DisplayTime,
                               delegate (int index)
                               {
                                   DisplayTime = index;
                                   ColumnEvents[1].Hidden = ColumnEvents[2].Hidden = ColumnEvents[3].Hidden = true;
                                   ColumnEvents[9].Hidden = ColumnEvents[10].Hidden = ColumnEvents[11].Hidden = true;
                                   if (DisplayTime == 0)
                                   {
                                       ColumnEvents[1].Hidden = false;
                                       ColumnEvents[9].Hidden = false;
                                   }
                                   else if (DisplayTime == 1)
                                   {
                                       ColumnEvents[2].Hidden = false;
                                       ColumnEvents[10].Hidden = false;
                                   }
                                   else if (DisplayTime == 2)
                                   {
                                       ColumnEvents[3].Hidden = false;
                                       ColumnEvents[11].Hidden = false;
                                   }
                               }
                               , null);


                        // Change page
                        if (GUILayout.Button(MPTKGui.IconFirst)) CurrentPage = 0;
                        if (GUILayout.Button(MPTKGui.IconPrevious)) CurrentPage--;
                        GUILayout.Label($"Page {CurrentPage} / {MidiEventsLoaded.Count - 1}", MidiCommonEditor.styleLabelCenter, GUILayout.Height(alignHorizontal));
                        if (GUILayout.Button(MPTKGui.IconNext)) CurrentPage++;
                        if (GUILayout.Button(MPTKGui.IconLast)) CurrentPage = MidiEventsLoaded.Count - 1;
                        CurrentPage = Mathf.Clamp(CurrentPage, 0, MidiEventsLoaded.Count - 1);
                        GUILayout.EndHorizontal();

                        //GUILayout.BeginHorizontal();
                        //GUILayout.Label("Filter Display:", GUILayout.Width(100), GUILayout.Height(alignHorizontal));
                        //if (GUILayout.Button("All", GUILayout.Width(WIDTH_BUTTON_PLAYER))) for (int i = 0; i < FiltersEvent.Length; i++) FiltersEvent[i] = true;
                        //if (GUILayout.Button("None", GUILayout.Width(WIDTH_BUTTON_PLAYER))) for (int i = 0; i < FiltersEvent.Length; i++) FiltersEvent[i] = false;
                        //FiltersEvent[0] = GUILayout.Toggle(FiltersEvent[0], "Note On", MidiCommonEditor.styleToggle);
                        //FiltersEvent[1] = GUILayout.Toggle(FiltersEvent[1], "Note Off", MidiCommonEditor.styleToggle);
                        //FiltersEvent[2] = GUILayout.Toggle(FiltersEvent[2], "Control Change", MidiCommonEditor.styleToggle);
                        //FiltersEvent[3] = GUILayout.Toggle(FiltersEvent[3], "Patch Change", MidiCommonEditor.styleToggle);
                        //FiltersEvent[4] = GUILayout.Toggle(FiltersEvent[4], "Meta", MidiCommonEditor.styleToggle);
                        //FiltersEvent[5] = GUILayout.Toggle(FiltersEvent[5], "Touch", MidiCommonEditor.styleToggle);
                        //FiltersEvent[6] = GUILayout.Toggle(FiltersEvent[6], "Others", MidiCommonEditor.styleToggle);
                        //GUILayout.EndHorizontal();

                        GUILayout.EndArea();
                        // End player command // End setting display 

                        // Begin area title list
                        // --------------------------
                        GUILayout.BeginArea(new Rect(startX, nextAreaY, width, HEIGHT_TITLE), MidiCommonEditor.styleListTitle);
                        nextAreaY += HEIGHT_TITLE - 1; // -1 to overlay edge
                        GUILayout.BeginHorizontal();
                        GUIStyle style = MPTKGui.LabelListNormal; // get a ref to the style
                        style.contentOffset = new Vector2(-ScrollerMidiPlayer.x, style.contentOffset.y);
                        foreach (MPTKGui.StyleItem column in ColumnEvents)
                            if (!column.Hidden)
                                if (column.ItemPopupContent == null)
                                    GUILayout.Label(new GUIContent(column.Caption, column.Tooltip), style, GUILayout.Width(column.Width));
                                else
                                    MPTKGui.ComboBox(ref column.ItemPopup, column.Caption, column.ItemPopupContent, -1, null, style, column.ItemPopupWidth, GUILayout.Width(column.Width));

                        style.contentOffset = Vector2.zero;
                        GUILayout.EndHorizontal();
                        GUILayout.EndArea();

                        // Begin area MIDI events list
                        // --------------------------
                        // Why +30 ? Any idea !
                        GUILayout.BeginArea(new Rect(startX, nextAreaY, width, height - nextAreaY + 30), MidiCommonEditor.stylePanel);

                        ScrollerMidiPlayer = GUILayout.BeginScrollView(ScrollerMidiPlayer);//, false, false, MidiCommonEditor.stylePanel);

                        // Foreach MIDI events on the current page
                        // ---------------------------------------
                        int lineIndex = 0;
                        foreach (MPTKEvent midiEvent in MidiEventsLoaded[CurrentPage])
                        {
                            // Filters MIDI events
                            switch (midiEvent.Command)
                            {
                                case MPTKCommand.NoteOn: if (!PopupFiltersCommand[0].Selected) continue; break;
                                case MPTKCommand.NoteOff: if (!PopupFiltersCommand[1].Selected) continue; break;
                                case MPTKCommand.ControlChange: if (!PopupFiltersCommand[2].Selected) continue; break;
                                case MPTKCommand.PatchChange: if (!PopupFiltersCommand[3].Selected) continue; break;
                                case MPTKCommand.MetaEvent: if (!PopupFiltersCommand[4].Selected) continue; break;
                                case MPTKCommand.ChannelAfterTouch:
                                case MPTKCommand.KeyAfterTouch: if (!PopupFiltersCommand[5].Selected) continue; break;
                                default: if (!PopupFiltersCommand[6].Selected) continue; break;
                            }
                            // Filters channel
                            if (!PopupFiltersChannel[midiEvent.Channel].Selected) continue;

                            // Filters track
                            if (!PopupFiltersTrack[(int)midiEvent.Track].Selected) continue;

                            GUIStyle styleRow;
                            bool isPlayed;
                            if (FollowEvent && LastEventPlayed != null && midiEvent.Tick == LastEventPlayed.Tick)
                            {
                                //selectedEvent = midiEvent.Index;
                                styleRow = MPTKGui.LabelListPlayed;
                                isPlayed = true;
                            }
                            else if (midiEvent.Index == SelectedEvent)
                            {
                                styleRow = MPTKGui.LabelListSelected;  
                                isPlayed = false;
                            }
                            else
                            {
                                styleRow = null; // Default column style
                                isPlayed = false;
                            }

                            GUILayout.BeginHorizontal();
                            try
                            {
                                int index = midiEvent.Index;
                                string text = "";
                                int column = 0;
                                AddColumn(midiEvent, ColumnEvents[column++], midiEvent.Index.ToString(), styleRow);
                                AddColumn(midiEvent, ColumnEvents[column++], midiEvent.Tick.ToString(), styleRow);
                                AddColumn(midiEvent, ColumnEvents[column++], midiEvent.RealTime, styleRow);
                                AddColumn(midiEvent, ColumnEvents[column++], midiEvent.RealTime, styleRow);
                                AddColumn(midiEvent, ColumnEvents[column++], midiEvent.Track.ToString(), styleRow);
                                AddColumn(midiEvent, ColumnEvents[column++], midiEvent.Channel.ToString(), styleRow);

                                bool displayMoreColumns = false;
                                // Command column
                                switch (midiEvent.Command)
                                {
                                    case MPTKCommand.ControlChange:
                                        text = "CC - " + midiEvent.Controller.ToString();
                                        break;
                                    case MPTKCommand.MetaEvent:
                                        text = "Meta - " + midiEvent.Meta.ToString();
                                        break;
                                    default:
                                        text = midiEvent.Command.ToString();
                                        break;
                                }
                                AddColumn(midiEvent, ColumnEvents[column++], text, styleRow); // Command column

                                // Value column to display depend on command
                                switch (midiEvent.Command)
                                {
                                    case MPTKCommand.NoteOn:
                                        displayMoreColumns = true; // Display velocity and duration
                                        text = midiEvent.Value.ToString("000") + " - " + HelperNoteLabel.LabelFromMidi(midiEvent.Value);
                                        break;
                                    case MPTKCommand.NoteOff:
                                        text = midiEvent.Value.ToString("000") + " - " + HelperNoteLabel.LabelFromMidi(midiEvent.Value);
                                        break;
                                    case MPTKCommand.PatchChange:
                                        text = midiEvent.Value.ToString("000") + " - " + MidiPlayerGlobal.MPTK_GetPatchName(0, midiEvent.Value);
                                        break;
                                    case MPTKCommand.MetaEvent:
                                        switch (midiEvent.Meta)
                                        {
                                            case MPTKMeta.KeySignature: text = $"SharpsFlats:{midiEvent.Value} MajorMinor:{midiEvent.Duration}"; break;
                                            case MPTKMeta.TimeSignature: text = $"Numerator:{midiEvent.Value} Denominator:{midiEvent.Duration}"; break;
                                            case MPTKMeta.SetTempo: text = $"µseconds:{midiEvent.Value} Tempo:{midiEvent.Duration}"; break;
                                            default: text = midiEvent.Info ?? ""; break;
                                        }
                                        if (text.Length > 60) text = text.Substring(0, 60);
                                        break;
                                    default:
                                        text = midiEvent.Value.ToString();
                                        break;
                                }
                                AddColumn(midiEvent, ColumnEvents[column++], text, styleRow); // Value column

                                AddColumn(midiEvent, ColumnEvents[column++], displayMoreColumns ? midiEvent.Velocity.ToString() : "", styleRow);
                                AddColumn(midiEvent, ColumnEvents[column++], displayMoreColumns ? midiEvent.durationTicks.ToString() : "", styleRow);
                                AddColumn(midiEvent, ColumnEvents[column++], displayMoreColumns ? midiEvent.Duration : -1, styleRow);
                                AddColumn(midiEvent, ColumnEvents[column++], displayMoreColumns ? midiEvent.Duration : -1, styleRow); // 11 time hh:mm:ss

                                // make events played visible in the schroll
                                lineIndex++;
                                if (Event.current.type == EventType.Repaint)
                                {
                                    Rect lastEventDraw = GUILayoutUtility.GetLastRect();
                                    if (isPlayed && FollowEvent)
                                    {
                                        if (lineIndex > 10)
                                            lastEventDraw.y += 5 * 15; // 5 lines from the bottom
                                        else
                                            lastEventDraw.y = 0; // let the event displayed from the top
                                        lastEventDraw.x = ScrollerMidiPlayer.x; // don't move the horizontal scroll
                                        GUI.ScrollTo(lastEventDraw);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.LogWarning(ex);
                            }
                            GUILayout.EndHorizontal();
                        }

                        GUILayout.EndScrollView();
                    }
                }

                GUILayout.EndArea();
                // End MIDI events list

            }
            catch (Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }
        private void AddColumn(MPTKEvent midiEvent, MPTKGui.StyleItem item, float timeMs, GUIStyle styleRow)
        {
            if (!item.Hidden)
            {
                string text = "0";
                if (timeMs < 0)
                    text = "";
                else if (timeMs > 0)
                {
                    if (DisplayTime == 1)
                        text = (timeMs / 1000f).ToString("F2");
                    else if (DisplayTime == 2)
                    {
                        TimeSpan timePos = TimeSpan.FromMilliseconds(timeMs);
                        text = string.Format("{0:00}:{1:00}:{2:00}:{3:000}", timePos.Hours, timePos.Minutes, timePos.Seconds, timePos.Milliseconds);
                    }
                }
                AddColumn(midiEvent, item, text, styleRow);
            }
        }

        private void AddColumn(MPTKEvent midiEvent, MPTKGui.StyleItem item, string text, GUIStyle styleRow=null)
        {
            if (!item.Hidden)
            {
                GUIStyle style = styleRow == null ? item.Style : styleRow;
                if (item.Offset != 0) style.contentOffset = new Vector2(item.Offset, 0);
                GUILayout.Label(text, style, GUILayout.Width(item.Width));
                if (item.Offset != 0) style.contentOffset = Vector2.zero;

                // User select a line ?
                if (Event.current.type == EventType.MouseDown)
                    if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        //Debug.Log($"{midiEvent.Index} XXX {window.position.x + GUILayoutUtility.GetLastRect().x}");
                        SelectedEvent = midiEvent.Index;
                        MidiPlayerEditor.MidiPlayer.MPTK_TickCurrent = midiEvent.Tick;
                        window.Repaint();
                    }
            }
        }

        private void CheckKeyboardEvent()
        {
            Event e = Event.current;
            //if (e.type != EventType.Layout && e.type != EventType.Repaint) Debug.Log(e.type + " " + e.mousePosition + " isMouse:" + e.isMouse + " isKey:" + e.isKey + " keyCode:" + e.keyCode + " modifiers:" + e.modifiers + " displayIndex:" + e.displayIndex);
            //Debug.Log($" {e.type} ");
            //return;
            // Check keyboard
            // --------------
            if (e.type == EventType.KeyDown || e.type == EventType.KeyUp)
            {
                //Debug.Log($"{e.keyCode} {(int)e.keyCode}");
                if (e.keyCode >= KeyCode.Keypad0 && e.keyCode <= KeyCode.Keypad9)
                {
                }
                e.Use();
            }

            // Check mouse
            // -----------
            if (e.type == EventType.MouseDown || e.type == EventType.MouseUp || e.type == EventType.MouseMove)
            {
                Debug.Log($"MouseMove {e.mousePosition} {(int)e.button} {(int)e.clickCount}");
            }
        }
    }
}