using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeBridge
{
    private UnityEngine.AndroidJavaObject javaClass;

    private UnityEngine.AndroidJavaObject memoryInfoClass;

    public NativeBridge()
    {
        javaClass = new UnityEngine.AndroidJavaObject("com.dc.androidprofiler.UnitySupport");
        memoryInfoClass = new UnityEngine.AndroidJavaObject("com.dc.androidprofiler.MemoryInfo");
    }

    static AndroidJavaObject m_unityActivity;

    public static AndroidJavaObject GetUnityActivity()
    {
        if (m_unityActivity != null)
            return m_unityActivity;

        // 获取 UnityPlayer 对应的 AndroidJavaClass
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        // 获取当前的 Activity
        AndroidJavaObject activity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        m_unityActivity = activity;
        return activity;
    }

    public MemoryInfo getMemoryInfo()
    {
#if !UNITY_ANDROID || UNITY_EDITOR
        return null;
#endif
        //Debug.Log("android");
        AndroidJavaObject mInfo = javaClass.Call<AndroidJavaObject>("getMemonry");
        //Debug.Log("call success");
        long dalvikHeapSize = mInfo.Get<long>("dalvikHeapSize");
        long dalvikHeapAlloc = mInfo.Get<long>("dalvikHeapAlloc");
        long dalvikHeapFree = mInfo.Get<long>("dalvikHeapFree");

        long nativeHeapSize = mInfo.Get<long>("nativeHeapSize");
        long nativeHeapAlloc = mInfo.Get<long>("nativeHeapAlloc");
        long nativeHeapFree = mInfo.Get<long>("nativeHeapFree");

        int dalvikPss = mInfo.Get<int>("dalvikPss");
        int dalvikPrivateDirty = mInfo.Get<int>("dalvikPrivateDirty");
        int dalvikSharedDirty = mInfo.Get<int>("dalvikSharedDirty");

        int nativePss = mInfo.Get<int>("nativePss");
        int nativePrivateDirty = mInfo.Get<int>("nativePrivateDirty");
        int nativeSharedDirty = mInfo.Get<int>("nativeSharedDirty");

        int otherPss = mInfo.Get<int>("otherPss");
        int otherPrivateDirty = mInfo.Get<int>("otherPrivateDirty");
        int otherSharedDirty = mInfo.Get<int>("otherSharedDirty");

        MemoryInfo memoryInfo = new MemoryInfo();
        memoryInfo.dalvikHeapSize = dalvikHeapSize;
        memoryInfo.dalvikHeapAlloc = dalvikHeapAlloc;
        memoryInfo.dalvikHeapFree = dalvikHeapFree;

        memoryInfo.nativeHeapSize = nativeHeapSize;
        memoryInfo.nativeHeapAlloc = nativeHeapAlloc;
        memoryInfo.nativeHeapFree = nativeHeapFree;

        memoryInfo.dalvikPss = dalvikPss;
        memoryInfo.dalvikPrivateDirty = dalvikPrivateDirty;
        memoryInfo.dalvikSharedDirty = dalvikSharedDirty;

        memoryInfo.nativePss = nativePss;
        memoryInfo.nativePrivateDirty = nativePrivateDirty;
        memoryInfo.nativeSharedDirty = nativeSharedDirty;

        memoryInfo.otherPss = otherPss;
        memoryInfo.otherPrivateDirty = otherPrivateDirty;
        memoryInfo.otherSharedDirty = otherSharedDirty;

        memoryInfo.statsDic = new Dictionary<string, int>();
        AndroidJavaObject statusList = mInfo.Get<AndroidJavaObject>("statusList");
        int listSize = statusList.Call<int>("size");
        for (int i = 0; i + 1 < listSize; i += 2)
        {
            string key = statusList.Call<string>("get", i);
            string value = statusList.Call<string>("get", i + 1);
            int intValue = 0;
            int.TryParse(value, out intValue);
            memoryInfo.statsDic.Add(key, intValue);
        }

        return memoryInfo;
    }

    public CpuInfo getCpuInfo()
    {
#if !UNITY_ANDROID || UNITY_EDITOR
        return null;
#endif
        Debug.Log("android");
        GetPermissions();
        AndroidJavaObject cInfo = javaClass.Call<AndroidJavaObject>("readCpuInfo");
        Debug.Log("call success");
        if (cInfo == null)
            return null;

        CpuInfo cpuInfo = new CpuInfo();
        cpuInfo.totalTimes = cInfo.Get<long>("totaljiffie");
        cpuInfo.usageTimes = cInfo.Get<long>("usagejiffie");
        return cpuInfo;
    }

    public void GetPermissions()
    {
#if !UNITY_ANDROID || UNITY_EDITOR
        return;
#endif
        var unityActivity = GetUnityActivity();
        AndroidJavaClass permissionHelperClass = new AndroidJavaClass("com.dc.androidprofiler.PermissionHelper");
        permissionHelperClass.CallStatic("requestReadExternalStoragePermission", unityActivity);
    }

    public void RegisterBatteryReceiver(Action<AndroidBatteryInfo> batteryChangedCb, Action batteryLowCb, Action batteryOKayCb)
    {
#if !UNITY_ANDROID || UNITY_EDITOR
        return;
#endif
        var unityActivity = GetUnityActivity();
        var callback = new BatteryReceiverCallbackFromJava(batteryChangedCb, batteryLowCb, batteryOKayCb);
        bool result = javaClass.Call<bool>("registerBatteryReceiver", unityActivity, callback);
        Debug.Log("注册电量事件:" + result);
    }

    public void UnRegisterBatteryReceiver()
    {
        bool result = javaClass.Call<bool>("unregisterBatteryReceiver");
        Debug.Log("注销电量事件:" + result);
    }

    public long GetDownloadBytes()
    {
#if !UNITY_ANDROID || UNITY_EDITOR
        return 0;
#endif
        long result = javaClass.Call<long>("getDownloadBytes");
        return result;
    }

    public long GetUploadBytes()
    {
#if !UNITY_ANDROID || UNITY_EDITOR
        return 0;
#endif
        long result = javaClass.Call<long>("getUploadBytes");
        return result;
    }
}

public class MemoryInfo
{
    //Java Runtime API
    //单位 byte
    public long dalvikHeapSize;
    public long dalvikHeapAlloc;
    public long dalvikHeapFree;

    //c++层获取的数据
    public long nativeHeapSize;
    public long nativeHeapAlloc;
    public long nativeHeapFree;

    //Java Debug API获取的数据
    //单位 KB
    public int dalvikPss;
    public int dalvikPrivateDirty;
    public int dalvikSharedDirty;

    public int nativePss;
    public int nativePrivateDirty;
    public int nativeSharedDirty;

    public int otherPss;
    public int otherPrivateDirty;
    public int otherSharedDirty;

    public Dictionary<string, int> statsDic;
}

public class CpuInfo
{
    public long totalTimes;

    public long usageTimes;

    public double GetUsedRate()
    {
        return (double)usageTimes / (double)totalTimes;
    }
}

public class BatteryReceiverCallbackFromJava : AndroidJavaProxy
{
    Action<AndroidBatteryInfo> batteryChangedCb;
    Action batteryLowCb;
    Action batteryOKayCb;
    public BatteryReceiverCallbackFromJava(Action<AndroidBatteryInfo> batteryChangedCb, Action batteryLowCb, Action batteryOKayCb) : base("com.dc.androidprofiler.BatteryReceiverCallbackForUnity")
    {
        this.batteryChangedCb = batteryChangedCb;
        this.batteryLowCb = batteryLowCb;
        this.batteryOKayCb = batteryOKayCb;
    }

    public void batteryChanged(int level, int scale, int temperature, int status, int health, int pluggen, int voltage, int capacity, int current)
    {
        var info = new AndroidBatteryInfo(level, scale, temperature, status, health, pluggen, voltage, capacity, current);
        batteryChangedCb?.Invoke(info);
    }

    public void batteryLow()
    {
        batteryLowCb?.Invoke();
    }

    public void batteryOKay()
    {
        batteryOKayCb?.Invoke();
    }
}

public struct AndroidBatteryInfo
{
    /// <summary>
    /// 电池健康
    /// </summary>
    public AndroidBatteryHealth BatteryHealth { get { return (AndroidBatteryHealth)health; } }
    /// <summary>
    /// 电池状态
    /// </summary>
    public AndroidBatteryStatus BatteryStatus { get { return (AndroidBatteryStatus)status; } }
    /// <summary>
    /// 充电状态
    /// </summary>
    public AndroidBatteryPlugged BatteryPlugged { get { return (AndroidBatteryPlugged)pluggen; } }
    /// <summary>
    /// 温度
    /// 单位 摄氏度
    /// </summary>
    public float Temperature
    {
        get
        {
            return temperature / 10.0f;
        }
    }

    /// <summary>
    /// 电量百分比
    /// 0-100
    /// </summary>
    public int BatteryPct
    {
        get
        {
            return level * 100 / scale;
        }
    }

    /// <summary>
    /// 电池电压 单位伏特(V)
    /// </summary>
    public float Voltage
    {
        get
        {
            return (float)voltage / 1000f;
        }
    }

    /// <summary>
    /// 电池容量 单位毫安时(mAh)
    /// </summary>
    public float Capacity
    {
        get
        {
            return ToMA(capacity);
            //return (float)capacity / 1000f;
        }
    }

    /// <summary>
    /// 电池电流 单位毫安(mA)
    /// </summary>
    public float Current
    {
        get
        {
            return ToMA(current);
            //return (float)current / 1000000f;
        }
    }

    int level;
    int scale;
    int temperature;
    int status;
    int health;
    int pluggen;
    int voltage;
    int capacity;
    int current;


    public AndroidBatteryInfo(int level, int scale, int temperature, int status, int health, int pluggen, int voltage, int capacity, int current)
    {
        this.level = level;
        this.scale = scale;
        this.temperature = temperature;
        this.status = status;
        this.health = health;
        this.pluggen = pluggen;
        this.voltage = voltage;
        this.capacity = capacity;
        this.current = current;
    }

    private float ToMA(float maOrua)
    {
        return maOrua < 10000 ? maOrua : maOrua / 1000f;
    }
}

public enum AndroidBatteryHealth
{
    UNKNOWN = 1,
    GOOD = 2,
    OVERHEAT = 3,
    DEAD = 4,
    OVER_VOLTAGE = 5,
    UNSPECIFIED_FAILURE = 6,
    COLD = 7
}

public enum AndroidBatteryStatus
{
    UNKNOWN = 1,
    CHARGING = 2,
    DISCHARGING = 3,
    NOT_CHARGING = 4,
    FULL = 5
}


public enum AndroidBatteryPlugged
{
    NONE = 0,
    AC = 1,
    USB = 2,
    WIRELESS = 4
}