using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;

namespace EDU_HUB_AI.Config.Component.Domain
{
    // 시설 등록/수정 — INNER(층·이미지) / OUTER(지도좌표)
    public class FacilityEditModal : AppModal
    {
        private readonly FacilityInfoDto? _source;
        private readonly ComboBox _cmbType;
        private readonly TextField _txtName;
        private readonly TextField _txtLocation;
        private readonly TextField _txtDescription;
        private readonly Panel _panelInner;
        private readonly TextField _txtFloor;
        private readonly Panel _panelImage;
        private TextBox _txtImagePath = null!;
        private PictureBox _picPreview = null!;
        private Button _btnBrowse = null!;
        private readonly Panel _panelOuter;
        private readonly FacilityMapPicker _mapPicker;

        private string? _pendingImageSourcePath;
        private string? _currentImagePath;
        private string? _committedType;

        public FacilityInfoDto? Result { get; private set; }

        public FacilityEditModal(FacilityInfoDto? source)
        {
            _source = source;
            var initialType = string.IsNullOrWhiteSpace(source?.facilityType) ? "INNER" : source.facilityType.ToUpperInvariant();
            _currentImagePath = string.Equals(initialType, "OUTER", StringComparison.OrdinalIgnoreCase)
                ? null
                : source?.imagePath;
            ModalTitle = source == null ? "시설 등록" : "시설 수정";
            ConfirmText = "저장";

            var stack = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                Padding = new Padding(0),
                BackColor = ThemeColors.Surface
            };
            stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var row = 0;
            _cmbType = AddComboField(stack, "구분", row++);
            _txtName = AddField(stack, "시설명", source?.name, "시설 이름", row++);
            _txtLocation = AddField(stack, "위치", source?.location, "건물·동·방향", row++);
            _txtDescription = AddField(stack, "안내", source?.description, "이용 안내", row++);

            _panelInner = AddSectionPanel(stack, row++);
            _panelImage = BuildImagePanel();
            _panelInner.Controls.Add(_panelImage);
            _txtFloor = AddField(_panelInner, "층수", source?.floor?.ToString(), "예) 2", 0);

            _panelOuter = AddSectionPanel(stack, row++);
            _mapPicker = new FacilityMapPicker();
            _panelOuter.Controls.Add(_mapPicker);
            if (string.Equals(initialType, "OUTER", StringComparison.OrdinalIgnoreCase))
                _mapPicker.SetCoordinates(source?.mapX, source?.mapY);
            else
                _mapPicker.Clear();

            _cmbType.Items.AddRange(["INNER", "OUTER"]);
            _cmbType.SelectedIndexChanged += (_, _) => OnFacilityTypeSwitched();
            _cmbType.SelectedItem = initialType is "OUTER" ? "OUTER" : "INNER";
            _btnBrowse.Click += OnBrowseImage;
            _picPreview.Click += OnBrowseImage;
            _picPreview.Cursor = Cursors.Hand;

            Body.Controls.Add(stack);
            UpdateImageUi();
            UpdateTypePanels();
        }

        public static FacilityInfoDto? Show(IWin32Window owner, FacilityInfoDto? source)
        {
            using var modal = new FacilityEditModal(source);
            return modal.ShowDialog(owner) == DialogResult.OK ? modal.Result : null;
        }

        protected override async void OnConfirm()
        {
            var type = _cmbType.SelectedItem?.ToString()?.Trim().ToUpperInvariant();
            var name = _txtName.Text.Trim();
            var location = _txtLocation.Text.Trim();
            var description = _txtDescription.Text.Trim();

            if (string.IsNullOrWhiteSpace(type) || (type != "INNER" && type != "OUTER"))
            {
                MessageBox.Show("구분을 선택해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("시설명을 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(location))
            {
                MessageBox.Show("위치를 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int? floor = null;
            decimal? mapX = null;
            decimal? mapY = null;
            string? imagePath = null;

            // INNER — 층수·이미지
            if (type == "INNER")
            {
                mapX = null;
                mapY = null;

                if (!int.TryParse(_txtFloor.Text.Trim(), out var f) || f < 0)
                {
                    MessageBox.Show("유효한 층수를 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                floor = f;

                // 이미지 업로드
                if (!string.IsNullOrWhiteSpace(_pendingImageSourcePath))
                {
                    try
                    {
                        imagePath = await FacilityImageStore.UploadSelectedFileAsync(_pendingImageSourcePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"이미지 업로드 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    imagePath = string.IsNullOrWhiteSpace(_currentImagePath) ? null : _currentImagePath;
                }
            }
            // OUTER — 지도 좌표
            else
            {
                floor = null;
                imagePath = null;

                if (!_mapPicker.MapX.HasValue || !_mapPicker.MapY.HasValue)
                {
                    MessageBox.Show("지도에서 위치를 선택해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                mapX = Math.Round(_mapPicker.MapX.Value, 1);
                mapY = Math.Round(_mapPicker.MapY.Value, 1);
            }

            Result = _source != null ? CopyOf(_source) : new FacilityInfoDto();
            Result.facilityType = type;
            Result.name = name;
            Result.location = location;
            Result.description = description;
            Result.floor = floor;
            Result.mapX = mapX;
            Result.mapY = mapY;
            Result.imagePath = imagePath ?? "";
            Result.delYn ??= "N";
            base.OnConfirm();
        }

        private void OnBrowseImage(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Title = "시설 이미지 선택",
                Filter = "이미지 (*.png;*.jpg;*.jpeg;*.webp)|*.png;*.jpg;*.jpeg;*.webp"
            };

            if (dialog.ShowDialog(FindForm()) != DialogResult.OK)
                return;

            if (!FacilityImageStore.TryValidateSelectedFile(dialog.FileName, out var error))
            {
                MessageBox.Show(error, "이미지 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _pendingImageSourcePath = dialog.FileName;
            _currentImagePath = null;
            UpdateImageUi();
        }

        private void UpdateImageUi()
        {
            if (!string.IsNullOrWhiteSpace(_pendingImageSourcePath))
            {
                _txtImagePath.Text = _pendingImageSourcePath;
                try
                {
                    _picPreview.Image?.Dispose();
                    _picPreview.Image = Image.FromFile(_pendingImageSourcePath);
                }
                catch
                {
                    _picPreview.Image = null;
                }
                return;
            }

            _txtImagePath.Text = _currentImagePath ?? "";

            _picPreview.Image?.Dispose();
            _picPreview.Image = string.IsNullOrWhiteSpace(_currentImagePath)
                ? null
                : FacilityImageStore.TryLoadThumbnail(_currentImagePath, 80);
        }

        private void OnFacilityTypeSwitched()
        {
            var type = GetSelectedFacilityType();
            if (string.IsNullOrEmpty(type))
                return;

            // 구분 전환 시 이미지·좌표 초기화
            if (_committedType != null
                && !string.Equals(_committedType, type, StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(type, "OUTER", StringComparison.OrdinalIgnoreCase))
                    ClearImageSelection();
                else if (string.Equals(type, "INNER", StringComparison.OrdinalIgnoreCase))
                    _mapPicker.Clear();
            }

            _committedType = type;
            UpdateTypePanels();
        }

        private string? GetSelectedFacilityType() =>
            _cmbType.SelectedItem?.ToString()?.Trim().ToUpperInvariant();

        private void ClearImageSelection()
        {
            _pendingImageSourcePath = null;
            _currentImagePath = null;
            UpdateImageUi();
        }

        private void UpdateTypePanels()
        {
            var inner = string.Equals(_cmbType.SelectedItem?.ToString(), "INNER", StringComparison.OrdinalIgnoreCase);
            _panelInner.Visible = inner;
            _panelOuter.Visible = !inner;
            _panelImage.Visible = inner;
            SetCardWidth(inner ? 420 : 540);
            FitCardSize();
        }

        private Panel BuildImagePanel()
        {
            const int previewSize = 88;
            const int inputHeight = 32;

            var panel = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0, 0, 0, 14)
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, previewSize + 8));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, previewSize));

            var lbl = new Label
            {
                Text = "시설 이미지",
                Font = ThemeFonts.BodySm,
                ForeColor = ThemeColors.TextMuted,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 6)
            };
            layout.Controls.Add(lbl, 0, 0);
            layout.SetColumnSpan(lbl, 2);

            _picPreview = new PictureBox
            {
                Size = new Size(previewSize, previewSize),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 8, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            var pathArea = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0)
            };
            pathArea.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            pathArea.RowStyles.Add(new RowStyle(SizeType.Absolute, inputHeight));
            pathArea.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            var pathRow = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0)
            };
            pathRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            pathRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, inputHeight));

            _txtImagePath = new TextBox
            {
                Height = inputHeight,
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                Font = ThemeFonts.Body,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 4, 0),
                PlaceholderText = "이미지 파일을 선택하세요"
            };

            _btnBrowse = new Button
            {
                Text = "...",
                Size = new Size(inputHeight, inputHeight),
                MinimumSize = new Size(inputHeight, inputHeight),
                MaximumSize = new Size(inputHeight, inputHeight),
                Dock = DockStyle.Fill,
                Font = ThemeFonts.Body,
                FlatStyle = FlatStyle.System,
                Cursor = Cursors.Hand,
                Margin = new Padding(0),
                TabStop = true
            };

            pathRow.Controls.Add(_txtImagePath, 0, 0);
            pathRow.Controls.Add(_btnBrowse, 1, 0);
            pathArea.Controls.Add(pathRow, 0, 1);

            layout.Controls.Add(_picPreview, 0, 1);
            layout.Controls.Add(pathArea, 1, 1);

            panel.Controls.Add(layout);
            return panel;
        }

        private static TextField AddField(TableLayoutPanel parent, string label, string? value, string placeholder, int row)
        {
            parent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var field = new TextField
            {
                FieldLabel = label,
                Text = value ?? "",
                Placeholder = placeholder,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };
            parent.Controls.Add(field, 0, row);
            return field;
        }

        private static TextField AddField(Panel parent, string label, string? value, string placeholder, int row)
        {
            var field = new TextField
            {
                FieldLabel = label,
                Text = value ?? "",
                Placeholder = placeholder,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 14)
            };
            parent.Controls.Add(field);
            return field;
        }

        private static ComboBox AddComboField(TableLayoutPanel parent, string label, int row)
        {
            parent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0, 0, 0, 14)
            };

            var lbl = new Label
            {
                Text = label,
                Font = ThemeFonts.BodySm,
                ForeColor = ThemeColors.TextMuted,
                AutoSize = true,
                Dock = DockStyle.Top
            };

            var cmb = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = ThemeFonts.Body,
                BackColor = ThemeColors.Surface
            };

            panel.Controls.Add(cmb);
            panel.Controls.Add(lbl);
            parent.Controls.Add(panel, 0, row);
            return cmb;
        }

        private static Panel AddSectionPanel(TableLayoutPanel parent, int row)
        {
            parent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0, 0, 0, 4)
            };
            parent.Controls.Add(panel, 0, row);
            return panel;
        }

        private static FacilityInfoDto CopyOf(FacilityInfoDto f) => new()
        {
            facilityId = f.facilityId,
            facilityType = f.facilityType,
            name = f.name,
            location = f.location,
            floor = f.floor,
            mapX = f.mapX,
            mapY = f.mapY,
            imagePath = f.imagePath,
            description = f.description,
            createdAt = f.createdAt,
            updatedAt = f.updatedAt,
            delYn = f.delYn
        };
    }
}
