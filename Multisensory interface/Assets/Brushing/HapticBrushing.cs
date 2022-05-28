using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using TMPro;

public class HapticBrushing : MonoBehaviour
{
    public BrushingAndLinking brushingAndLinking;

    public HapticDisplay hapticDisplay;

    public TextMeshPro moreDetailsText;

    public string identifier;

    public bool color = true;

    private int Lastint;

    // Update is called once per frame
    public void hapticInfo()
    {
        print(brushingAndLinking.GetBrushedIndices().Count);
        if(brushingAndLinking.GetBrushedIndices().Count > 0)
        {
            //float xVal = brushingAndLinking.brushingVisualisations[0].dataSource[brushingAndLinking.brushingVisualisations[0].xDimension.Attribute].Data[brushingAndLinking.GetBrushedIndices()[0]];
            //float yVal = brushingAndLinking.brushingVisualisations[0].dataSource[brushingAndLinking.brushingVisualisations[0].yDimension.Attribute].Data[brushingAndLinking.GetBrushedIndices()[0]];
            //print("X: " + brushingAndLinking.brushingVisualisations[0].dataSource.getOriginalValue(xVal, brushingAndLinking.brushingVisualisations[0].xDimension.Attribute));
            //print("Y: " + brushingAndLinking.brushingVisualisations[0].dataSource.getOriginalValue(yVal, brushingAndLinking.brushingVisualisations[0].yDimension.Attribute));

            //print(brushingAndLinking.GetBrushedIndices()[0]);
            //hapticDisplay.playPoint((float)brushingAndLinking.brushingVisualisations[0].dataSource.getOriginalValue(yVal, brushingAndLinking.brushingVisualisations[0].yDimension.Attribute));

            View[] views = brushingAndLinking.brushingVisualisations[0].GetComponentsInChildren<View>();
            
            float distanceToInput = 9999999999;
            int indiceDistance = 0;
            
            // create the new view reference list
            foreach (var view in views)
            {
                Vector3[] vertices = view.BigMesh.getBigMeshVertices();

                Vector3 bigMeshT = view.BigMesh.transform.position;

                Vector3 bigMrshScale = view.BigMesh.transform.lossyScale;

                Vector3 inputT = brushingAndLinking.input1.transform.position;

                List<int> brushedIndices = brushingAndLinking.GetBrushedIndices();

                //Calcular a distancia entre os pontos e o comando
                for(int i = 0; i != brushedIndices.Count; i++)
                {
                    Vector3 point = new Vector3();
                    point.x = vertices[brushedIndices[i]].x;
                    point.y = vertices[brushedIndices[i]].y;
                    point.z = vertices[brushedIndices[i]].z;
                    point = view.BigMesh.transform.TransformPoint(point);

                    float distanceToInputNew = Mathf.Sqrt(
                        Mathf.Pow(inputT.x - point.x, 2) +
                        Mathf.Pow(inputT.y - point.y, 2) +
                        Mathf.Pow(inputT.z - point.z, 2));

                    if (distanceToInputNew < distanceToInput)
                    {
                        distanceToInput = distanceToInputNew;
                        indiceDistance = i;
                    }
                }
            }

            /*
            if (brushingAndLinking.GetBrushedIndices().Count > 0)
            {


                int realIte = brushingAndLinking.GetBrushedIndices()[indiceDistance];
                //Meter o ponto mais proximo


                Color[] colours = new Color[brushingAndLinking.brushingVisualisations[0].dataSource.DataCount];

                //Mudar a cor do ponto 
                for (int i = 0; i != brushingAndLinking.brushingVisualisations[0].dataSource.DataCount; i++)
                {
                    if (i != realIte)
                    {
                        colours[i].a = 1;
                        colours[i] = Color.white;
                    }
                    if (i == realIte)
                    {
                        colours[i].a = 1;
                        colours[i] = Color.yellow;
                    }
                }

                foreach (var view in views)
                {
                    view.BigMesh.updateBigMeshColours(colours);
                }
            }
            */

            int dimensionCount = brushingAndLinking.brushingVisualisations[0].dataSource.DimensionCount;

            //print("Debug");

            for (int i = 0; i < dimensionCount; i++)
            {
                //print(brushingAndLinking.brushingVisualisations[0].dataSource[i].Identifier);
                if (identifier == brushingAndLinking.brushingVisualisations[0].dataSource[i].Identifier)
                {
                    float identifierValue = brushingAndLinking.brushingVisualisations[0].dataSource[brushingAndLinking.brushingVisualisations[0].dataSource[i].Identifier].Data[brushingAndLinking.GetBrushedIndices()[indiceDistance]];
                    print("Nomalized value: " + identifierValue);
                    hapticDisplay.playNormalizedPoint(identifierValue);
                    print(identifier +": " + brushingAndLinking.brushingVisualisations[0].dataSource.getOriginalValue(identifierValue, brushingAndLinking.brushingVisualisations[0].dataSource[i].Identifier));
                }

            }
        }
    }

    public void ChangeMode()
    {
        if (color)
            color = false;
        else
            color = true;
    }

    private void Start()
    {
        Lastint = -1;
    }

    private void Update()
    {
        if(brushingAndLinking.input1.transform.hasChanged)
        {
            if(color)
            {
                View[] views = brushingAndLinking.brushingVisualisations[0].GetComponentsInChildren<View>();

                float distanceToInput = 9999999999;
                int indiceDistance = 0;

                // create the new view reference list
                foreach (var view in views)
                {
                    Vector3[] vertices = view.BigMesh.getBigMeshVertices();

                    Vector3 bigMeshT = view.BigMesh.transform.position;
                    Vector3 bigMrshScale = view.BigMesh.transform.lossyScale;

                    Vector3 inputT = brushingAndLinking.input1.transform.position;

                    
                    //print("VerticesL: " + vertices.Length);
                    //print("VisualizationTransform: " + bigMeshT);
                    //print("VerticeTrasnform: " + vertices[brushingAndLinking.GetBrushedIndices()[0]]);
                    //print("comand Location: " + brushingAndLinking.input1.transform.position);

                    List<int> brushedIndices = brushingAndLinking.GetBrushedIndices();

                    //Calcular a distancia entre os pontos e o comando
                    for (int i = 0; i != brushedIndices.Count; i++)
                    {
                        Vector3 point = new Vector3();
                        point.x = vertices[brushedIndices[i]].x;
                        point.y = vertices[brushedIndices[i]].y;
                        point.z = vertices[brushedIndices[i]].z;
                        point = view.BigMesh.transform.TransformPoint(point);

                        float distanceToInputNew = Mathf.Sqrt(
                            Mathf.Pow(inputT.x - point.x, 2) + 
                            Mathf.Pow(inputT.y - point.y, 2) + 
                            Mathf.Pow(inputT.z - point.z, 2));
                        if (distanceToInputNew < distanceToInput)
                        {
                            distanceToInput = distanceToInputNew;
                            indiceDistance = i;
                        }
                    }

                    //print(distanceToInput);
                    //print(indiceDsitance);

                }
                
                if (brushingAndLinking.GetBrushedIndices().Count > 0)
                {


                    int realIte = brushingAndLinking.GetBrushedIndices()[indiceDistance];
                    //Meter o ponto mais proximo


                    Color[] colours = new Color[brushingAndLinking.brushingVisualisations[0].dataSource.DataCount];

                    //Mudar a cor do ponto 
                    for (int i = 0; i != brushingAndLinking.brushingVisualisations[0].dataSource.DataCount; i++)
                    {
                        if (i != realIte)
                        {
                            colours[i].a = 1;
                            colours[i] = Color.white;
                        }
                        if (i == realIte)
                        {
                            colours[i].a = 1;
                            colours[i] = Color.yellow;
                            if(Lastint != realIte)
                            {
                                Lastint = realIte;
                                detailsOnDemand(i);
                            }
                            
                        }
                    }

                    foreach (var view in views)
                    {
                        view.BigMesh.updateBigMeshColours(colours);
                    }
                }
                
            }
        }
    }

    private void detailsOnDemand(int detailId)
    {

        moreDetailsText.text = "";

        int dimensionCount = brushingAndLinking.brushingVisualisations[0].dataSource.DimensionCount;

        for (int i = 0; i < dimensionCount; i++)
        {
            string identifier = brushingAndLinking.brushingVisualisations[0].dataSource[i].Identifier;
            float normalizedValue = brushingAndLinking.brushingVisualisations[0].dataSource[identifier].Data[detailId];
            object Value = brushingAndLinking.brushingVisualisations[0].dataSource.getOriginalValue(normalizedValue, identifier);
            
            moreDetailsText.text += "\n" + identifier + ": " + Value;

        }
    }
}
