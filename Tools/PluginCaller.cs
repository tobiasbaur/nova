﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ssi
{   
    public class PluginCaller
    {
        object obj = null;
        Type type = null;

        string dllName;
        string directory;
        bool isLoaded;
        bool IsLoaded { get { return isLoaded; } }

        public const string PLUGIN_FOLDER = "plugins";

        public PluginCaller(string dllPath, string typeName)
        {
            isLoaded = true;
            
            dllName = Path.GetFileNameWithoutExtension(dllPath);
            directory = Environment.CurrentDirectory + "\\" + PLUGIN_FOLDER + "\\" + dllName + "\\";

            if (!downloadDll(dllName))
            {
                MessageTools.Error("Dll not found '" + dllName + ".dll'");
                isLoaded = false;
            }
            else
            {
                Assembly asm = Assembly.LoadFile(directory + dllName + ".dll");

                if (asm != null)
                {
                    foreach (Type type in asm.GetExportedTypes())
                    {
                        if (type.Name == typeName)
                        {
                            this.type = type;
                            obj = Activator.CreateInstance(type);

                            object result = call("dependencies", new Dictionary<string, object>());
                            if (result != null)
                            {
                                string[] dependencies = (string[])result;
                                foreach (string dependency in dependencies)
                                {
                                    if (!downloadDll(dependency))
                                    {
                                        MessageTools.Error("Dll not found '" + dependency + ".dll'");
                                        isLoaded = false;
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }

        public bool downloadDll(string name)
        {
            string path = directory + name + ".dll";

            if (!File.Exists(path))
            {
                try
                {
                    string url = "https://github.com/hcmlab/nova/blob/master/bin/" + PLUGIN_FOLDER + "/" + dllName + "/" + name + ".dll?raw=true";
                    WebClient Client = new WebClient();
                    Directory.CreateDirectory(directory + dllName + "//" + name);
                    Client.DownloadFile(url, directory + dllName + "//" + name + ".dll");
   
                }
                catch
                {
                    MessageTools.Error("Tools for creating Samplelists are not available, please check your internet connection.");
                    return false;
                }

            }

            return true;
        }

        public object call(string name, Dictionary<string,object> args)
        {
            if (obj == null || type == null || !isLoaded)
            {
                return null;
            }

            Type[] varInfo = { args.GetType() };            
            MethodInfo methodInfo = type.GetMethod(name, varInfo);
            if (methodInfo == null)
            {
                return null;                               
            }

            return type.InvokeMember(name, BindingFlags.InvokeMethod, null, obj, new object[] { args });
        }
    }
}
