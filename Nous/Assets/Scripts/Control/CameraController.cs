using UnityEngine;

namespace Game.Control
{
    public class CameraController : MonoBehaviour
    {
        [Header("Customization")]
        [SerializeField] private Vector3 _offset = Vector3.zero;
        [SerializeField] private float _smooth = .4f;

        private Vector3 _velocity = Vector3.zero;

        [SerializeField] private float _minZoom = 10f;
        [SerializeField] private float _maxZoom = 40f;
        [SerializeField] private float _zoomLimiter = 10f;

        [Header("Data")]
        [SerializeField] private Transform _playersContainer = null;

        private Transform[] _players;

        private Camera _camera = null;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            
            _players = new Transform[_playersContainer.childCount];
            for (int i = 0; i < _playersContainer.childCount; i++)
            {
                _players[i] = _playersContainer.GetChild(i);
            }
        }

        void Update()
        {
            var bounds = GetBounds();

            Move(bounds);
            Zoom(bounds);
        }

        private void Zoom(Bounds bounds)
        {
            float greatestDistance = bounds.size.x;

            float newZoom = Mathf.Lerp(_minZoom, _maxZoom, greatestDistance / _zoomLimiter);
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, newZoom, Time.deltaTime * 1.5f);
        }

        private void Move(Bounds bounds)
        {
            Vector3 centerPoint = Vector3.zero;
            
            if (_players.Length == 1) 
                centerPoint = _players[0].transform.position;
            else
                centerPoint = bounds.center;
            
            Vector3 newPos = centerPoint - _offset;
            transform.position = Vector3.SmoothDamp(transform.position, newPos, ref _velocity, _smooth);
        }
        
        private Bounds GetBounds()
        {
            var bounds = new Bounds();
            foreach (var target in _players)
            {
                bounds.Encapsulate(target.transform.position);
            }

            return bounds;
        }
    }
}
