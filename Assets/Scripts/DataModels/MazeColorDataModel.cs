using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeColorDataModel : MonoBehaviour
{
    [SerializeField] private MazeColorTuple[] _colorTuples;
    [SerializeField] private Color _fuelColor;
    [SerializeField] private Color _doorColor;
    [SerializeField] private Color _goldColor;

    public Color GetColorForType(GroundType type)
    {
        if (_groundTypeToColor == null)
        {
            _groundTypeToColor = new Dictionary<GroundType, Color>
            {
                { GroundType.Empty, Color.clear },
                { GroundType.Door, _doorColor },
                { GroundType.Fuel, _fuelColor },
                { GroundType.Gold, _goldColor }
            };
            foreach (var tuple in _colorTuples)
            {
                _groundTypeToColor[tuple.groundType] = tuple.color;
            }
        }

        return _groundTypeToColor[type];
    }

    public GroundType[] groundTypes => _colorTuples.Select(tuple => tuple.groundType).ToArray();
    
    private Dictionary<GroundType, Color> _groundTypeToColor;

    [System.Serializable]
    public class MazeColorTuple
    {
        public GroundType groundType;
        public Color color;
    }
}
