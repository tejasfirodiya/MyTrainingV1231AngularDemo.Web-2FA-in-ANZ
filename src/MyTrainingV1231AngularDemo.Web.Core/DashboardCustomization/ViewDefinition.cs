namespace MyTrainingV1231AngularDemo.Web.DashboardCustomization
{
    public class ViewDefinition
    {
        public string Id { get; protected set; }

        public string ViewFile { get; protected set; }

        public string JavascriptFile { get; protected set; }

        public string JavascriptClassName { get; protected set; }

        public string CssFile { get; protected set; }

        public ViewDefinition(
            string id,
            string viewFile,
            string javascriptFile = null,
            string cssFile = null,
            string javascriptClassName = null
        )
        {
            Id = id;
            ViewFile = viewFile;
            JavascriptFile = javascriptFile;
            CssFile = cssFile;
            JavascriptClassName = javascriptClassName ?? id;
        }
    }
}