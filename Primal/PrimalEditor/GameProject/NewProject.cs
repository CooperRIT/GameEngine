﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using PrimalEditor.Utilities;
using System.Collections.ObjectModel;

namespace PrimalEditor.GameProject
{
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string ProjectType { get; set; }

        [DataMember]
        public string ProjectFile { get; set; }

        [DataMember]
        public List<string> Folders { get; set; }

        public byte[] Icon { get; set; }

        public byte[] Screenshot { get; set; }

        public string IconFilePath { get; set; }

        public string ScreenshotFilePath { get; set; }

        public string ProjectFilePath { get; set; }
    }


    class NewProject : ViewModelBase
    {
        //This is a more hardcoded way to impliment this
        //TODO: Get the files from the installation location
        private readonly string _templatePath = @"..\..\PrimalEditor\ProjectTemplates";

        private string _projectName = "NewProject";

        public string ProjectName
        {
            get => _projectName;

            set
            {
                if(_projectName != value)
                {
                    _projectName = value;
                    OnPropertyChanged(nameof(ProjectName));
                }
            }
        }


        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\PrimalProject\";

        public string ProjectPath
        {
            get => _projectPath;

            set
            {
                if (_projectPath != value)
                {
                    _projectPath = value;
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }

        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();

        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates
        {
            get;
        }

        public NewProject()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);

            try
            {
                var templateFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templateFiles.Any());
                
                foreach(var file in templateFiles)
                {
                    var template = Serializer.FromFile<ProjectTemplate>(file);
                    template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Icon.png"));
                    template.Icon = File.ReadAllBytes(template.IconFilePath);

                    template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Screenshot.png"));
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);

                    template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));


                    _projectTemplates.Add(template);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //TODO Log errors in editor
            }
        }
    }
}


/*foreach(var file in templateFiles)
{
   var template = new ProjectTemplate()
   {
      ProjectType = "Empty Project",
      ProjectFile = "project.primal",
      Folders = new List<string>() { ".Primal", "Content", "Gamecode"}
   };


   Serializer.ToFile(template, file);
}*/