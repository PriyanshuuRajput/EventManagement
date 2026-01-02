namespace Applications.Dto
{
    public class HomeBannerDto
    {
            public int Id { get; set; }
            public string Image { get; set; } = string.Empty;
            public string? Title { get; set; }
            public int? EventId { get; set; }
            public string? EventTitle { get; set; }
            public string? Link { get; set; }
            public int Position { get; set; }
            public bool Status { get; set; }

    }
}
