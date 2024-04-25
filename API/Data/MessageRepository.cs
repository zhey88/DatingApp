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

    public void AddGroup(Group group)
    {
        _context.Groups.Add(group);
    }

    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Connection> GetConnection(string connectionId)
    {
        return await _context.Connections.FindAsync(connectionId);
    }

    public async Task<Group> GetGroupForConnection(string connectionId)
    {
        return await _context.Groups
            .Include(x => x.Connections)
            .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
            //going to return the group for that particular connection
            .FirstOrDefaultAsync();
    }

    public async Task<Message> GetMessage(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<Group> GetMessageGroup(string groupName)
    {
        //eagerly load the connections
        return await _context.Groups
            .Include(x => x.Connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);
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

    public async Task<IEnumerable<MessageDto>> GetMessageThread
            (string currentUsername, string recipientUsername)
    {   //We need to get the messages for both sides of the conversation.
        var query = _context.Messages
        //if we're using projection, we don't need to eagerly load the other related entities
        //use thenInclude because the photos are a related entity for the app user

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
            .AsQueryable();


        //get a list of the unread messages  from our query and we're going to 
        //take the opportunity to mark them as sent
        var unreadMessages = query.Where(m => m.DateRead == null
        //RecipientUsername == currentUsername because it is the recipient who will read them
        //ToList() so that we no need to get them from database, we could just get them from query
            && m.RecipientUsername == currentUsername).ToList();

        //Check if we have any unread messages
        if (unreadMessages.Any())
        {
            //mark the message as read as soon as it's received by the recipient
            //unread messages to be updated with date read
            foreach (var message in unreadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }
        }

        //So now the query is more cleaner, unnecessary info such as photo is not included
        return await query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public void RemoveConnection(Connection connection)
    {
        _context.Connections.Remove(connection);
    }
}