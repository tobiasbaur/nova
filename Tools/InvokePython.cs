﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Python.Runtime;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace ssi {
    class InvokePython
    {

        public static void initPython()
        {
            var pythonPath = @"C:\\Program Files\\Python36";
            var path = $"{pythonPath};{Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine)}";
            Environment.SetEnvironmentVariable("Path", path, EnvironmentVariableTarget.Process);
            PythonEngine.PythonHome += pythonPath;

            //TODO change path variable to relative 
            //TODO add scripts path in front of everything else
            PythonEngine.PythonPath += ";PythonScripts";
            PythonEngine.Initialize();
            PythonEngine.BeginAllowThreads();
        }

        public static BitmapImage imageExplainer(string pathModell, byte[] toExplain)
        {
            using (Py.GIL())
            {
                dynamic os = Py.Import("os");
                dynamic sys = Py.Import("sys");

                //Console.WriteLine(os.getcwd());
                //Console.WriteLine(sys.path + "\n");
                //Console.WriteLine(sys.version);
                //Console.WriteLine(os.__file__);
                dynamic limeExplainer = Py.Import("ImageExplainer");
                //byte[] expImg = limeExplainer.explain(pathModell, toExplain);
                var expImg = limeExplainer.explainToOrg(pathModell, toExplain);
                BitmapImage final_img = new BitmapImage();
                final_img.BeginInit();
                final_img.StreamSource = new System.IO.MemoryStream((byte[])expImg);
                final_img.EndInit();
                final_img.Freeze();
                //ImageConverter imageConverter = new ImageConverter();
                //Image final_img = (Image)imageConverter.ConvertFrom((byte[])expImg);
                //final_img.Save("test.jpg

                return final_img;
            }
        }

        //TODO
        private static void textExplainer()
        {

        }

        //TODO
        private static void audioExplainer()
        {

        }

        //Needed for debugging purposes
        static void AttachDebugger()
        {
            Console.WriteLine("waiting for .NET debugger to attach");
            while (!Debugger.IsAttached)
            {
                Thread.Sleep(100);
            }
            Console.WriteLine(".NET debugger is attached");

        }

        public static Dictionary<string, double> correlate(AnnoList anno1, AnnoList anno2)
        {

            int nValues = Math.Min(anno1.Count, anno2.Count);

            List<double> anno1ValuesL = new List<double>();
            List<double> anno2ValuesL = new List<double>();
            for (int i = 0; i < nValues; i++)
            {
                anno1ValuesL.Add(anno1[i].Score);
                anno2ValuesL.Add(anno2[i].Score);
            }

            List<string> scriptsName = new List<string>();

            foreach (string fp in Directory.GetFiles("Scripts", "*.py", SearchOption.TopDirectoryOnly).Where(s => !s.Contains("__init__.py")))
            {
                Console.WriteLine(Path.GetFileNameWithoutExtension(fp));
                scriptsName.Add(Path.GetFileNameWithoutExtension(fp));
            }

            Dictionary<string, double> measureValueDic= new Dictionary<string, double>();

            using (Py.GIL())
            {
                dynamic sys = PythonEngine.ImportModule("sys");
                dynamic spearman = Py.Import("SpearmanR");
                dynamic kendall = Py.Import("KendallTau");
                dynamic scriptType = Py.Import("EnumScriptType");
                string typeT = scriptType.ScriptType.CORRELATION.name;

                dynamic os = Py.Import("os");
                Console.WriteLine(os.getcwd());


                foreach (string sc in scriptsName)
                {
                    dynamic temp = Py.Import(sc);

                    Console.WriteLine(sc);
                    Console.WriteLine(temp.getType());
                    Console.WriteLine(typeT);

                    if(typeT.Equals((string)temp.getType()))
                    {
                        measureValueDic[sc] = temp.correlate(anno1ValuesL, anno2ValuesL);
                    }
                }

                //Only for cross debugging needed
                //AttachDebugger();
                
            }
            return measureValueDic;
        }

    }
}
