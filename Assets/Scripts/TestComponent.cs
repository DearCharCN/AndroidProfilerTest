using System.Text;
using UnityEngine;

public class TestComponent : MonoBehaviour
{
    NativeBridge nativeClass;

    void Start()
    {
        nativeClass = new NativeBridge();
    }

    StringBuilder sb = new StringBuilder();

    public void OnMemoryClick()
    {
        var info = nativeClass.getMemoryInfo();
        if (info == null)
            return;

        sb.Clear();
        sb.AppendLine(string.Format("dalvikHeapSize:{0}", info.dalvikHeapSize));
        sb.AppendLine(string.Format("dalvikHeapAlloc:{0}", info.dalvikHeapAlloc));
        sb.AppendLine(string.Format("dalvikHeapFree:{0}", info.dalvikHeapFree));

        sb.AppendLine(string.Format("nativeHeapSize:{0}", info.nativeHeapSize));
        sb.AppendLine(string.Format("nativeHeapAlloc:{0}", info.nativeHeapAlloc));
        sb.AppendLine(string.Format("nativeHeapFree:{0}", info.nativeHeapFree));

        sb.AppendLine(string.Format("dalvikPss:{0}", info.dalvikPss));
        sb.AppendLine(string.Format("dalvikPrivateDirty:{0}", info.dalvikPrivateDirty));
        sb.AppendLine(string.Format("dalvikSharedDirty:{0}", info.dalvikSharedDirty));

        sb.AppendLine(string.Format("nativePss:{0}", info.nativePss));
        sb.AppendLine(string.Format("nativePrivateDirty:{0}", info.nativePrivateDirty));
        sb.AppendLine(string.Format("nativeSharedDirty:{0}", info.nativeSharedDirty));

        sb.AppendLine(string.Format("otherPss:{0}", info.otherPss));
        sb.AppendLine(string.Format("otherPrivateDirty:{0}", info.otherPrivateDirty));
        sb.AppendLine(string.Format("otherSharedDirty:{0}", info.otherSharedDirty));

        if(info.statsDic != null)
        {
            foreach (var kv in info.statsDic)
            {
                sb.AppendLine(string.Format("{0}:{1}", kv.Key, kv.Value));
            }
        }
        Debug.Log(sb.ToString());
    }

    public void OnCpuClick()
    {
        var info = nativeClass.getCpuInfo();
        if (info == null)
            return;

        Debug.Log(string.Format("CPU使用率: {0}", info.GetUsedRate()));
    }

    public void OnBatteryClick()
    {
        nativeClass.RegisterBatteryReceiver(OnBatteryChanged, OnBatteryLow, OnBatteryOKay);
    }

    public void OnUnregisterBatteryClick()
    {
        nativeClass.UnRegisterBatteryReceiver();
    }

    public void OnGetBytesClick()
    {
        long downloadBytes = nativeClass.GetDownloadBytes();
        long uploadBytes = nativeClass.GetUploadBytes();
        Debug.Log(string.Format("downloadBytes:{0}, uploadBytes:{1}", downloadBytes, uploadBytes));
    }

    AndroidBatteryInfo curInfo;

    private void OnBatteryChanged(AndroidBatteryInfo androidBatteryInfo)
    {
        Debug.Log("batteryChanged");
        curInfo = androidBatteryInfo;
        Debug.Log(string.Format("BatteryPct:{0} %, \r\nBatteryHealth:{1}, \r\nTemperature:{2} ℃, \r\nBatteryPlugged:{3}, \r\nBatteryStatus:{4}, \r\nVoltage:{5} V, \r\nCapacity:{6} mAh, \r\nCurrent:{7} mA\r\n", 
            androidBatteryInfo.BatteryPct, androidBatteryInfo.BatteryHealth, 
            androidBatteryInfo.Temperature, androidBatteryInfo.BatteryPlugged, 
            androidBatteryInfo.BatteryStatus, androidBatteryInfo.Voltage, 
            androidBatteryInfo.Capacity, androidBatteryInfo.Current));
    }

    private void OnBatteryLow()
    {
        Debug.Log("OnBatteryLow");
    }

    private void OnBatteryOKay()
    {
        Debug.Log("OnBatteryOKay");
    }
}
