using NotikaIdentityEmail.Entities;

namespace NotikaIdentityEmail.Models.CommentViewModels;

public class CommentViewModel
{
    public Comment? Comment { get; set; }

    public IEnumerable<Comment> Comments { get; set; } = [];

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 6;

    public int TotalCount { get; set; }

    public int TotalPages => PageSize <= 0 ? 0 : (TotalCount + PageSize - 1) / PageSize;

    public bool HasPrevious => PageNumber > 1;

    public bool HasNext => PageNumber < TotalPages;

    public int CommentCount => TotalCount > 0 ? TotalCount : Comments?.Count() ?? 0;
}
