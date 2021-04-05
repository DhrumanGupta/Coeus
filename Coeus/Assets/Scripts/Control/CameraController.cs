using System.Linq;
using UnityEngine;

namespace Game.Control
{
    public class CameraController : MonoBehaviour
    {
        [Header("Customization")]
        [SerializeField] private Vector3 _offset = Vector3.zero;
        [SerializeField] private float _smooth = .4f;

        private Vector3 _velocity = Vector3.zero;
        private Transform[] _players;

        [SerializeField] private float _minZoom = 10f;
        [SerializeField] private float _maxZoom = 40f;
        [SerializeField] private float _zoomLimiter = 10f;
        
        private Camera _camera = null;

        private void Start()
        {
            _camera = GetComponent<Camera>();
            _players = PlayerController.Controllers.Select(x => x.Transform).ToArray();
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
            Vector3 centerPoint = _players.Length == 1 ? _players[0].transform.position : bounds.center;
            
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
