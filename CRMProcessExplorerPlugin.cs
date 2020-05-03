using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace CRMProcessExplorer
{
    // Do not forget to update version number and author (company attribute) in AssemblyInfo.cs class
    // To generate Base64 string for Images below, you can use https://www.base64-image.de/
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "CRM Process Explorer"),
        ExportMetadata("Description", "This is a description for my first plugin"),
        // Please specify the base64 content of a 32x32 pixels image
        ExportMetadata("SmallImageBase64", "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAQAAADZc7J/AAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAAAmJLR0QA/4ePzL8AAAAJcEhZcwAACxMAAAsTAQCanBgAAAAHdElNRQfkBBwXJDEXEem6AAADpUlEQVRIx53Ve2jVZRgH8M85O+rmNrfpnBOb5dQwUOclWoXkBbV/LJdKZaQJSSKaZUIXsqhIuhtRpuEFnUWZaAppkZCYG4ZpXkhNiY3cRW3Tuenxcraz9Ye/Haczhb7//Z7neb/v97n93iS3QpLwzdyhWxzPtFBvDf6y1wE1Wtrz3xzNLovqYZynTZarypkbK8g1RE8ZojY6fQOdWQpNN0GtxVY71/6mWapFxcUU/aeajh5UIqZYr/bOdEPN8ZW4SQlbF/1kXBeXa5mYLfLaUzxuvZdFjQU5Xlei3I/uvC4uzYearJN5rXmUI4qMUW4geMtv5ppov2XShHQy1kwDArVrxL3atoM5dlouoquJOiNkrefAsxr86hNfqBdzxCiQ74CT7rlKsECloW30dFDsBdDNLMsdVWORJ5Q5Gih8SpMVIq2FOWhNm4mImO5bg9o08TaDdcA0jeaCLCVOGkgEI+R7Qxw5ZssXlmKeaiTLFXJKpUp0UqhJJaizwWKj/AGfq9AHvGm3BV4K0hlhkyrVfjBOCCOdt0lqoGuYs74RoYtSv+iMDKVmJ4QXKlNrs63qnTAeg9X4TsfA39U++2Rzh3JfC6O/Mg8H7iSrVRsvSQdTNdgqRbJtjuufqNRmFfqHpUvVoBm1yrxjvmykK7DRT+IabVSiQK5LDsuzykNCaHJaspSwsFDQjjpz7fK2R9HkgtrgrmZNWrRgiaXusiRoZZNmzWEXXNYtoPjTAocMw3lb3B1swmj32+MkjpljhV76IKyLqHMRZ9W6Xbo6UG+PsfJUWCrHWrvlmOKcj1zSUYq4vhocR7Jeqp0hYp2ziTnsaJUaBcGwvKLUfmsUorMP7FUi6mfpyFfpsyuH5mnxfEAwXoN3E1MZkipTBClec9E/KuwyBkxx3uQrYYOcslMWmOmyqUgxWoHkRMveE/O7e+UFcRFrHdCj1b1So2lggMPKzbDEBTW2+dRApNmlxn1t1m24KvOvfhY6ZX8wzqMdE3fRJhv8rcUqEWm2O643UhXJErHSDt2vEoQsFLdaepDSPEVSddDXDnU+tkFUqUwMUm6kSY544No/UqZ1Gr2fWJVWTFChRb3tQenGiXrReo9phzxbxSyVe401bKAiwwJtTBb3pTmGJCxt0EuxmJ3GJzauPYrENIuq8kxr9m2RZoYXZPtesd3q2j9ksj0iVb0T9jvZnoCQfp40SU/H7XHQIaViboLQDW3dFRiurwzlFmnwv5F0y8fXv+LEHut4zWMHAAAAJXRFWHRkYXRlOmNyZWF0ZQAyMDIwLTA0LTI4VDIzOjM2OjQ5LTA0OjAwwmrl3gAAACV0RVh0ZGF0ZTptb2RpZnkAMjAyMC0wNC0yOFQyMzozNjo0OS0wNDowMLM3XWIAAAAASUVORK5CYII="),
        // Please specify the base64 content of a 80x80 pixels image
        ExportMetadata("BigImageBase64", "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"),
        ExportMetadata("BackgroundColor", "#606060"),
        ExportMetadata("PrimaryFontColor", "White"),
        ExportMetadata("SecondaryFontColor", "White")]
    public class CRMProcessExplorerPlugin : PluginBase
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new PluginControl();
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        public CRMProcessExplorerPlugin()
        {
            // If you have external assemblies that you need to load, uncomment the following to 
            // hook into the event that will fire when an Assembly fails to resolve
            // AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveEventHandler);
        }

        /// <summary>
        /// Event fired by CLR when an assembly reference fails to load
        /// Assumes that related assemblies will be loaded from a subfolder named the same as the Plugin
        /// For example, a folder named Sample.XrmToolBox.MyPlugin 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Assembly loadAssembly = null;
            Assembly currAssembly = Assembly.GetExecutingAssembly();

            // base name of the assembly that failed to resolve
            var argName = args.Name.Substring(0, args.Name.IndexOf(","));

            // check to see if the failing assembly is one that we reference.
            List<AssemblyName> refAssemblies = currAssembly.GetReferencedAssemblies().ToList();
            var refAssembly = refAssemblies.Where(a => a.Name == argName).FirstOrDefault();

            // if the current unresolved assembly is referenced by our plugin, attempt to load
            if (refAssembly != null)
            {
                // load from the path to this plugin assembly, not host executable
                string dir = Path.GetDirectoryName(currAssembly.Location).ToLower();
                string folder = Path.GetFileNameWithoutExtension(currAssembly.Location);
                dir = Path.Combine(dir, folder);

                var assmbPath = Path.Combine(dir, $"{argName}.dll");

                if (File.Exists(assmbPath))
                {
                    loadAssembly = Assembly.LoadFrom(assmbPath);
                }
                else
                {
                    throw new FileNotFoundException($"Unable to locate dependency: {assmbPath}");
                }
            }

            return loadAssembly;
        }
    }
}