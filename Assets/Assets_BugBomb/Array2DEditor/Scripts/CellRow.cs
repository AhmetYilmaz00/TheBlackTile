using Array2DEditor.Configurations;
using UnityEngine;

namespace Array2DEditor
{
    [System.Serializable]
    public class CellRow<T>
    {
        [SerializeField]
        private T[] row = new T[Array2DConfigurations.DefaultGridSize];

        public T this[int i]
        {
            get => row[i];
            set => row[i] = value;
        }
    }
}
