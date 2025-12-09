namespace Applications.Dto
{
    public class BreadCrumbItemDto
    {
        public string Label { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }

        public BreadCrumbItemDto(string label, string url, bool isActive = false)
        {
            Label = label;
            Url = url;
            IsActive = isActive;
        }
    }
}

