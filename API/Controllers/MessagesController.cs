using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class MessagesController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public MessagesController(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;

    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        //Get the username
        var username = User.GetUsername();

        //Check if the user is sending message to himself
        if (username == createMessageDto.RecipientUsername.ToLower())
            return BadRequest("You cannot send messages to yourself");
        //get hold of the sender and rthe recipient of the message
        var sender = await _uow.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await _uow.UserRepository.GetUserByUsernameAsync
                            (createMessageDto.RecipientUsername);

        //Check if the recipient is null
        if (recipient == null) return NotFound();

        //Create the message
        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        //Add the message to the repository
        //in order for Entity Framework to track this, then we need to use the context.ADD
        _uow.MessageRepository.AddMessage(message);

        //Save these to the database
        if (await _uow.Complete()) return Ok(_mapper.Map<MessageDto>(message));

        return BadRequest("Failed to send message");
    }

    [HttpGet]
    //we're going to pass this information up as a query string, but we're going to use our messageparams
    public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]
        MessageParams messageParams)
    {
        //Get the username in the claim principle
        messageParams.Username = User.GetUsername();

        var messages = await _uow.MessageRepository.GetMessagesForUser(messageParams);

        //Add the reponse to the paginationHeader
        Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage,
            messages.PageSize, messages.TotalCount, messages.TotalPages));

        return messages;
    }

    //To delete the message with the messageId
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        //Do alot of checks because we're not going to actually delete the message 
        //until both user have deleted that specific message.
        //Get the usernmae
        var username = User.GetUsername();
        //Get the message
        var message = await _uow.MessageRepository.GetMessage(id);
        //Check the user is attempting to delete this message is either sender or the recipient
        if (message.SenderUsername != username && message.RecipientUsername != username) 
            return Unauthorized();

        if (message.SenderUsername == username) message.SenderDeleted = true;

        if (message.RecipientUsername == username) message.RecipientDeleted = true;
        //check to see if both the sender and the recipient have deleted the message
        if (message.SenderDeleted && message.RecipientDeleted)
        {
            //Then we going to delete the message from the database
            _uow.MessageRepository.DeleteMessage(message);
        }
        //Update the database
        if (await _uow.Complete()) return Ok();

        return BadRequest("Problem deleting the message");
    }
}