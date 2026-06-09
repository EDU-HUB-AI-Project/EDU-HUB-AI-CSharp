using EDU_HUB_AI.Config.Theme;

namespace EDU_HUB_AI.Config.Component.Domain
{
    // OUTER 지도 클릭 → mapX/mapY (픽셀 좌표)
    public class FacilityMapPicker : Panel
    {
        public const int MarkerSize = 20;
        public const int HitSize = 20;

        private static readonly string MapAssetPath =
            Path.Combine(AppContext.BaseDirectory, "Assets", "facility-location-map.png");

        private Image? _mapImage;
        private PointF? _markerImagePoint;
        private readonly Label _lblCoords;

        public decimal? MapX => _markerImagePoint.HasValue ? (decimal)_markerImagePoint.Value.X : null;
        public decimal? MapY => _markerImagePoint.HasValue ? (decimal)_markerImagePoint.Value.Y : null;

        public FacilityMapPicker()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            BackColor = ThemeColors.Surface;
            Height = 280;
            Dock = DockStyle.Top;
            Margin = new Padding(0, 0, 0, 8);
            Cursor = Cursors.Hand;

            _lblCoords = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 22,
                Font = ThemeFonts.BodySm,
                ForeColor = ThemeColors.TextMuted,
                Text = "지도를 클릭해 위치를 선택하세요",
                TextAlign = ContentAlignment.MiddleLeft
            };

            Controls.Add(_lblCoords);
            LoadMapImage();
            MouseClick += OnMapClick;
            Resize += (_, _) => Invalidate();
        }

        public void SetCoordinates(decimal? x, decimal? y)
        {
            if (x.HasValue && y.HasValue && _mapImage != null)
            {
                _markerImagePoint = new PointF((float)x.Value, (float)y.Value);
                UpdateCoordLabel();
                Invalidate();
                return;
            }

            Clear();
        }

        public void Clear()
        {
            _markerImagePoint = null;
            _lblCoords.Text = "지도를 클릭해 위치를 선택하세요";
            Invalidate();
        }

        private void LoadMapImage()
        {
            try
            {
                if (File.Exists(MapAssetPath))
                    _mapImage = Image.FromFile(MapAssetPath);
            }
            catch
            {
                _mapImage = null;
            }
        }

        private void OnMapClick(object? sender, MouseEventArgs e)
        {
            if (_mapImage == null) return;

            var display = GetDisplayRectangle();
            if (!display.Contains(e.Location)) return;

            var imagePoint = ClientToImage(e.Location, display);
            if (!imagePoint.HasValue) return;

            _markerImagePoint = imagePoint;
            UpdateCoordLabel();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.Clear(BackColor);

            var mapArea = new Rectangle(0, 0, Width, Height - _lblCoords.Height);
            if (_mapImage == null)
            {
                using var pen = new Pen(ThemeColors.Border);
                g.DrawRectangle(pen, mapArea.X, mapArea.Y, mapArea.Width - 1, mapArea.Height - 1);
                TextRenderer.DrawText(g, "지도 이미지를 불러올 수 없습니다.", Font, mapArea, ThemeColors.TextMuted,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                return;
            }

            var display = GetDisplayRectangle();
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(_mapImage, display);

            using (var border = new Pen(ThemeColors.Border))
                g.DrawRectangle(border, display.X, display.Y, display.Width - 1, display.Height - 1);

            if (_markerImagePoint.HasValue)
                DrawMarker(g, ImageToClient(_markerImagePoint.Value, display));
        }

        private void DrawMarker(Graphics g, Point center)
        {
            var half = MarkerSize / 2;
            var rect = new Rectangle(center.X - half, center.Y - half, MarkerSize, MarkerSize);

            using var fill = new SolidBrush(Color.FromArgb(90, 37, 99, 235));
            using var ring = new Pen(Color.FromArgb(220, 37, 99, 235), 2f);
            using var dot = new SolidBrush(Color.White);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillEllipse(fill, rect);
            g.DrawEllipse(ring, rect);
            g.FillEllipse(dot, center.X - 3, center.Y - 3, 6, 6);
        }

        private Rectangle GetDisplayRectangle()
        {
            var area = new Rectangle(0, 0, Width, Height - _lblCoords.Height);
            if (_mapImage == null || area.Width <= 0 || area.Height <= 0)
                return Rectangle.Empty;

            var ratio = Math.Min(area.Width / (float)_mapImage.Width, area.Height / (float)_mapImage.Height);
            var w = Math.Max(1, (int)(_mapImage.Width * ratio));
            var h = Math.Max(1, (int)(_mapImage.Height * ratio));
            return new Rectangle(area.X + (area.Width - w) / 2, area.Y + (area.Height - h) / 2, w, h);
        }

        private PointF? ClientToImage(Point client, Rectangle display)
        {
            if (_mapImage == null || !display.Contains(client))
                return null;

            var relX = (client.X - display.X) / (float)display.Width;
            var relY = (client.Y - display.Y) / (float)display.Height;
            return new PointF(relX * _mapImage.Width, relY * _mapImage.Height);
        }

        private Point ImageToClient(PointF imagePoint, Rectangle display)
        {
            if (_mapImage == null)
                return Point.Empty;

            var relX = imagePoint.X / _mapImage.Width;
            var relY = imagePoint.Y / _mapImage.Height;
            return new Point(
                display.X + (int)Math.Round(relX * display.Width),
                display.Y + (int)Math.Round(relY * display.Height));
        }

        private void UpdateCoordLabel()
        {
            if (!_markerImagePoint.HasValue)
            {
                _lblCoords.Text = "지도를 클릭해 위치를 선택하세요";
                return;
            }

            _lblCoords.Text = $"X: {Math.Round(_markerImagePoint.Value.X, 6)}   Y: {Math.Round(_markerImagePoint.Value.Y, 6)}";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _mapImage?.Dispose();
            base.Dispose(disposing);
        }
    }
}
