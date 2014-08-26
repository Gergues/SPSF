#region Using Directives

using System;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using Microsoft.Practices.ComponentModel;
using Microsoft.Practices.RecipeFramework;
using Microsoft.Practices.RecipeFramework.Library;
using Microsoft.Practices.RecipeFramework.Services;
using Microsoft.Practices.RecipeFramework.VisualStudio;
using Microsoft.Practices.RecipeFramework.VisualStudio.Templates;
using EnvDTE;
using Microsoft.Practices.RecipeFramework.VisualStudio.Library.Templates;
using Microsoft.Practices.Common.Services;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ComponentModel.Design;
using System.Xml;

#endregion

namespace SPALM.SPSF.Library.Actions
{
    /// <summary>
    /// Adds a new Elements definition, e.g. a contenttype and registers the element in the parent feature
    /// </summary>
    [ServiceDependency(typeof(DTE))]
  public class AddSPDataAction : BaseItemAction
    {
        public AddSPDataAction() : base()
        {

        }

        /// <summary>
        /// Contains the name of the element, used to create filenames and folders
        /// </summary>
        [Input(Required = true)]
        public string ElementsName { get; set; }

        /// <summary>
        /// Contains the type of element, e.g. "ContentTypes". This is used to summarize elements of the same type (e.g. in feature "ContentTypes.xml" or in a folder "ContentTypes")
        /// </summary>
        [Input(Required = true)]
        public string ElementsCategory { get; set; }

        /// <summary>
        /// Required for Visual Studio 2010 project template 
        /// </summary>
        [Input(Required = true)]
        public string SPDataTemplate { get; set; }

        [Output]
        public ProjectItem CreatedElementFolder { get; set; }

        public override void Execute()
        {
            DTE dte = (DTE)this.GetService(typeof(DTE));
            Project project = this.GetTargetProject(dte);

            //1. get correct parameters ("$(FeatureName)" as "FeatureX")       
            string evaluatedSPDataTemplate = EvaluateParameterAsString(dte, SPDataTemplate);
            string evaluatedSPDataCategory = EvaluateParameterAsString(dte, ElementsCategory);
            string evaluatedSPDataName = EvaluateParameterAsString(dte, ElementsName);
            string evaluatedDeploymentPath = EvaluateParameterAsString(dte, DeploymentPath);

            //2. create the Element
            if (Helpers2.IsSharePointVSTemplate(dte, project))
            {
              AddSPDataDefinitionToVSTemplate(dte, project, evaluatedSPDataTemplate, evaluatedSPDataCategory, evaluatedSPDataName);
            }            
        }


        private void AddSPDataDefinitionToVSTemplate(DTE dte, Project project, string evaluatedSPDataTemplate, string evaluatedSPDataCategory, string evaluatedSPDataName)
        {
            string spDataContent = GenerateContent(evaluatedSPDataTemplate, "SharePointProjectItem.spdata");

            //1. create the folder for the content type, which means adding the spdata stuff to a folder in the project

            string targetFolder = evaluatedSPDataCategory + @"\" + evaluatedSPDataName;
            ProjectItems contentTypeFolder = null;
            ProjectItem spDataItem = Helpers2.AddFileToProject(dte, project, targetFolder, "SharePointProjectItem.spdata", spDataContent, true, false, out contentTypeFolder);

            ProjectItem elementFolder = Helpers.FindProjectItemByPath(project, targetFolder);

            Helpers2.AddVSElementToVSPackage(dte, project, elementFolder);

            CreatedElementFolder = elementFolder;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Undo()
        {
            //TODO: Delete the created projectitems
        }
    }
}