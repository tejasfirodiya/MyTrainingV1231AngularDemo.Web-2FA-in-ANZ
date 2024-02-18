namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Services.UI
{
    public class PageHeaderButton
    {
        public string Text { get; set; }

        public Func<Task> ClickAction { get; set; }

        public PageHeaderButton(string text, Func<Task> clickAction)
        {
            Text = text;
            ClickAction = clickAction;  
        }
    }
}
