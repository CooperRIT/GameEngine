using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using PrimalEditor.Utilities;
using System.Collections.ObjectModel;
using System.IO;

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
                    ValidateProjectPath();
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
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }

        private bool _isValid = false;
        public bool IsValid
        {
            get => _isValid;

            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;

            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();

        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates
        {
            get;
        }

        private bool ValidateProjectPath()
        {
            string path = ProjectPath;

            if (!path.EndsWith(@"\"))
            {
                path += @"\";
            }

            path += $@"{ProjectName}\";

            IsValid = false;

            if (string.IsNullOrWhiteSpace(ProjectName))
            {
                ErrorMessage = "Please type in a project name";
            }

            //If it does find any of these invalid characters it will return a value other then -1 making this statement true
            else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                ErrorMessage = "Invalid character(s) used in the project name";
            }

            else if (string.IsNullOrWhiteSpace(ProjectPath))
            {
                ErrorMessage = "Please select a valid project folder";
            }

            else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                ErrorMessage = "Invalid character(s) used in the project path";
            }

            else if (Directory.Exists(ProjectPath) && Directory.EnumerateFileSystemEntries(path).Any())
            {
                ErrorMessage = "Selected folder already exists";
            }
            else
            {
                ErrorMessage = string.Empty;
                IsValid = true;
            }

            return IsValid;
        }

        /// <summary>
        /// Returns the location of the newly created game project
        /// </summary>
        /// <param name="projectTemplate"></param>
        /// <returns></returns>
        public string CreateProject(ProjectTemplate projectTemplate)
        {
            //Validate the project path one more time just in case
            if(!ValidateProjectPath())
            {
                return string.Empty;
            }

            string path = ProjectPath;

            if (!path.EndsWith(@"\"))
            {
                path += @"\";
            }

            path = $@"{ProjectPath}{ProjectName}\";

            try
            {
                //If it does not exist we need to create it
                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                foreach(string folder in projectTemplate.Folders)
                {
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder)));
                }

                DirectoryInfo dirInfo = new DirectoryInfo(path + @".primal\");

                dirInfo.Attributes |= FileAttributes.Hidden;

                /*File.Copy(projectTemplate.IconFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Icon.png")));
                File.Copy(projectTemplate.IconFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Screenshot.png")));

                Project project = new Project(ProjectName, path);


                Serializer.ToFile(project, path + $"{ProjectName}" + Project.Extension);*/

                string projectXml = File.ReadAllText(projectTemplate.ProjectFilePath);

                projectXml = string.Format(projectXml, ProjectName, ProjectPath);

                string projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));

                File.WriteAllText(projectPath, projectXml);

                return path;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
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
                //Just to be sure
                ValidateProjectPath();
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