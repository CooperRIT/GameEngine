using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PrimalEditor.Components
{
    [DataContract]
    public class Transform : Component
    {
        private Vector3D _position;

        [DataMember]
        public Vector3D Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        private Vector3D _rotation;

        [DataMember]
        public Vector3D Rotation
        {
            get { return _rotation; }

            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    OnPropertyChanged(nameof(Rotation));
                }
            }
        }

        private Vector3D _scale;

        [DataMember]
        public Vector3D Scale
        {
            get { return _scale; }

            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    OnPropertyChanged(nameof(Scale));
                }
            }
        }

        public Transform(GameEntity user) : base(user)
        {
            //18.10
        }
    }
}
