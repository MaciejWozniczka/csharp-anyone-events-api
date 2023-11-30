namespace MobileApp.Host.Categories;

[ApiController]
public class ManageCategory : ControllerBase
{
    private readonly IMediator _mediator;
    public ManageCategory(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Category" }, Summary = "Add category")]
    [HttpPost("/api/category")]
    public async Task<Result<Guid>> PostCategoryAsync([FromBody] ManageCategoryCommand command)
    {
        return await _mediator.Send(command);
    }

    [Authorize]
    [SwaggerOperation(Tags = new[] { "Category" }, Summary = "Change category")]
    [HttpPut("/api/category/{id}")]
    public async Task<Result<Guid>> PutCategoryAsync(Guid id, [FromBody] ManageCategoryCommand command)
    {
        return await _mediator.Send(command.Set(p => p.Id = id));
    }

    public class ManageCategoryCommand : IRequest<Result<Guid>>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
    }

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ManageCategoryCommand, Category>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }

    public class ManageCategoryCommandHandler : IRequestHandler<ManageCategoryCommand, Result<Guid>>
    {
        private readonly DataContext _db;
        private readonly IMapper _mapper;
        public ManageCategoryCommandHandler(DataContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(ManageCategoryCommand request, CancellationToken cancellationToken)
        {
            Category category;
            var isAdding = request.Id == Guid.Empty;

            if (isAdding)
            {
                category = new Category();

                request.Id = Guid.NewGuid();
                request.CreateDate = DateTime.Now;
                request.IsDeleted = false;

                await _db.AddAsync(category, cancellationToken);
            }
            else
            {
                request.CreateDate = DateTime.Now;

                category = await _db.Categories
                    .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

                if (category == null)
                {
                    return Result.NotFound<Guid>(request.Id);
                }
            }

            category = _mapper.Map(request, category);

            return Result.Ok(category.Id);
        }
    }
}