using System.Diagnostics;
using System.Threading;
using System;
using UnityEngine;

public class CrushReport
{
    public static string ue_filename;
    public static string ue_info;
    public static void Write(string afilename, string info, Exception ex)
    {
        if (!System.IO.Directory.Exists(Application.persistentDataPath + "/BuildTest")) 
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/BuildTest"); 
        }
        //object obj = null;
        //Console.WriteLine(obj.ToString());
        System.IO.Directory.CreateDirectory("./BuildTest/Logs/crash");

        //DateTime.Now.ToString("yyyyMMddHHmmss")

        //string filename = string.Format("./BuildTest/Logs/crash/{0}_{1}.txt", afilename, DateTime.Now.ToString("yyyyMMddHHmmss"));
        string filename = string.Format("/BuildTest{0}_{1}.txt", afilename, DateTime.Now.ToString("yyyyMMddHHmmss"));
        System.IO.StreamWriter sw = new System.IO.StreamWriter(new System.IO.FileStream(filename, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite));

        string fheader = string.Format("Occur: {0}  ThreadID: [{1}]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"), Thread.CurrentThread.ManagedThreadId);
        //System.Reflection.Assembly.GetExecutingAssembly().
        if (ex != null)
            sw.WriteLine("Source : [" + ex.Source + "]");
        if (ex != null)
            sw.WriteLine("TargetSite : [" + ex.TargetSite.Name + "]");
        sw.WriteLine(fheader);
        sw.WriteLine(info);
        if (ex != null)
        {
            sw.WriteLine(ex.GetType().ToString());
            sw.WriteLine(ex.Message);
            sw.WriteLine(ex.StackTrace);
        }

        // Create a StackTrace that captures filename, 
        // line number and column information.
        sw.WriteLine();
        sw.WriteLine();
        StackTrace st = new StackTrace(true);
        string stackIndent = "";
        for (int i = 0; i < st.FrameCount; i++)
        {
            // Note that at this level, there are four 
            // stack frames, one for each method invocation.
            StackFrame sf = st.GetFrame(i);
            sw.WriteLine();

            sw.WriteLine(stackIndent + " Method: {0} => {1}",
                sf.GetMethod().ReflectedType, sf.GetMethod());
            sw.WriteLine(stackIndent + " File: {0}",
                sf.GetFileName());
            sw.WriteLine(stackIndent + " Line Number: {0}",
                sf.GetFileLineNumber());
            stackIndent += "  ";
        }

        sw.Close();

    }

    public static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        Exception ex = (Exception)args.ExceptionObject;

        try
        {
            CrushReport.Write("_UE", "UnhandledException", ex);
        }
        catch
        {

        }

        Console.WriteLine(string.Format("MyHandler caught : {0} ", ex.Message));
        Console.WriteLine(string.Format("Runtime terminating: {0} ", args.IsTerminating));
        //Console.WriteLine("Runtime terminating: {0}", args.IsTerminating);
        Environment.Exit(1);
    }
}
//try
//{ }
//catch (Exception ex)
//{
//    CrushReport.Write("", "", ex)
//}
