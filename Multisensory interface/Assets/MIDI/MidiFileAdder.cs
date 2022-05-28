using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

public class MidiFileAdder : MonoBehaviour
{
    public string fileName;

    public string dataVar1FileName;
    public string dataVar2FileName;
    public string dataVar3FileName;

    public int minPercentageFile;
    public int maxPercentageFile;

    public MidiFilePlayer midiFilePlayer;

    public void addFile()
    {
        StartCoroutine(waiter());
        //callProg();
        //addFileMPTK();

    }

    public void addFileMPTK()
    {
        MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Add(fileName);
        
        MidiPlayerGlobal.MPTK_Stop();
        MidiPlayerGlobal.BuildMidiList();

        midiFilePlayer.MPTK_Stop();
        midiFilePlayer.MPTK_MidiName = fileName;
        midiFilePlayer.MPTK_MidiIndex = MidiPlayerGlobal.CurrentMidiSet.MidiFiles.IndexOf(fileName);
        print(MidiPlayerGlobal.MPTK_FindMidi(fileName));

        print("rdy!");
        //midiFilePlayer.MPTK_Load();
        //midiFilePlayer.MPTK_Play();

    }

    public void callProg()
    {
        var p = new System.Diagnostics.Process();
        p.StartInfo.FileName = "index.js";
        p.StartInfo.Arguments = "node index.js " + "\"../MidiPlayer/Resources/" + fileName + "\"" + " " + minPercentageFile + " " + maxPercentageFile + " \"../Data/" + dataVar1FileName + "\"";
        p.StartInfo.WorkingDirectory = "C:/Users/migue/OneDrive/Ambiente de Trabalho/IATK-master - Cópia/Assets/Node/";
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.UseShellExecute = false;
        p.Start();

        string log = p.StandardOutput.ReadToEnd();
        string errorLog = p.StandardError.ReadToEnd();

        p.WaitForExit();
        p.Close();

        print(log);
        print(errorLog);
    }

    IEnumerator waiter()
    {
        //StreamWriter sr = File.CreateText("Assets/MidiPlayer/Resources/MidiDB/" + fileName + ".bytes");
        //sr.Close();
        //yield return new WaitForSeconds(1);
        //print("importing");
        //TextAsset bindata = Resources.Load<TextAsset>("MidiDB/" + fileName);
        //print("Import done!");
        //yield return new WaitForSeconds(1);
        callProg();
        yield return new WaitForSeconds(5);
        //print("Agora");
        string sourceFile = @"C:/Users/migue/OneDrive/Ambiente de Trabalho/IATK-master - Cópia/Assets/MidiPlayer/Resources/" + fileName + ".bytes";
        string destinationFile = @"C:/Users/migue/OneDrive/Ambiente de Trabalho/IATK-master - Cópia/Assets/MidiPlayer/Resources/MidiDB/" + fileName + ".bytes";

        //string soutceMetaFile = @"C:/Users/migue/OneDrive/Ambiente de Trabalho/IATK-master - Cópia/Assets/MidiPlayer/Resources/" + fileName + ".bytes.meta";
        //string destinationMetaFile = @"C:/Users/migue/OneDrive/Ambiente de Trabalho/IATK-master - Cópia/Assets/MidiPlayer/Resources/MidiDB/" + fileName + ".bytes.meta";

        //To move a file or folder to a new location:
        System.IO.File.Move(sourceFile, destinationFile);
        //System.IO.File.Move(soutceMetaFile, destinationMetaFile);
        yield return new WaitForSeconds(2);
        midiFilePlayer.MPTK_Stop();
        //midiFilePlayer.midiLoaded.MPTK_LoadFile(@"C:/Users/migue/OneDrive/Ambiente de Trabalho/IATK-master - Cópia/Assets/MidiPlayer/Resources/MidiDB/" + fileName + ".bytes");
        //print(midiFilePlayer.midiLoaded.MPTK_Duration);
        yield return new WaitForSeconds(70);
        addFileMPTK();
        
    }
}