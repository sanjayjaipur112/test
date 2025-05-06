using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace WASenderStandalone
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Create an instance of the WASender main form using reflection
            Type waSenderFormType = Type.GetType("WASender.WaSenderForm, WASender");
            if (waSenderFormType != null)
            {
                object waSenderForm = Activator.CreateInstance(waSenderFormType, new object[] { args });
                Application.Run((Form)waSenderForm);
            }
            else
            {
                MessageBox.Show("Failed to load WASender application. Please reinstall the application.", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // Get the assembly name
            string assemblyName = new AssemblyName(args.Name).Name;
            
            // Check if the assembly is embedded as a resource
            string resourceName = $"WASenderStandalone.EmbeddedAssemblies.{assemblyName}.dll";
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            
            if (executingAssembly.GetManifestResourceInfo(resourceName) != null)
            {
                using (Stream stream = executingAssembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        byte[] assemblyData = new byte[stream.Length];
                        stream.Read(assemblyData, 0, assemblyData.Length);
                        return Assembly.Load(assemblyData);
                    }
                }
            }
            
            // If not embedded, try to load from the application directory
            string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{assemblyName}.dll");
            if (File.Exists(assemblyPath))
            {
                return Assembly.LoadFrom(assemblyPath);
            }
            
            return null;
        }
    }
}
