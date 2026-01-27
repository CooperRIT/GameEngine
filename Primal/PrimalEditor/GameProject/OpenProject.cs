using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PrimalEditor.GameProject
{
    [DataContract]
     public class ProjectData
    {
        [DataMember]
        public string ProjectName { get; set; }

        [DataMember]
        public string ProjectPath {  get; set; }

        [DataMember]
        public DateTime Date {  get; set; }

        public string FullPath { get => $"{ProjectPath}{ProjectName}{Project.Extension}"; }

        public byte[] Icon { get; set; }

        public byte[] ScreenShot { get; set; }
    }

    [DataContract]
    public class ProjectDataList
    {
        [DataMember]
        public List<ProjectData> Projects { get; set; }
    }


    public  class OpenProject
    {
        //App data location
        private static readonly string _applicationDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\PrimalEditor\";

        private static readonly string _projectDataPath;

        private static readonly ObservableCollection<ProjectData> _projects = new ObservableCollection<ProjectData>();

        public static ReadOnlyObservableCollection<ProjectData> Projects 
        { get; }

        static OpenProject()
        {
            try
            {
                if (!Directory.Exists(_applicationDataPath))
                {
                    Directory.CreateDirectory(_applicationDataPath);
                }

                _projectDataPath = $@"{_applicationDataPath}ProjectData.xml";

                Projects = new ReadOnlyObservableCollection<ProjectData>(_projects);

                ReadProjectData();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private static void ReadProjectData()
        {
            if(File.Exists(_projectDataPath))
            {
                List<ProjectData> projects = Serializer.FromFile<ProjectDataList>(_projectDataPath).Projects.OrderByDescending(x => x.Date).ToList();
                _projects.Clear();

                foreach(ProjectData projectData in projects)
                {
                    if(File.Exists(projectData.FullPath))
                    {
                        projectData.Icon = File.ReadAllBytes($@"{projectData.ProjectPath}\.Primal\Icon.png");
                        projectData.ScreenShot = File.ReadAllBytes($@"{projectData.ProjectPath}\.Primal\Screenshot.png");
                        _projects.Add(projectData);
                    }
                }
            }
        }

        public static Project Open(ProjectData projectData)
        {
            ReadProjectData();

            ProjectData project = _projects.FirstOrDefault(x => x.FullPath == projectData.FullPath);

            if (project != null)
            {
                project.Date = DateTime.Now;
            }
            //This means that the method was called with project data that was not found in thelist 
            else
            {
                project = projectData;
                project.Date = DateTime.Now;
                _projects.Add(project);
            }

            WriteProjectData();

            return Project.Load(project.FullPath);
        }

        private static void WriteProjectData()
        {
            List<ProjectData> projects = _projects.OrderBy(x => x.Date).ToList();

            Debug.WriteLine(_projects.Count);

            Serializer.ToFile(new ProjectDataList() { Projects = projects }, _projectDataPath);
        }
    }
}
