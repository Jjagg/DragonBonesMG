using Microsoft.Xna.Framework;

namespace DragonBonesMG.Curves {
    public struct CubicBezier : ITweenCurve {

        private const float SampleStep = 0.05f;
        private readonly Vector2 _p1;
        private readonly Vector2 _p2;

        private readonly Curve _sampledCurve;

        public CubicBezier(Vector2 p1, Vector2 p2) {
            _p1 = p1;
            _p2 = p2;
            _sampledCurve = new Curve();
            ComputeCurve();
        }

        public CubicBezier(float x1, float y1, float x2, float y2) :
            this(new Vector2(x1, y1), new Vector2(x2, y2)) {
        }

        public float GetValue(float time) {
            return _sampledCurve.Evaluate(time);
        }

        private Vector2 Evaluate(float t) {
            var u = 1 - t;
            return 3 * t * u * u * _p1 + 3 * t * t * u * _p2 + t * t * t * Vector2.One;
        }

        private void ComputeCurve() {
            var keys = _sampledCurve.Keys;
            keys.Clear();
            keys.Add(new CurveKey(0, 0));
            for (var t = SampleStep; t < 1; t += SampleStep) {
                var p = Evaluate(t);
                keys.Add(new CurveKey(p.X, p.Y));
            }
            keys.Add(new CurveKey(1, 1));
            _sampledCurve.ComputeTangents(CurveTangent.Smooth);
        }
    }

}