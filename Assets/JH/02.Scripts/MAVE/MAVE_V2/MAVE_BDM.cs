using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using InTheHand.Devices.Bluetooth;
using InTheHand.Devices.Bluetooth.Rfcomm;
using InTheHand.Devices.Enumeration;
using Unity.Burst;
using System.Threading;

public class MAVE_BDM : MonoBehaviour
{
    public DeviceInformation[] devInfo;

    // Start is called before the first frame update
    void Start()
    {
        Thread findThread = new Thread(new ThreadStart(finder));
        findThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Hello Unity");
    }

    public void finder()
    {
        var _devInfo = FindAll(RfcommServiceId.SerialPort);
        devInfo = _devInfo;
        Debug.Log(devInfo);
    }

    public DeviceInformation[] FindAll(RfcommServiceId serviceId)
    {
        return DeviceInformation.FindAll(RfcommDeviceService.GetDeviceSelector(serviceId)).ToArray();
    }
}
