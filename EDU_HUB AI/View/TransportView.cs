using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Domain;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.exception;
using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;

namespace EDU_HUB_AI.View
{
    public partial class TransportView : UserControl
    {
        private List<TransportDto> _all = [];
        private readonly AdminTransportController _controller = new();
        private readonly Dictionary<string, TransportTypeCard> _cards = new(StringComparer.OrdinalIgnoreCase);

        public TransportView()
        {
            InitializeComponent();
            BackColor = ThemeColors.Background;
            bodyPanel.BackColor = ThemeColors.Background;
            scrollPanel.BackColor = ThemeColors.Background;
            cardsPanel.BackColor = ThemeColors.Background;

            BuildCards();
            pageHeader1.SyncClicked += async (_, _) => await LoadAndRender();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadAndRender();
        }

        private void BuildCards()
        {
            cardsPanel.Controls.Clear();
            _cards.Clear();

            var types = TransportTypes.All;
            var rowCount = (int)Math.Ceiling(types.Length / 2.0);
            cardsPanel.RowCount = rowCount;
            cardsPanel.RowStyles.Clear();
            for (var r = 0; r < rowCount; r++)
                cardsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                var card = new TransportTypeCard(type)
                {
                    Dock = DockStyle.Fill,
                    Height = 300,
                    Margin = new Padding(0, 0, 12, 12)
                };

                card.CreateRequested += (_, _) => OnCreate(type, card);
                card.ActionRequested += (_, e) => OnRowAction(type, card, e);

                cardsPanel.Controls.Add(card, i % 2, i / 2);
                _cards[type] = card;
            }
        }

        private async Task<List<TransportDto>> LoadData()
        {
            var res = await _controller.GetTransportList();
            return res?.Data ?? [];
        }

        private async Task LoadAndRender(bool showOverlay = true)
        {
            var overlay = showOverlay ? LoadingOverlay.Create(bodyPanel, "데이터 로딩 중...") : null;
            _controller.OnRetry = (attempt, max) => overlay?.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

            try
            {
                _all = await LoadData();
                foreach (var (type, card) in _cards)
                    card.BindItems(_all);
            }
            finally
            {
                _controller.OnRetry = null;
                overlay?.Close();
                overlay?.Dispose();
            }
        }

        private async void OnCreate(string type, TransportTypeCard card)
        {
            var template = card.GetItemAt(0);
            TransportDto? draft = template == null
                ? null
                : new TransportDto
                {
                    type = type,
                    departLocation = template.departLocation,
                    destination = template.destination
                };

            var created = TransportEditModal.Show(FindForm(), draft, type);
            if (created == null) return;

            var overlay = LoadingOverlay.Create(bodyPanel, "등록 중...");
            _controller.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

            try
            {
                var res = await _controller.InsertTransport(created);
                if (res?.Status == 200)
                    await LoadAndRender(showOverlay: false);
                else
                    MessageBox.Show(FindForm(), res?.Message ?? "등록에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch(ApiException ex) { MessageBox.Show(FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            catch (Exception ex) { MessageBox.Show(FindForm(), $"요청 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { _controller.OnRetry = null; overlay.Close(); overlay.Dispose(); }
        }

        private async void OnRowAction(string type, TransportTypeCard card, TableActionEventArgs e)
        {
            var target = card.GetItemAt(e.RowIndex);
            if (target == null) return;

            if (e.Action == TableAction.Edit)
            {
                var edited = TransportEditModal.Show(FindForm(), target);
                if (edited == null || string.IsNullOrWhiteSpace(target.transportId)) return;

                var overlay = LoadingOverlay.Create(bodyPanel, "수정 중...");
                _controller.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

                try
                {
                    var res = await _controller.UpdateTransport(target.transportId, edited);
                    if (res?.Status == 200)
                        await LoadAndRender(showOverlay: false);
                    else
                        MessageBox.Show(FindForm(), res?.Message ?? "수정에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (ApiException ex) { MessageBox.Show(FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                catch (Exception ex) { MessageBox.Show(FindForm(), $"요청 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                finally { _controller.OnRetry = null; overlay.Close(); overlay.Dispose(); }
            }
            else if (e.Action == TableAction.Delete)
            {
                var label = $"{TransportTypes.GetLabel(target.type)} {target.departTime}";
                if (!ConfirmModal.Show(FindForm(), "삭제 확인", $"'{label}' 운행을 삭제할까요?"))
                    return;

                if (string.IsNullOrWhiteSpace(target.transportId)) return;

                var overlay = LoadingOverlay.Create(bodyPanel, "삭제 중...");
                _controller.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");

                try
                {
                    var res = await _controller.DeleteTransport(target.transportId);
                    if (res?.Status == 200)
                        await LoadAndRender(showOverlay: false);
                    else
                        MessageBox.Show(FindForm(), res?.Message ?? "삭제에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (ApiException ex) { MessageBox.Show(FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                catch (Exception ex) { MessageBox.Show(FindForm(), $"요청 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                finally { _controller.OnRetry = null; overlay.Close(); overlay.Dispose(); }
            }
        }
    }
}
