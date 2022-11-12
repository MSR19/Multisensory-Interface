//#define MPTK_PRO
#define DEBUG_START_MIDIx
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System;
using UnityEngine.Events;
using MEC;

namespace MidiPlayerTK
{
    // required to instanciate in edit mode: Awake() and Start() are executed when this class is instanciated
    [ExecuteInEditMode] 
    public partial class MidiFileEditorPlayer : MidiFilePlayer
    {
    }
}

