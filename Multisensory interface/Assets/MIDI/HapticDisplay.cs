using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System.Numerics;
using System;
using VRTK;


public class HapticDisplay : MonoBehaviour
{
    public TextAsset data;

    public int maxSec;
    // Start is called before the first frame update
    public VRTK_ControllerEvents controllerEventsR;
    public VRTK_ControllerEvents controllerEventsL;

    private float minData;
    private float maxData;

    private int ka;

    private float[] array2;
    void Start()
    {
        ka = 0;
        string[] strings = (data.text.Split('\r','\n'));
        int k = 0;
        for (int i = 0; i != strings.Length; i++)
        {
            if(strings[i] != "")
            {
                k++;
            }
        }

        float[] array = new float[k];
        k = 0;
        
        for (int i = 0; i != strings.Length; i++)
        {
            if (strings[i] != "")
            {
                array[k] = float.Parse(strings[i]);
                k++;
            }
        }

        //Normalizar

        //Encontrar o max e o min
        if(array.Length == 0)
        {
            return;
        }
        float max = array[0];
        float min = array[0];

        for (int i = 0; i != array.Length; i++)
        {
            if (array[i] > max)
                max = array[i];
            if (array[i] < min)
                min = array[i];
        }

        maxData = max;
        minData = min;
        float meanValue = max - min;

        for (int i = 0; i != array.Length; i++)
        {
            array[i] = (array[i] - min) / meanValue;
            array[i] = Mathf.Floor(array[i] * 100) / 100;
            //print(array[i]);
        }

        int numPerGroup;
        numPerGroup = (int)Math.Ceiling((decimal)(array.Length / (maxSec-1)));

        int contI = 0;
        float cont = 0;
        int ks = 0;

        float[] array3 = new float[maxSec];
        for (int i = 0; i != array.Length; i++)
        {
            cont = cont + array[i];
            contI++;
            if(contI == numPerGroup)
            {
                if ((cont / numPerGroup) <= 0.1f)
                    array3[ks] = 0.1f;
                else
                    array3[ks] = (float)Math.Round((cont / numPerGroup) * 10f) / 10f;
                cont = 0;
                contI = 0;
                ks++;
            }
        }
        if(cont > 0f)
        {
            array3[ks] = (float)Math.Round((cont / contI) * 10f) / 10f;
        }

            array2 = array3;
    }

    // Update is called once per frame
    public void playHaptec2()
    {
        //print(ka);
        //print(array2[ka]);
        //print(array2.Length);
        if (ka == array2.Length)
        {
            ka = 0;
        }
        else
        {
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controllerEventsL.gameObject), (float)0.5, 2f, 0.01f);
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controllerEventsR.gameObject), (float)0.5, 2f, 0.01f);
            ka++;
        }
        StartCoroutine(playHaptic());
    }

    public IEnumerator playHaptic()
    {
        //print("haha");
        yield return new WaitForSeconds(2);
        //print("hehe");
        yield return new WaitForSeconds(2);
        for (int i = 0; i != array2.Length; i++)
        {
            //print("HOHO");
            //print(array2[i]);
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controllerEventsL.gameObject),  ((float)0.5 + array2[i]/2), (array2[i]/2),  0.01f);
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controllerEventsR.gameObject),  ((float)0.5 + array2[i]/2), (array2[i]/2), 0.01f);
            yield return new WaitForSeconds(1);
        }
        /*
        print(ka);
        print(array3[ka]);
        if (ka == array3.Length)
        {
            ka = 0;
        }
        else
        {
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controllerEventsL.gameObject), (float)0.5, array3[ka], 0.01f);
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controllerEventsR.gameObject), (float)0.5, array3[ka], 0.01f);
            ka++;
            yield return new WaitForSeconds(2);
        }*/
    }

    private float normalizeDataPoint(float dataPoint)
    {
        float newdataPoint = (dataPoint - minData) / (maxData - minData);
        newdataPoint = Mathf.Floor(newdataPoint * 100) / 100;
        
        //print(newdataPoint);

        return newdataPoint;
    }

    public void playPoint(float dataPoint)
    {
        float normalizedPoint = normalizeDataPoint(dataPoint);

        VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controllerEventsL.gameObject), ((float)0.5 + normalizedPoint / 2), normalizedPoint, 0.01f);
        VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controllerEventsR.gameObject), ((float)0.5 + normalizedPoint / 2), normalizedPoint, 0.01f);

    }

    public void playNormalizedPoint(float dataPointNormalized)
    {
        if (dataPointNormalized < 0.1f)
            dataPointNormalized = 0.1f;
        VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controllerEventsL.gameObject), ((float)0.5 + dataPointNormalized / 2), dataPointNormalized, 0.01f);
        VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controllerEventsR.gameObject), ((float)0.5 + dataPointNormalized / 2), dataPointNormalized, 0.01f);
    }
}
