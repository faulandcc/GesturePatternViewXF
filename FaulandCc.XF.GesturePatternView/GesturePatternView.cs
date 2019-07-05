using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Plugin.Vibrate;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using ContentView = Xamarin.Forms.ContentView;

namespace FaulandCc.XF.GesturePatternView
{
    public delegate void GesturePatternCompletedEventHandler(object sender, GesturePatternCompletedEventArgs e);
    public delegate void TouchPointTouchedEventHandler(object sender, TouchPointTouchedEventArgs e);

    /// <summary>
    /// The gesture pattern view. 
    /// </summary>
    public class GesturePatternView : ContentView
    {
        private readonly List<GestureTouchPoint> _touchPoints = new List<GestureTouchPoint>();
        private readonly StringBuilder _gestureValueBuilder = new StringBuilder();
        private string _lastTouchPointValue;
        private SKCanvasView _canvas;
        private List<Point> _fixedPoints = new List<Point>();
        private Point _pendingPoint = Point.Zero;
        private float _heigth;
        private float _width;
        private double _tpRadius;


        #region bindable properties

        /// <summary>
        /// The number of horizontal touch points.
        /// </summary>
        public static readonly BindableProperty HorizontalTouchPointsProperty = BindableProperty.Create<GesturePatternView, int>(x => x.HorizontalTouchPoints, 3, BindingMode.OneWay);

        /// <summary>
        /// The number of vertical touch points.
        /// </summary>
        public static readonly BindableProperty VerticalTouchPointsProperty = BindableProperty.Create<GesturePatternView, int>(x => x.VerticalTouchPoints, 3, BindingMode.OneWay);

        /// <summary>
        /// The gesture pattern value.
        /// </summary>
        public static readonly BindableProperty GesturePatternValueProperty = BindableProperty.Create<GesturePatternView, string>(x => x.GesturePatternValue, null, BindingMode.OneWay);

        /// <summary>
        /// The color to use for untouched touch points.
        /// </summary>
        public static readonly BindableProperty TouchPointColorProperty = BindableProperty.Create<GesturePatternView, Color>(x => x.TouchPointColor, Color.Black, BindingMode.OneWay);

        /// <summary>
        /// The color to use for touched touch points.
        /// </summary>
        public static readonly BindableProperty TouchPointHighlightColorProperty = BindableProperty.Create<GesturePatternView, Color>(x => x.TouchPointHighlightColor, Color.Yellow, BindingMode.OneWay);

        /// <summary>
        /// The touch point stroke width.
        /// </summary>
        public static readonly BindableProperty TouchPointStrokeWidthProperty = BindableProperty.Create<GesturePatternView, float>(x => x.TouchPointStrokeWidth, 3, BindingMode.OneWay);

        /// <summary>
        /// The touched touch point stroke width.
        /// </summary>
        public static readonly BindableProperty TouchPointTouchedStrokeWidthProperty = BindableProperty.Create<GesturePatternView, float>(x => x.TouchPointTouchedStrokeWidth, 3, BindingMode.OneWay);

        /// <summary>
        /// The touch point inner circle stroke width.
        /// </summary>
        public static readonly BindableProperty TouchPointInnerCircleStrokeWidthProperty = BindableProperty.Create<GesturePatternView, float>(x => x.TouchPointInnerCircleStrokeWidth, 3, BindingMode.OneWay);

        /// <summary>
        /// The color to use for a drawing line.
        /// </summary>
        public static readonly BindableProperty LineDrawingColorProperty = BindableProperty.Create<GesturePatternView, Color>(x => x.LineDrawingColor, Color.Red, BindingMode.OneWay);

        /// <summary>
        /// The stroke width to use for a drawing line.
        /// </summary>
        public static readonly BindableProperty LineDrawingStrokeWidthProperty = BindableProperty.Create<GesturePatternView, float>(x => x.LineDrawingStrokeWidth, 5, BindingMode.OneWay);

        /// <summary>
        /// The color to use for a drawn line.
        /// </summary>
        public static readonly BindableProperty LineDrawnColorProperty = BindableProperty.Create<GesturePatternView, Color>(x => x.LineDrawnColor, Color.Green, BindingMode.OneWay);

        /// <summary>
        /// The stroke width to use for a drawn line.
        /// </summary>
        public static readonly BindableProperty LineDrawnStrokeWidthProperty = BindableProperty.Create<GesturePatternView, float>(x => x.LineDrawnStrokeWidth, 5, BindingMode.OneWay);

        /// <summary>
        /// Vibrate when touching a touch point.
        /// </summary>
        public static readonly BindableProperty VibrateProperty = BindableProperty.Create<GesturePatternView, bool>(x => x.Vibrate, true, BindingMode.OneWay);


        #endregion


        #region properties

        /// <summary>
        /// The number of horizontal touch points.
        /// </summary>
        public int HorizontalTouchPoints
        {
            get { return (int)GetValue(VerticalTouchPointsProperty); }
            set { SetValue(VerticalTouchPointsProperty, value); }
        }

        /// <summary>
        /// The number of vertical touch points.
        /// </summary>
        public int VerticalTouchPoints
        {
            get { return (int)GetValue(HorizontalTouchPointsProperty); }
            set { SetValue(HorizontalTouchPointsProperty, value); }
        }

        /// <summary>
        /// The gesture pattern value.
        /// </summary>
        public string GesturePatternValue
        {
            get { return (string)GetValue(GesturePatternValueProperty); }
            set { SetValue(GesturePatternValueProperty, value); }
        }

        /// <summary>
        /// The color of an untouched touch point.
        /// </summary>
        public Color TouchPointColor
        {
            get { return (Color)GetValue(TouchPointColorProperty); }
            set { SetValue(TouchPointColorProperty, value); }
        }

        /// <summary>
        /// The text color for a touched touch points.
        /// </summary>
        public Color TouchPointHighlightColor
        {
            get { return (Color)GetValue(TouchPointHighlightColorProperty); }
            set { SetValue(TouchPointHighlightColorProperty, value); }
        }

        /// <summary>
        /// The touch point stroke width.
        /// </summary>
        public float TouchPointStrokeWidth
        {
            get { return (float)GetValue(TouchPointStrokeWidthProperty); }
            set { SetValue(TouchPointStrokeWidthProperty, value); }
        }

        /// <summary>
        /// The touched touch point stroke width.
        /// </summary>
        public float TouchPointTouchedStrokeWidth
        {
            get { return (float)GetValue(TouchPointTouchedStrokeWidthProperty); }
            set { SetValue(TouchPointTouchedStrokeWidthProperty, value); }
        }

        /// <summary>
        /// The touch point inner circle stroke width.
        /// </summary>
        public float TouchPointInnerCircleStrokeWidth
        {
            get { return (float)GetValue(TouchPointInnerCircleStrokeWidthProperty); }
            set { SetValue(TouchPointInnerCircleStrokeWidthProperty, value); }
        }

        /// <summary>
        /// The color to use for a drawing line.
        /// </summary>
        public Color LineDrawingColor
        {
            get { return (Color)GetValue(LineDrawingColorProperty); }
            set { SetValue(LineDrawingColorProperty, value); }
        }

        /// <summary>
        /// The stroke width to use for a drawing line.
        /// </summary>
        public float LineDrawingStrokeWidth
        {
            get { return (float)GetValue(LineDrawingStrokeWidthProperty); }
            set { SetValue(LineDrawingStrokeWidthProperty, value); }
        }

        /// <summary>
        /// The color to use for a drawn line.
        /// </summary>
        public Color LineDrawnColor
        {
            get { return (Color)GetValue(LineDrawnColorProperty); }
            set { SetValue(LineDrawnColorProperty, value); }
        }

        /// <summary>
        /// The stroke width to use for a drawn line.
        /// </summary>
        public float LineDrawnStrokeWidth
        {
            get { return (float)GetValue(LineDrawnStrokeWidthProperty); }
            set { SetValue(LineDrawnStrokeWidthProperty, value); }
        }

        /// <summary>
        /// Vibrate when touching a touch point.
        /// </summary>
        public bool Vibrate
        {
            get { return (bool)GetValue(VibrateProperty); }
            set { SetValue(VibrateProperty, value); }
        }

        #endregion


        #region events

        /// <summary>
        /// Raised as soon as the finger was released.
        /// </summary>
        public event GesturePatternCompletedEventHandler GesturePatternCompleted;
        protected virtual void OnGesturePatternCompleted(string gesturePatternValue)
        {
            this.GesturePatternValue = gesturePatternValue;
            this.GesturePatternCompleted?.Invoke(this, new GesturePatternCompletedEventArgs()
            {
                GesturePatternValue = this.GesturePatternValue
            });
        }

        /// <summary>
        /// Raised with every touched touch point.
        /// </summary>
	    public event TouchPointTouchedEventHandler TouchPointTouched;
        protected virtual void OnTouchPointTouched(GestureTouchPoint gtp)
        {
            if (this.Vibrate && CrossVibrate.Current.CanVibrate)
            {
                CrossVibrate.Current.Vibration(TimeSpan.FromMilliseconds(100));
            }
            this.TouchPointTouched?.Invoke(this, new TouchPointTouchedEventArgs(gtp));
        }

        #endregion


        #region ctor

        /// <summary>
        /// Create gesture pattern view instance.
        /// </summary>
        public GesturePatternView()
        {
            this.CreateTouchInterface();

            // Clear the pattern if the device is turned during a gesture.
            this.LayoutChanged += (sender, args) =>
            {
                this.Clear();
            };
        }

        #endregion


        #region public methods

        /// <summary>
        /// Clear the pattern view.
        /// </summary>
        public void Clear()
        {
            this.GesturePatternValue = null;
            _pendingPoint = Point.Zero;
            _lastTouchPointValue = null;
            _gestureValueBuilder.Clear();
            _fixedPoints.Clear();
            foreach (var gtp in _touchPoints)
            {
                gtp.Reset();
            }
            _canvas.InvalidateSurface();
        }

        #endregion


        #region private methods

        private void CreateTouchInterface()
        {
            this.Content = null;
            _touchPoints.Clear();

            if (this.VerticalTouchPoints <= 0 || this.HorizontalTouchPoints <= 0)
            {
                return;
            }

            _canvas = new SKCanvasView();
            _canvas.IgnorePixelScaling = false;
            _canvas.EnableTouchEvents = true;
            _canvas.PaintSurface += CanvasOnPaintSurface;
            _canvas.Touch += CanvasOnTouch;

            this.Content = _canvas;
        }

        private void CanvasOnTouch(object o, SKTouchEventArgs e)
        {
            e.Handled = true;
            if (e.InContact && e.ActionType == SKTouchAction.Moved)
            {
                Debug.WriteLine($"TOUCH {e.ActionType} {e.Location}");
                var location = e.Location.ToFormsPoint();

                bool hasPendingPoint = true;
                foreach (var gestureTouchPoint in _touchPoints)
                {
                    if (TouchedTouchPoint(location, gestureTouchPoint) && gestureTouchPoint.Value != _lastTouchPointValue)
                    {
                        gestureTouchPoint.Touch();
                        _gestureValueBuilder.Append(gestureTouchPoint.Value);
                        _lastTouchPointValue = gestureTouchPoint.Value;
                        _fixedPoints.Add(gestureTouchPoint.Center);
                        hasPendingPoint = false;
                        this.OnTouchPointTouched(gestureTouchPoint);
                        break;
                    }
                }
                if (hasPendingPoint)
                {
                    _pendingPoint.X = location.X;
                    _pendingPoint.Y = location.Y;
                }
                else
                {
                    _pendingPoint = Point.Zero;
                }
                _canvas?.InvalidateSurface();
            }
            else if (!e.InContact && e.ActionType == SKTouchAction.Released && _touchPoints.Any(p => p.IsTouched))
            {
                Debug.WriteLine($"RELEASED {e.ActionType} {e.Location}");

                // Gesture pattern completed.
                this.OnGesturePatternCompleted(_gestureValueBuilder.ToString());

                // Reset the touchpoints.
                foreach (var gestureTouchPoint in _touchPoints)
                {
                    gestureTouchPoint.Reset();
                }
                // Clear the recognized gesture values.
                _gestureValueBuilder.Clear();
                _lastTouchPointValue = null;

                _pendingPoint = Point.Zero;
                _canvas?.InvalidateSurface();
            }
        }

        private void CanvasOnPaintSurface(object o, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();

            bool recreateTouchPoints = false;
            if (_width != canvas.LocalClipBounds.Width || _heigth != canvas.LocalClipBounds.Height)
            {
                _width = canvas.LocalClipBounds.Width;
                _heigth = canvas.LocalClipBounds.Height;
                recreateTouchPoints = true;
            }

            var cellWidth = _width / this.HorizontalTouchPoints;
            var cellHeight = _heigth / this.VerticalTouchPoints;
            if (recreateTouchPoints)
            {
                _touchPoints.Clear();

                for (int yAxisIndex = 0; yAxisIndex < this.VerticalTouchPoints; yAxisIndex++)
                {
                    for (int xAxisIndex = 0; xAxisIndex < this.HorizontalTouchPoints; xAxisIndex++)
                    {
                        double x = (cellWidth / 8) + (cellWidth * xAxisIndex);
                        double y = (cellWidth / 8) + (cellHeight * yAxisIndex);

                        double centerX = x + (cellWidth / 2);
                        double centerY = y + (cellHeight / 2);

                        var gtp = new GestureTouchPoint()
                        {
                            Value = (_touchPoints.Count + 1).ToString(),
                            Width = cellWidth/4,
                            Height = cellHeight/4,
                            IsTouched = false,
                            Location = new Point(centerX - (cellWidth/4), centerY - (cellHeight/4))
                        };
                        _touchPoints.Add(gtp);
                    }
                }
            }

            SKPaint skPaintTouchPoint = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                Color = this.TouchPointColor.ToSKColor(),
                StrokeWidth = this.TouchPointStrokeWidth
            };
            SKPaint skPaintTouchPointTouched = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = this.TouchPointHighlightColor.ToSKColor(),
                StrokeWidth = this.TouchPointTouchedStrokeWidth
            };
            SKPaint skPaintTouchPointTouchedInnerCircle = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                Color = this.LineDrawnColor.ToSKColor(),
                StrokeWidth = this.TouchPointInnerCircleStrokeWidth
            };
            SKPaint skPaintLineDrawing = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = this.LineDrawingColor.ToSKColor(),
                StrokeWidth = this.LineDrawingStrokeWidth
            };
            SKPaint skPaintLineDrawn = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = this.LineDrawnColor.ToSKColor(),
                StrokeWidth = this.LineDrawnStrokeWidth
            };

            // Paint the touchpoints.
            _tpRadius = ((cellHeight + cellWidth) / 2) * 0.05;
            foreach (var touchPoint in _touchPoints)
            {
                // DEBUG: draw the touchpoint location
                //canvas.DrawLine((float)touchPoint.Location.X, (float)touchPoint.Location.Y, (float)(touchPoint.Location.X + touchPoint.Width), (float)touchPoint.Location.Y, skPaintLineDrawing);
                //canvas.DrawLine((float)touchPoint.Location.X, (float)touchPoint.Location.Y, (float)touchPoint.Location.X, (float)(touchPoint.Location.Y + touchPoint.Height), skPaintLineDrawing);

                canvas.DrawCircle((float)touchPoint.Center.X, (float)touchPoint.Center.Y, (float)_tpRadius, skPaintTouchPoint);
            }

            // Any touched points?
            if (_fixedPoints.Count == 0)
            {
                return;
            }

            // Paint the fixed points.
            double tpInnerRadius = _tpRadius / 3;
            double tpOuterRadius = _tpRadius * 3;

            canvas.DrawCircle((float)_fixedPoints.First().X, (float)_fixedPoints.First().Y, (float)tpOuterRadius, skPaintTouchPointTouched);
            canvas.DrawCircle((float)_fixedPoints.First().X, (float)_fixedPoints.First().Y, (float)tpInnerRadius, skPaintTouchPointTouchedInnerCircle);
            for (int i = 1; i < _fixedPoints.Count; i++)
            {
                float xStart = (float)_fixedPoints[i - 1].X;
                float yStart = (float)_fixedPoints[i - 1].Y;
                float xEnd = (float)_fixedPoints[i].X;
                float yEnd = (float)_fixedPoints[i].Y;
                canvas.DrawLine(xStart, yStart, xEnd, yEnd, skPaintLineDrawn);
                canvas.DrawCircle((float)_fixedPoints[i].X, (float)_fixedPoints[i].Y, (float)tpOuterRadius, skPaintTouchPointTouched);
                canvas.DrawCircle((float)_fixedPoints[i].X, (float)_fixedPoints[i].Y, (float)tpInnerRadius, skPaintTouchPointTouchedInnerCircle);
            }
            // Paint the pending point.
            if (_pendingPoint != Point.Zero)
            {
                float xStart = (float)_fixedPoints.Last().X;
                float yStart = (float)_fixedPoints.Last().Y;
                float xEnd = (float)_pendingPoint.X;
                float yEnd = (float)_pendingPoint.Y;
                canvas.DrawLine(xStart, yStart, xEnd, yEnd, skPaintLineDrawing);
            }
        }

        private bool TouchedTouchPoint(Point location, GestureTouchPoint touchPoint)
        {
            // Touch point touched?
            if (location.X >= touchPoint.Location.X &&
                location.X <= (touchPoint.Location.X + touchPoint.Width) &&
                location.Y >= touchPoint.Location.Y &&
                location.Y <= (touchPoint.Location.Y + touchPoint.Height))
            {
                return true;
            }

            // Touch point touched by drawing line?
            if (_fixedPoints.Any())
            {
                Point lineStartPoint = _fixedPoints.Last();
                Point lineEndPoint = location;

                return this.PointOnLineSegment(lineStartPoint, lineEndPoint, touchPoint.Center, _tpRadius * 1.6);
            }
            return false;
        }

        public bool PointOnLineSegment(Point pt1, Point pt2, Point pt, double epsilon)
        {
            if (pt.X - Math.Max(pt1.X, pt2.X) > epsilon ||
                Math.Min(pt1.X, pt2.X) - pt.X > epsilon ||
                pt.Y - Math.Max(pt1.Y, pt2.Y) > epsilon ||
                Math.Min(pt1.Y, pt2.Y) - pt.Y > epsilon)
                return false;

            if (Math.Abs(pt2.X - pt1.X) < epsilon)
                return Math.Abs(pt1.X - pt.X) < epsilon || Math.Abs(pt2.X - pt.X) < epsilon;
            if (Math.Abs(pt2.Y - pt1.Y) < epsilon)
                return Math.Abs(pt1.Y - pt.Y) < epsilon || Math.Abs(pt2.Y - pt.Y) < epsilon;

            double x = pt1.X + (pt.Y - pt1.Y) * (pt2.X - pt1.X) / (pt2.Y - pt1.Y);
            double y = pt1.Y + (pt.X - pt1.X) * (pt2.Y - pt1.Y) / (pt2.X - pt1.X);

            return Math.Abs(pt.X - x) < epsilon || Math.Abs(pt.Y - y) < epsilon;
        }

        #endregion
    }


    public class GesturePatternCompletedEventArgs : EventArgs
    {
        public string GesturePatternValue { get; set; }
    }

    public class TouchPointTouchedEventArgs : EventArgs
    {
        public GestureTouchPoint TouchPoint { get; }


        public TouchPointTouchedEventArgs(GestureTouchPoint gtp)
        {
            this.TouchPoint = gtp;
        }
    }
}
