using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using MidiPlayerTK;

public class AnimationIATKMidi : MonoBehaviour
{
    public bool syncAnimation;
    private bool ColorNomal;

    public MidiFilePlayer midiFilePlayer;

    public Visualisation visualisation;
    //public BigMesh bigmesh;

    private Color[] originalColours;

    private int lastPointChanged;

    // Start is called before the first frame update
    void Start()
    {
        //print("Atribute :" + visualisation.dataSource[0].Identifier);
        //print("OLA: " +  i + " : "+ visualisation.dataSource[0].Data[i]);
        //for(int i = 0; i != visualisation.dataSource.DataCount; i++)

        lastPointChanged = 0;
        ColorNomal = false;


        View[] views = visualisation.GetComponentsInChildren<View>();

        foreach (var view in views)
        {
            originalColours = view.BigMesh.getColors();
        }

        //visualisation.colorPaletteDimension = "type";
        /*
        print(visualisation.coloursPalette.Length);

        
        for(int i = 0; i != visualisation.coloursPalette.Length; i++)
        {
            print("a1: " + visualisation.coloursPalette[i].a);
            print("r: " + visualisation.coloursPalette[i].b);
            print("g: " + visualisation.coloursPalette[i].g);
            print("b: " + visualisation.coloursPalette[i].b);
            //visualisation.coloursPalette[i] = Color.cyan;
            visualisation.coloursPalette[i].a = 1;
            print("a2: " + visualisation.coloursPalette[i].a);
        }
        */
        //visualisation.updateProperties();
        //visualisation.colorPaletteDimension = "Undefined";
        /*
        ViewBuilder vb = new ViewBuilder(MeshTopology.Points, "wine dataset").
            initialiseDataView(dataSouce.DataCount).
            setDataDimension(dataSouce["type"].Data, ViewBuilder.VIEW_DIMENSION.X).
            setDataDimension(dataSouce["ph"].Data, ViewBuilder.VIEW_DIMENSION.Y).
            setDataDimension(dataSouce["alcohol"]).
            setColors
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (syncAnimation)
        {
            if (midiFilePlayer.MPTK_TickCurrent > 0 && midiFilePlayer.MPTK_TickLast > 0)
            {
                float aux = (float)midiFilePlayer.MPTK_TickCurrent / midiFilePlayer.MPTK_TickLast;

                //print(aux);
                //print(midiFilePlayer.MPTK_TickLast);

                int aux2 = (int)Mathf.Round(visualisation.dataSource.DataCount * aux);

                //print("aux2: " + aux2);

                if (aux2 != lastPointChanged)
                {
                    lastPointChanged = aux2;
                    updateColores();
                }
            }
        }
        else
        {
            //if(!ColorNomal)
            //{
            //    ColorNomalize();
            //    ColorNomal = true;
            //}
            if (midiFilePlayer.MPTK_TickCurrent > 0 && midiFilePlayer.MPTK_TickLast > 0)
            {
                float aux = (float)midiFilePlayer.MPTK_TickCurrent / midiFilePlayer.MPTK_TickLast;

                //print(aux);
                //print(midiFilePlayer.MPTK_TickLast);

                int aux2 = (int)Mathf.Round(visualisation.dataSource.DataCount * aux);

                //print("aux2: " + aux2);

                if (aux2 != lastPointChanged)
                {
                    lastPointChanged = aux2;
                    updateColoresHilight();
                }
            }
        }
    }

    private void updateColores()
    {
        Color[] colours = new Color[visualisation.dataSource.DataCount];

        //print("V: " +visualisation.dataSource.DataCount);
        //print("X: " + bigmesh.getColors().Length);
        //print("VERTICES: " + bigmesh.getBigMeshVertices().Length);

        
        for (int i = 0; i != visualisation.dataSource.DataCount; i++)
        {
            if(i < lastPointChanged)
            {
                colours[i].a = 1;
                colours[i] = originalColours[i];
            } 
            if(i == lastPointChanged)
            {
                colours[i].a = 1;
                colours[i] = Color.yellow;
            }
            if (i > lastPointChanged)
            {
                colours[i] = Color.clear;
            }
        }

        //visualisation.bigmesh.updateBigMeshColours(colours);
        //visualisation.theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.Colour);

        View[] views = visualisation.GetComponentsInChildren<View>();

        // create the new view reference list
        foreach (var view in views)
        {
            view.BigMesh.updateBigMeshColours(colours);
        }
    }

    private void updateColoresHilight()
    {
        Color[] colours = new Color[visualisation.dataSource.DataCount];

        //print("V: " +visualisation.dataSource.DataCount);
        //print("X: " + bigmesh.getColors().Length);
        //print("VERTICES: " + bigmesh.getBigMeshVertices().Length);


        for (int i = 0; i != visualisation.dataSource.DataCount; i++)
        {
            if (i < lastPointChanged)
            {
                colours[i].a = 1;
                colours[i] = originalColours[i];
            }
            if (i == lastPointChanged)
            {
                colours[i].a = 1;
                colours[i] = Color.yellow;
            }
            if (i > lastPointChanged)
            {
                colours[i].a = 1;
                colours[i] = originalColours[i];
            }
        }

        //visualisation.bigmesh.updateBigMeshColours(colours);
        //visualisation.theVisualizationObject.UpdateVisualisation(AbstractVisualisation.PropertyType.Colour);

        View[] views = visualisation.GetComponentsInChildren<View>();

        // create the new view reference list
        foreach (var view in views)
        {
            view.BigMesh.updateBigMeshColours(colours);
        }
    }


    private void ColorNomalize()
    {
        View[] views = visualisation.GetComponentsInChildren<View>();

        foreach (var view in views)
        {
            view.BigMesh.updateBigMeshColours(originalColours);
        }
    }

    public void animationButton()
    {
        if (syncAnimation)
        {
            syncAnimation = false;
            ColorNomal = false;
        }
            
        else
            syncAnimation = true;
    }

}
