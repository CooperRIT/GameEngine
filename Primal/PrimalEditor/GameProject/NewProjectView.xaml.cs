using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrimalEditor.GameProject
{
    /// <summary>
    /// Interaction logic for NewProjectView.xaml
    /// </summary>
    public partial class NewProjectView : UserControl
    {
        public NewProjectView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the data that we made from the data context and uses that to call the Create Project Method with the selcted template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCreateButtonClick(object sender, RoutedEventArgs e)
        {
            NewProject vm = DataContext as NewProject;

            string projectPath = vm.CreateProject(templateListBox.SelectedItem as ProjectTemplate);

            bool dialogResult = false;

            Window win = Window.GetWindow(this);

            if(!string.IsNullOrEmpty(projectPath))
            {
                dialogResult = true;
                Project project = OpenProject.Open(new ProjectData() { ProjectName = vm.ProjectName, ProjectPath = projectPath });
            }
            win.DialogResult = dialogResult;
            win.Close();
        }
    }
}
