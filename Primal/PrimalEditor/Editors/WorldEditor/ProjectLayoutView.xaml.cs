using PrimalEditor.GameProject;
using PrimalEditor.Components;
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
using PrimalEditor.Utilities;

namespace PrimalEditor.Editors
{
    /// <summary>
    /// Interaction logic for ProjectLayoutView.xaml
    /// </summary>
    public partial class ProjectLayoutView : UserControl
    {
        public ProjectLayoutView()
        {
            InitializeComponent();
        }

        private void OnAddGameEntity_Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Scene vm = btn.DataContext as Scene;
            vm.AddGameEntityCommand.Execute(new GameEntity(vm) { Name = "EmptyGameEntity"});
        }

        private void OnGameEntities_Listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GameEntityView.Instance.DataContext = null;
            ListBox listBox = sender as ListBox;

            

            var newSelection = listBox.SelectedItems.Cast<GameEntity>().ToList();

            var previousSelection = newSelection.Except(e.AddedItems.Cast<GameEntity>().Concat(e.RemovedItems.Cast<GameEntity>().ToList()));


            Project.UndoRedo.Add(new UndoRedoAction(
                () =>
                {
                    listBox.UnselectAll();
                    foreach (var item in previousSelection)
                    {
                        (listBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem).IsSelected = true;
                    }
                },
                () =>
                {
                    listBox.UnselectAll();
                    foreach (var item in newSelection)
                    {
                        (listBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem).IsSelected = true;
                    }
                },
                "Selection Changed"
                ));

            MSGameEntity msEntity = null;

            if(newSelection.Any())
            {
                msEntity = new MSGameEntity(newSelection);
            }

            GameEntityView.Instance.DataContext = msEntity;
        }
    }
}
