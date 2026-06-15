using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;
using Org.BouncyCastle.Asn1.X509;

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
            _cmbType = AddComboField(stack, "구분", row++, required: true);
            _txtName = AddField(stack, "시설명", source?.name, "시설 이름", row++);
            _txtName.Required = true;
            _txtName.TextChanged += (_, _) => _txtName.HasError = false;
            _txtLocation = AddField(stack, "위치", source?.location, "건물·동·방향", row++);
            _txtLocation.Required = true;
            _txtLocation.TextChanged += (_, _) => _txtLocation.HasError = false;
            _txtDescription = AddField(stack, "안내", source?.description, "이용 안내", row++);

            _panelInner = AddSectionPanel(stack, row++);
            _panelImage = BuildImagePanel();
            _panelInner.Controls.Add(_panelImage);
            _txtFloor = AddField(_panelInner, "층수", source?.floor?.ToString(), "예) 2", 0);
            _txtFloor.Required = true;
            _txtFloor.TextChanged += (_, _) => _txtFloor.HasError = false;

            _panelOuter = AddSectionPanel(stack, row++);
            _mapPicker = new FacilityMapPicker();
            _panelOuter.Controls.Add(_mapPicker);
            if (string.Equals(initialType, "OUTER", StringComparison.OrdinalIgnoreCase))
                _mapPicker.SetCoordinates(source?.mapX, source?.mapY);
            else
                _mapPicker.Clear();

            _cmbType.Items.AddRange([
                new TypeItem("INNER", "내부"),
                new TypeItem("OUTER", "외부")
                ]);
            _cmbType.DisplayMember = "Label";
            _cmbType.SelectedIndexChanged += (_, _) => OnFacilityTypeSwitched();
            SelectType(initialType);

            _btnBrowse.Click += OnBrowseImage;
            _picPreview.Click += OnBrowseImage;
            _picPreview.Cursor = Cursors.Hand;

            Body.Controls.Add(stack);
            UpdateImageUi();
            UpdateTypePanels();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var targetH = _panelInner.Height;
            _mapPicker.Height = targetH;
            _panelOuter.AutoSize = false;
            _panelOuter.Height = targetH;
            FitCardSize();
        }

        private void SelectType(string? value)
        {
            var normalized = value?.Trim().ToUpperInvariant();

            for(var i = 0; i <_cmbType.Items.Count; i++)
            {
                if (_cmbType.Items[i] is TypeItem item && string.Equals(item.Value, normalized, StringComparison.OrdinalIgnoreCase))
                {
                    _cmbType.SelectedIndex = i;
                    return;
                }
            }

            if(_cmbType.Items.Count > 0)
            {
                _cmbType.SelectedIndex = 0;
            }
        }

        public static FacilityInfoDto? Show(IWin32Window owner, FacilityInfoDto? source)
        {
            using var modal = new FacilityEditModal(source);
            return modal.ShowDialog(owner) == DialogResult.OK ? modal.Result : null;
        }

        protected override async void OnConfirm()
        {
            _txtName.HasError = false;
            _txtLocation.HasError = false;
            _txtFloor.HasError = false;

            var type = GetSelectedFacilityType();
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
                _txtName.HasError = true;
                MessageBox.Show("시설명을 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(location))
            {
                _txtLocation.HasError = true;
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
                    _txtFloor.HasError = true;
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
                mapX = Math.Round(_mapPicker.MapX.Value, 6);
                mapY = Math.Round(_mapPicker.MapY.Value, 6);
            }

            bool isEdit = _source != null;
            if (!ConfirmModal.Show(Owner,
                isEdit ? "수정 확인" : "등록 확인",
                isEdit ? "수정하시겠습니까?" : "등록하시겠습니까?",
                isEdit ? "수정" : "등록",
                ButtonVariant.Primary))
                return;

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
                _picPreview.Invalidate();
                return;
            }

            _txtImagePath.Text = _currentImagePath ?? "";

            _picPreview.Image?.Dispose();
            _picPreview.Image = string.IsNullOrWhiteSpace(_currentImagePath)
                ? null
                : FacilityImageStore.TryLoadThumbnail(_currentImagePath, 400);
            _picPreview.Invalidate();
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
            (_cmbType.SelectedItem as TypeItem)?.Value?.Trim().ToUpperInvariant();

        private void ClearImageSelection()
        {
            _pendingImageSourcePath = null;
            _currentImagePath = null;
            UpdateImageUi();
        }

        private void UpdateTypePanels()
        {
            var inner = string.Equals(GetSelectedFacilityType(), "INNER", StringComparison.OrdinalIgnoreCase);
            _panelInner.Visible = inner;
            _panelOuter.Visible = !inner;
            _panelImage.Visible = inner;
            SetCardWidth(540);
            FitCardSize();
        }

        private Panel BuildImagePanel()
        {
            const int totalH = 280;
            const int footerH = 22;

            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = totalH,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0, 0, 0, 14)
            };

            _txtImagePath = new TextBox
            {
                Dock = DockStyle.Bottom,
                Height = footerH,
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                Font = ThemeFonts.BodySm,
                BackColor = ThemeColors.Surface,
                ForeColor = ThemeColors.TextMuted,
                PlaceholderText = "이미지를 클릭해 파일을 선택하세요"
            };

            _picPreview = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };

            _picPreview.Paint += (_, e) =>
            {
                if (_picPreview.Image != null)
                {
                    return;
                }
                using var pen = new Pen(ThemeColors.Border);
                e.Graphics.DrawRectangle(pen, 0, 0, _picPreview.Width - 1, _picPreview.Height - 1);

                TextRenderer.DrawText(e.Graphics, "이미지를 클릭해 선택하세요", ThemeFonts.Body, _picPreview.ClientRectangle, ThemeColors.TextMuted,
                                       TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };

            _btnBrowse = new Button
            {
                Visible = false,
                Size = new Size(1, 1)
            };

            panel.Controls.Add(_picPreview);
            panel.Controls.Add(_txtImagePath);
            panel.Controls.Add(_btnBrowse);
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

        private static ComboBox AddComboField(TableLayoutPanel parent, string label, int row, bool required = false)
        {
            parent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0, 0, 0, 14)
            };

            var lblPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = ThemeColors.Surface,
                Margin = new Padding(0),
                Padding = new Padding(0),
                Dock = DockStyle.Top
            };
            lblPanel.Controls.Add(new Label
            {
                Text = label,
                Font = ThemeFonts.BodySm,
                ForeColor = ThemeColors.TextMuted,
                AutoSize = true,
                Margin = new Padding(0)
            });
            if(required)
            {
                lblPanel.Controls.Add(new Label
                {
                    Text = " *",
                    Font = ThemeFonts.BodySm,
                    ForeColor = ThemeColors.Danger,
                    AutoSize = true,
                    Margin = new Padding(0)
                });
            }

            var cmb = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = ThemeFonts.Body,
                BackColor = ThemeColors.Surface
            };

            panel.Controls.Add(cmb);
            panel.Controls.Add(lblPanel);
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
        private sealed class TypeItem(string value, string label)
        {
            public string Value { get; } = value;
            public string Label { get; } = label;
        }
    }
}
