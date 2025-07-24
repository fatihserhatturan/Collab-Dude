using AnnounceService.Domain.Entities;
using System.Linq.Expressions;

namespace AnnounceService.Domain.Specifications;

public class ActiveAnnouncesSpecification : BaseSpecification<Announce>
{
    public ActiveAnnouncesSpecification() : base(x => x.Status == AnnounceStatus.Active && !x.IsDeleted)
    {
        AddInclude(x => x.Category);
        AddOrderByDescending(x => x.CreatedAt);
    }
}

public class AnnouncesByCategorySpecification : BaseSpecification<Announce>
{
    public AnnouncesByCategorySpecification(Guid categoryId) 
        : base(x => x.CategoryId == categoryId && x.Status == AnnounceStatus.Active && !x.IsDeleted)
    {
        AddInclude(x => x.Category);
        AddOrderByDescending(x => x.CreatedAt);
    }
}

public class AnnouncesByUsernameSpecification : BaseSpecification<Announce>
{
    public AnnouncesByUsernameSpecification(string username) 
        : base(x => x.Username == username && !x.IsDeleted)
    {
        AddInclude(x => x.Category);
        AddInclude(x => x.Applications);
        AddOrderByDescending(x => x.CreatedAt);
    }
}

public class SearchAnnouncesSpecification : BaseSpecification<Announce>
{
    public SearchAnnouncesSpecification(string searchTerm) 
        : base(x => (x.Title.Contains(searchTerm) || x.Description!.Contains(searchTerm) || x.Content.Contains(searchTerm)) 
                    && x.Status == AnnounceStatus.Active && !x.IsDeleted)
    {
        AddInclude(x => x.Category);
        AddOrderByDescending(x => x.CreatedAt);
    }
}