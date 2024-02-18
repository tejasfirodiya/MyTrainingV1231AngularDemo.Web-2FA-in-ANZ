using Abp.Dependency;
using static System.Net.Mime.MediaTypeNames;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Services.UI
{
    public class HeaderButtonInfo
    {
        public Func<Task> OnClick { get; }

        public string Text { get; }

        public HeaderButtonInfo(string text, Func<Task> onClick)
        {
            OnClick = onClick;
            Text = text;
        }
    }

    public class PageHeaderService : ISingletonDependency
    {
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                TitleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler TitleChanged;


        public List<HeaderButtonInfo> HeaderButtonInfos { get; private set; }

        public event EventHandler HeaderButtonChanged;

        public void SetButtons(List<PageHeaderButton> buttons)
        {
            HeaderButtonInfos = new List<HeaderButtonInfo>();
            foreach (var button in buttons)
            {
                HeaderButtonInfos.Add(new HeaderButtonInfo(button.Text, button.ClickAction));
            }

            HeaderButtonChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ClearButton()
        {
            HeaderButtonInfos = new List<HeaderButtonInfo>();
            HeaderButtonChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
