using PrimalEditor.GameProject;
using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PrimalEditor.Components
{
    [DataContract]
    //For polymorphism
    [KnownType(typeof(Transform))]
    public class GameEntity : ViewModelBase
    {
        //21.50
        private bool _isEnabled = true;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }


        private string _name;

        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        [DataMember]
        public Scene ParentScene { get; private set; }

        [DataMember(Name = nameof(Components))]
        private readonly ObservableCollection<Component> _components = new ObservableCollection<Component>();

        public ReadOnlyObservableCollection<Component> Components { get; private set; }

        public ICommand RenameCommand { set; private get; }
        public ICommand IsEnabledCommand { get; private set; }


        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            if(_components != null)
            {
                Components = new ReadOnlyObservableCollection<Component>(_components);
                OnPropertyChanged(nameof(Components));
            }

            RenameCommand = new RelayCommand<string>(x =>
            {
                string oldName = _name;
                Name = x;

                Project.UndoRedo.Add(new UndoRedoAction(nameof(Name), this, oldName, x, $"Reame entity '{oldName}' to '{x}'"));

            }, x => x != _name);

            IsEnabledCommand = new RelayCommand<bool>(x =>
            {
                bool oldValue = _isEnabled;
                IsEnabled = x;

                Project.UndoRedo.Add(new UndoRedoAction(nameof(IsEnabled), this, oldValue, x, x ? $"Enable {Name}" : $"Disable {Name}"));

            });
        }

        public GameEntity(Scene scene)
        {
            Debug.Assert(scene != null);
            ParentScene = scene;
            _components.Add(new Transform(this));
            OnDeserialized(new StreamingContext());
        }
    }
}