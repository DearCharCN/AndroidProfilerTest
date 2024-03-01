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

    public MemoryInfo getMemoryInfo()
    {
#if !UNITY_ANDROID || UNITY_EDITOR
        return null;
#endif
        Debug.Log("android");
        AndroidJavaObject mInfo = javaClass.Call<AndroidJavaObject>("getMemonry");
        Debug.Log("call success");
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
        return memoryInfo;
    }

    void example()
    {
    }
}

public class MemoryInfo
{
    //Java Runtime API
    public long dalvikHeapSize;
    public long dalvikHeapAlloc;
    public long dalvikHeapFree;

    //c++层获取的数据
    public long nativeHeapSize;
    public long nativeHeapAlloc;
    public long nativeHeapFree;

    //Java Debug API获取的数据
    public int dalvikPss;
    public int dalvikPrivateDirty;
    public int dalvikSharedDirty;

    public int nativePss;
    public int nativePrivateDirty;
    public int nativeSharedDirty;

    public int otherPss;
    public int otherPrivateDirty;
    public int otherSharedDirty;
}


