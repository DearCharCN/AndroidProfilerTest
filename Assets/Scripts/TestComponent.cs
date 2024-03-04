using System.Text;
using UnityEngine;

public class TestComponent : MonoBehaviour
{
    NativeBridge nativeClass;

    void Start()
    {
        nativeClass = new NativeBridge();
    }

    public void OnClick()
    {
        var info = nativeClass.getMemoryInfo();
        if (info == null)
            return;

        StringBuilder sb = new StringBuilder();
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
}
