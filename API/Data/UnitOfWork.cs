using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    //going to be returning our repositories
    //we're going to be injecting UnitOfWork into our controllers instead of the repositories
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public UnitOfWork(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }
        public IUserRepository UserRepository => new UserRepository(_context, _mapper);

        public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

        public ILikesRepository LikesRepository => new LikesRepository(_context);

        public async Task<bool> Complete()
        {
            //So as long as there's more than zero changes, this is going to return true
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            //we just want to know if the context has changes that it is tracking
            //we can access the change tracker and we can use the hasChanges method
            //returns a boolean If entity Framework is tracking any changes to our entities in memory.
            //we're going to get that information from this method
            return _context.ChangeTracker.HasChanges();
        }
    }

}