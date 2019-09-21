using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;

public class Files : MonoBehaviour
{
    public BluetoothDeviceManager bluetoothDeviceManager;
    
    StreamWriter fr;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region textPrint

    /// <summary>
    /// 데이터 파일로 변환
    /// </summary>
    /// <param name="PPG">입력된 심박수값</param>
    public void textPrint(double[] outputText, string _fileName)
    {
        
        if (File.Exists("./Result/"+_fileName))
        {
            File.Delete("./Result/" + _fileName);

            fr = File.CreateText("./Result/" + _fileName);
            
            //fr.WriteLine(outputText.ToString());

            for(int i=0; i<outputText.Length; i++)
            {
                fr.WriteLine(outputText[i]);
            }
            fr.Close();
        }
        else
        {
            fr = File.CreateText("./Result/" + _fileName);

            //fr.WriteLine(outputText.ToString());

            for (int i = 0; i < outputText.Length; i++)
            {
                fr.WriteLine(outputText[i]);
            }
            fr.Close();
        }
    }

    public void textPrint(int[] outputText, string _fileName)
    {

        if (File.Exists("./Result/" + _fileName))
        {
            File.Delete("./Result/" + _fileName);

            fr = File.CreateText("./Result/" + _fileName);

            //fr.WriteLine(outputText.ToString());

            for (int i = 0; i < outputText.Length; i++)
            {
                fr.WriteLine(outputText[i]);
            }
            fr.Close();
        }
        else
        {
            fr = File.CreateText("./Result/" + _fileName);

            //fr.WriteLine(outputText.ToString());

            for (int i = 0; i < outputText.Length; i++)
            {
                fr.WriteLine(outputText[i]);
            }
            fr.Close();
        }
    }

    public void textPrint(int outputText, string _fileName)
    {

        if (File.Exists("./Result/" + _fileName))
        {
            File.Delete("./Result/" + _fileName);

            fr = File.CreateText("./Result/" + _fileName);

            //fr.WriteLine(outputText.ToString());

            fr.WriteLine(outputText);

            fr.Close();
        }
        else
        {
            fr = File.CreateText("./Result/" + _fileName);

            //fr.WriteLine(outputText.ToString());

            fr.WriteLine(outputText);

            fr.Close();
        }
    }

    public void textPrint(float outputText, string _fileName)
    {

        if (File.Exists("./Result/" + _fileName))
        {
            File.Delete("./Result/" + _fileName);

            fr = File.CreateText("./Result/" + _fileName);

            //fr.WriteLine(outputText.ToString());

            fr.WriteLine(outputText);

            fr.Close();
        }
        else
        {
            fr = File.CreateText("./Result/" + _fileName);

            //fr.WriteLine(outputText.ToString());

            fr.WriteLine(outputText);

            fr.Close();
        }
    }

    public void textPrint(double outputText, string _fileName)
    {

        if (File.Exists("./Result/" + _fileName))
        {
            File.Delete("./Result/" + _fileName);

            fr = File.CreateText("./Result/" + _fileName);

            //fr.WriteLine(outputText.ToString());

            fr.WriteLine(outputText);

            fr.Close();
        }
        else
        {
            fr = File.CreateText("./Result/" + _fileName);

            //fr.WriteLine(outputText.ToString());

            fr.WriteLine(outputText);

            fr.Close();
        }
    }

    #endregion textPrint

    [BurstCompile]
    public void textAppend(double outputText, string _fileName)
    {
        if (File.Exists("./Result/" + _fileName))
        {
            fr = File.AppendText("./Result/" + _fileName);

            //fr.WriteLine(outputText.ToString());

            fr.WriteLine(outputText);

            fr.Close();
        }
        else
        {
            fr = File.CreateText("./Result/" + _fileName);

            //fr.WriteLine(outputText.ToString());

            fr.WriteLine(outputText);

            fr.Close();
        }
    }

    [BurstCompile]
    public void textAppend(double[] outputText, string _fileName)
    {
        if (File.Exists("./Result/" + _fileName))
        {
            fr = File.AppendText("./Result/" + _fileName);

            //fr.WriteLine(outputText.ToString());
            for (int i = 0; i < outputText.Length; i++)
            {
                fr.Write(outputText[i] + "\t");
            }
            fr.WriteLine("");
            fr.Close();
        }
        else
        {
            fr = File.CreateText("./Result/" + _fileName);

            //fr.WriteLine(outputText.ToString());
            for (int i = 0; i < outputText.Length; i++)
            {
                fr.Write(outputText[i] + "\t");
            }
            fr.WriteLine("");
            fr.Close();
        }
    }
}
