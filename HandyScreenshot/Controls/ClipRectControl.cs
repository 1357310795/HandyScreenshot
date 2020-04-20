﻿using System;
using System.Windows;
using System.Windows.Media;

namespace HandyScreenshot.Controls
{
    public class ClipRectControl : FrameworkElement
    {
        private const double MinDisplayPointLimit = 80;
        private static readonly Brush MaskBrush = new SolidColorBrush(Color.FromArgb(0xA0, 0, 0, 0));
        private static readonly Brush PrimaryBrush = new SolidColorBrush(Color.FromRgb(0x20, 0x80, 0xf0));
        private static readonly Pen PrimaryPen = new Pen(PrimaryBrush, 2.5);
        private static readonly Pen WhitePen = new Pen(Brushes.White, 1.5);
        private static readonly Rect RectZero = new Rect(0, 0, 0, 0);
        private static readonly Point PointZero = new Point(0, 0);

        public static readonly DependencyProperty RectOperationProperty = DependencyProperty.Register(
            "RectOperation", typeof(RectOperation), typeof(ClipRectControl), new PropertyMetadata(null, (o, args) =>
            {
                if (o is ClipRectControl rectMaskControl)
                {
                    rectMaskControl.Attach();
                }
            }));
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(ClipRectControl), new PropertyMetadata(MaskBrush));

        public RectOperation RectOperation
        {
            get => (RectOperation)GetValue(RectOperationProperty);
            set => SetValue(RectOperationProperty, value);
        }

        public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        private readonly DrawingVisual _drawingVisual;

        private Rect _topRect = RectZero;
        private Rect _rightRect = RectZero;
        private Rect _bottomRect = RectZero;
        private Rect _leftRect = RectZero;
        private Rect _centralRect = RectZero;

        private Point _leftTopPoint = PointZero;
        private Point _topPoint = PointZero;
        private Point _rightTopPoint = PointZero;
        private Point _rightPoint = PointZero;
        private Point _rightBottomPoint = PointZero;
        private Point _bottomPoint = PointZero;
        private Point _leftBottomPoint = PointZero;
        private Point _leftPoint = PointZero;

        public ClipRectControl()
        {
            var children = new VisualCollection(this) { new DrawingVisual() };
            _drawingVisual = (DrawingVisual)children[0];
            SizeChanged += OnSizeChanged;
        }

        private void Attach()
        {
            RectOperation?.Attach((x, y, w, h) => Dispatcher.Invoke(() =>
            {
                var r = x + w;
                var b = y + h;

                _leftRect.Y = Math.Max(y, 0);
                _leftRect.Width = Math.Max(x, 0);
                _leftRect.Height = Math.Max(h, 0);

                _topRect.Height = Math.Max(y, 0);

                _rightRect.X = Math.Max(r, 0);
                _rightRect.Y = Math.Max(y, 0);
                _rightRect.Width = Math.Max(ActualWidth - r, 0);
                _rightRect.Height = Math.Max(h, 0);

                _bottomRect.Y = Math.Max(b, 0);
                _bottomRect.Height = Math.Max(ActualHeight - b, 0);

                _centralRect.X = x;
                _centralRect.Y = y;
                _centralRect.Width = w;
                _centralRect.Height = h;

                if (_centralRect.Width > MinDisplayPointLimit && _centralRect.Height > MinDisplayPointLimit)
                {
                    var halfR = x + w / 2D;
                    var halfB = y + h / 2D;

                    _leftTopPoint.X = x;
                    _leftTopPoint.Y = y;

                    _topPoint.X = halfR;
                    _topPoint.Y = y;

                    _rightTopPoint.X = r;
                    _rightTopPoint.Y = y;

                    _rightPoint.X = r;
                    _rightPoint.Y = halfB;

                    _rightBottomPoint.X = r;
                    _rightBottomPoint.Y = b;

                    _bottomPoint.X = halfR;
                    _bottomPoint.Y = b;

                    _leftBottomPoint.X = x;
                    _leftBottomPoint.Y = b;

                    _leftPoint.X = x;
                    _leftPoint.Y = halfB;
                }

                RefreshDrawingVisual();
            }));
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _topRect.Width = ActualWidth;
            _bottomRect.Width = ActualWidth;
        }

        private void RefreshDrawingVisual()
        {
            var dc = _drawingVisual.RenderOpen();

            DrawRectangle(dc, _leftRect, Background);
            DrawRectangle(dc, _topRect, Background);
            DrawRectangle(dc, _rightRect, Background);
            DrawRectangle(dc, _bottomRect, Background);
            DrawRectangle(dc, _centralRect, Brushes.Transparent, PrimaryPen);

            if (_centralRect.Width > MinDisplayPointLimit && _centralRect.Height > MinDisplayPointLimit)
            {
                DrawPoint(dc, _leftTopPoint);
                DrawPoint(dc, _topPoint);
                DrawPoint(dc, _rightTopPoint);
                DrawPoint(dc, _rightPoint);
                DrawPoint(dc, _rightBottomPoint);
                DrawPoint(dc, _bottomPoint);
                DrawPoint(dc, _leftBottomPoint);
                DrawPoint(dc, _leftPoint);
            }

            dc.Close();
        }

        private static void DrawRectangle(DrawingContext dc, Rect rect, Brush background, Pen pen = null)
        {
            if (rect.Width * rect.Height > 0)
            {
                dc.DrawRectangle(background, pen, rect);
            }
        }

        private static void DrawPoint(DrawingContext dc, Point point)
        {
            dc.DrawEllipse(PrimaryBrush, WhitePen, point, 5, 5);
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException();

            return _drawingVisual;
        }
    }
}
