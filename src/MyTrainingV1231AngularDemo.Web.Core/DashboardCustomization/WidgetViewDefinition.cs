namespace MyTrainingV1231AngularDemo.Web.DashboardCustomization
{
    public class WidgetViewDefinition : ViewDefinition
    {
        public byte DefaultWidth { get; }

        public byte DefaultHeight { get; }

        public WidgetViewDefinition(
            string id,
            string viewFile,
            string javascriptFile = null,
            string cssFile = null,
            byte defaultWidth = 12,
            byte defaultHeight = 10,
            string javascriptClassName = null) : base(id, viewFile, javascriptFile, cssFile, javascriptClassName)
        {
            DefaultWidth = defaultWidth;
            DefaultHeight = defaultHeight;
        }
    }
}