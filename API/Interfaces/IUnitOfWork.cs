namespace API.Interfaces
{

    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IMessageRepository MessageRepository { get; }
        ILikesRepository LikesRepository { get; }
        Task<bool> Complete();

        //this is going to tell us if Entity Framework is tracking anything 
        //that's been changed inside its transaction
        bool HasChanges();
    }

}