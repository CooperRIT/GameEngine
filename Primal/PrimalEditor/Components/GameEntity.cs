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
    class GameEntity : ViewModelBase
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



        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            if(_components != null)
            {
                Components = new ReadOnlyObservableCollection<Component>(_components);
                OnPropertyChanged(nameof(Components));
            }

            
        }

        public GameEntity(Scene scene)
        {
            Debug.Assert(scene != null);
            ParentScene = scene;
            _components.Add(new Transform(this));
            OnDeserialized(new StreamingContext());
        }
    }

    abstract class MSEntity : ViewModelBase
    {

        //Enables updates to selcted game objects
        private bool _enableUpdates = true;

        private bool? _isEnabled;

        public bool? IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if(value != _isEnabled)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private readonly ObservableCollection<IMSComponent> _components = new ObservableCollection<IMSComponent>();

        public ReadOnlyObservableCollection<IMSComponent> Components { get;  }

        public List<GameEntity> SelectedEntities { get; }

        

        public static float? GetMixedValue(List<GameEntity> entities, Func<GameEntity, float> getProperty)
        {
            float? value = getProperty(entities.First());

            foreach(var entity in entities.Skip(1))
            {
                if(!value.IsTheSameAs(getProperty(entity)))
                {
                    return null;
                }
            }

            return value;
        }

        public static bool? GetMixedValue(List<GameEntity> entities, Func<GameEntity, bool> getProperty)
        {
            bool? value = getProperty(entities.First());

            foreach (var entity in entities.Skip(1))
            {
                if (value != getProperty(entity))
                {
                    return null;
                }
            }

            return value;
        }

        public static string GetMixedValue(List<GameEntity> entities, Func<GameEntity, string> getProperty)
        {
            string value = getProperty(entities.First());

            foreach (var entity in entities.Skip(1))
            {
                if (value != getProperty(entity))
                {
                    return null;
                }
            }

            return value;
        }

        protected virtual bool UpdateMSCGameEnitity()
        {
            IsEnabled = GetMixedValue(SelectedEntities, new Func<GameEntity, bool>(x => x.IsEnabled));
            Name = GetMixedValue(SelectedEntities, new Func<GameEntity, string>(x => x.Name));

            return true;
        }

        public void Refresh()
        {
            _enableUpdates = false;
            UpdateMSCGameEnitity();
            _enableUpdates = true;
        }

        protected virtual bool UpdateGameEntities(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(IsEnabled):
                    SelectedEntities.ForEach(x => x.IsEnabled = IsEnabled.Value);
                    return true;

                case nameof(Name):
                    SelectedEntities.ForEach(x => x.Name = Name);
                    return true;

            }

            return false;
        }

        public MSEntity(List<GameEntity> entities)
        {
            Debug.Assert(entities?.Any() == true);

            Components = new ReadOnlyObservableCollection<IMSComponent>(_components);

            SelectedEntities = entities;

            PropertyChanged += (s, e) =>
            {
                if(!_enableUpdates)
                {
                    return;
                }
                UpdateGameEntities(e.PropertyName);
            };
        }
    }

    class MSGameEntity : MSEntity
    {
        public MSGameEntity(List<GameEntity> entities) : base(entities)
        {
            Refresh();
        }
    }
}