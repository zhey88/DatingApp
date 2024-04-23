using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public MessageRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Message> GetMessage(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        //Get the query for that particular container and then 
        //project that to a message DTO, which we then return as a paged list
        var query = _context.Messages
        //To get the most recent message first
            .OrderByDescending(x => x.MessageSent)
            .AsQueryable();

        //so that we can choose which container we're interested in viewing
        query = messageParams.Container switch
        {
            //So because we still have the message in the database, if only one side has actually deleted it
            //in our repository we're just going to check for that and simply not return messages 
            //if the sender or the recipient have flagged it for deletion on their side
            //Only show the message to when either side of the user did not delete the message
            "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username &&
             u.RecipientDeleted == false),
             //sender becuase this is messages sent from that particular user
            "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username &&
                u.SenderDeleted == false),
                //Default case, Inbox
            _ => query.Where(u => u.Recipient.UserName == messageParams.Username
                && u.RecipientDeleted == false && u.DateRead == null)
        };
        //ProjectTo helps you transform the results of a query into a new form
        var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

        return await PagedList<MessageDto>
            .CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public Task<PagedList<MessageDto>> GetMessagesForUser()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread
            (string currentUsername, string recipientUsername)
    {   //We need to get the messages for both sides of the conversation.
        var messages = await _context.Messages
        //use thenInclude because the photos are a related entity for the app user
            .Include(u => u.Sender).ThenInclude(p => p.Photos)
            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            .Where(
                //so we want to return messages where the recipient username is equal to currentUsername
                //and The sender username is equal to the recipient username
                //Only show the message to when either side of the user did not delete the message
                m => m.RecipientUsername == currentUsername && m.RecipientDeleted == false &&
                m.SenderUsername == recipientUsername ||
                m.RecipientUsername == recipientUsername &&
                m.SenderUsername == currentUsername && m.SenderDeleted == false
            )

            .OrderBy(m => m.MessageSent)
            //Save in the memory
            .ToListAsync();

        //get a list of the unread messages in memory and we're going to 
        //take the opportunity to mark them as sent
        var unreadMessages = messages.Where(m => m.DateRead == null
        //RecipientUsername == currentUsername because it is the recipient who will read them
        //ToList() so that we no need to get them from database, we could just get them from memory
            && m.RecipientUsername == currentUsername).ToList();

        //Check if we have any unread messages
        if (unreadMessages.Any())
        {
            //mark the message as read as soon as it's received by the recipient
            foreach (var message in unreadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int recipientId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}