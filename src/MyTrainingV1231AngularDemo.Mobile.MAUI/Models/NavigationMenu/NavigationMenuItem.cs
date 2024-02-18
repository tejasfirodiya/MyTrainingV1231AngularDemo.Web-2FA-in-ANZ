namespace MyTrainingV1231AngularDemo.Models.NavigationMenu
{
    public class NavigationMenuItem
    {
        private bool _isSelected;

        public string Title { get; set; }

        public string Icon { get; set; }

        public string NavigationUrl { get; set; }

        public object NavigationParameter { get; set; }

        public string RequiredPermissionName { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
            }
        }

        /// <summary>
        /// Sub items of this menu item.
        /// </summary>
        public List<NavigationMenuItem> Items { get; set; }

        public NavigationMenuItem()
        {
            Items = new List<NavigationMenuItem>();
        }
    }
}
