using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using Emailit.Data;
using Emailit.Models;
using Emailit.Services;
using Emailit.Services.Data;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Emailit.Pages.Message
{
    [RequestSizeLimit(2147483648)] //2GB
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _Logger;
        private readonly IUserRepository _userData;
        private readonly IMessageRepository _messageData;
        private readonly IFileManagement _fileManagement;
        private readonly IActionContextAccessor _ActionContextAccessor;
        private readonly IAttachedFileRepository _attachedFileRepository;
        private readonly IFileDataRepository _fileData;
        private readonly IHubNotificationHelper<NewMessageNotification> _Notifications;
        private readonly IReceivedMessageStateRepository _receivedMessageStateData;

        const int maxSizePerFileInMB = MessageDataValidation.maxSizePerFileInMB;
        const int maxFiles = MessageDataValidation.maxFiles;

        public CreateModel(ILogger<CreateModel> logger, IActionContextAccessor actionContextAccessor, IUserRepository userRepository, 
            IMessageRepository messageRepository, IFileManagement fileManagement, IAttachedFileRepository attachedFileRepository, 
            IFileDataRepository fileRepository, IHubNotificationHelper<NewMessageNotification> notificationHelper,
            IReceivedMessageStateRepository receivedMessageStateRepository)
        {
            _Logger = logger;
            _userData = userRepository;
            _messageData = messageRepository;
            _fileManagement = fileManagement;
            _ActionContextAccessor = actionContextAccessor;
            _attachedFileRepository = attachedFileRepository;
            _fileData = fileRepository;
            _Notifications = notificationHelper;
            _receivedMessageStateData = receivedMessageStateRepository;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnGetEmailsAsync(string search)
        {

            if (!string.IsNullOrWhiteSpace(search) && Conversions.ExtraSpaceRemover(search).Length < MessageDataValidation.MinLenghtSearchFieldOfUSersEmails)
            {
                return StatusCode(400);
            }

            List<User> users = await _userData.GetAllAsync(search, 5);
            var json = new JsonResult(users.Select(u => new { u.UserId, u.Name, u.Lastname, u.Email }));
            return json;
        }


        public async Task<IActionResult> OnPostAsync(string recipientsId, string ccRecipientsId, string tittle, string body,
            bool Confidential, MessagePriority priority, IList<IFormFile> formFiles)
        {
            if (string.IsNullOrWhiteSpace(recipientsId) || string.IsNullOrWhiteSpace(tittle) || string.IsNullOrWhiteSpace(body) ||
                !string.IsNullOrWhiteSpace(tittle) && tittle.Length > 254 || formFiles.Count() > maxFiles)
            {
                return StatusCode(400);
            }

            foreach (var file in formFiles)
            {
                var sizeMB = file.Length / 1024 / 1024;

                if (sizeMB > maxSizePerFileInMB)
                    return StatusCode(400);
            }

            //Getting UserId of the current user, from the cookie
            var userIdClaim = _ActionContextAccessor.ActionContext.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            //Validating User Claim
            if (!int.TryParse(userIdClaim, out int userId))
            {
                _Logger.LogError($"An error occurred while obtaining the user ID.");
                return Page();
            }

            Models.Message message = new Models.Message
            {
                SenderId = userId,
                Tittle = tittle,
                Confidential = Confidential,
                Priority = priority,
                Recipients = new List<ReceivedMessage>(),
                Date = DateTime.UtcNow

            };

            var recipients = recipientsId.Split(',').Distinct().ToList();

            foreach (var item in recipients)
            {
                if (int.TryParse(item, out int id) && await _userData.CheckAsync(id))
                {
                    message.Recipients.Add(new ReceivedMessage
                    {
                        UserId = id,
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(ccRecipientsId))
            {
                foreach (var item in ccRecipientsId.Split(',').Distinct().Except(recipients).ToList())
                {
                    if (int.TryParse(item, out int id) && await _userData.CheckAsync(id))
                    {
                        message.Recipients.Add(new ReceivedMessage
                        {
                            UserId = id,
                            CC = true
                        });
                    }
                }
            }

            if (!message.Recipients.Any())
            {
                Response.Headers.Add("app-message", "You should check the recipients, since no user was found to send this message to.");
                return Page();
            }

            bool error = false;

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var (finalHtml, imagesFromSummernote) = await SaveImagesBase64FromSummernote(body, maxSizePerFileInMB);

                    message.Body = finalHtml;
                    message.BodyInHtml = finalHtml;

                    await _messageData.AddAsync(message);

                    IList<FileData> attachments = new List<FileData>();

                    attachments = await _fileData.AddAsync(await _fileManagement.SaveFilesAsync(formFiles));

                    foreach (var img in imagesFromSummernote)
                        attachments.Add(img);

                    foreach (var item in attachments)
                        await _attachedFileRepository.AddAsync(new AttachedFile { Message = message, File = item });

                    var normalizedMessage = await _messageData.GetAsync(message.MessageId);

                    NewMessageNotification notification = new NewMessageNotification
                    {
                        MessageId = normalizedMessage.MessageId,
                        Tittle = normalizedMessage.Tittle,
                        Content = normalizedMessage.Body,
                        Author = normalizedMessage.Sender.Email,
                        ItHasAttachedFiles = attachments.Any(),
                        Confidential = normalizedMessage.Confidential,
                        Priority = normalizedMessage.Priority
                    };


                    foreach (var Id in normalizedMessage.Recipients.Select(d => d.UserId))
                        if (_Notifications.GetOnlineUsers().Any(u => u == Id))
                        {

                            await _receivedMessageStateData.AddAsync(new ReceivedMessageState
                            {
                                UserId = Id,
                                MessageId = normalizedMessage.MessageId,
                                State = MessageState.received,
                                Date = DateTime.UtcNow
                            });

                            await _Notifications.SendNotification(Id, notification);

                        }


                    scope.Complete();
                }
                catch (Exception ex)
                {
                    _Logger.LogError($"error saving message: {ex}");
                    error = true;
                }
            }

            if (error)
            {
                return StatusCode(500);
            }

            //  Response.Headers.Add("id-message", mensaje.MessageId.ToString());
            return Page();
        }



        /// <summary>
        /// Method which stores images from a Summernote HTML in the database and as a local file.
        /// </summary>
        /// <param name="html">Html to modify. In case there are no image tags, it will remain unchanged.</param>
        /// <param name="maxSizeMB">Maximum size allowed per file in MB</param>
        /// <returns>It returns two objects: The first of type string, which results in the already modified html 
        /// with its respective image nodes and its updated src.
        /// The second, a list of type FileData, which will contain the images that were stored in the DB and locally.
        /// </returns>
        private async Task<(string finalHtml, IList<FileData> imgAgregadas)> SaveImagesBase64FromSummernote(string html, int maxSizeMB)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HttpUtility.HtmlDecode(html));

            IList<FileData> files = new List<FileData>();

            //Getting nodes of img
            var images = doc.DocumentNode.SelectNodes("//img");


            if (images != null && images.Count > 0)
            {
                for (int i = 0; i < images.Count; i++)
                {
                    var img = images[i];
                    try
                    {
                        if (i >= maxFiles)
                        {
                            img.Remove();
                            continue;
                        }

                        string dataUrl = img.Attributes["src"].Value;

                        //if (Url.IsLocalUrl(dataUrl))
                        //    continue;

                        string base64 = dataUrl.Substring(dataUrl.LastIndexOf(',') + 1);
                        string MimeType = dataUrl.Substring(dataUrl.IndexOf(":") + 1, dataUrl.IndexOf(";") - dataUrl.IndexOf(":") - 1);
                        string imageName = img.Attributes["data-filename"].Value;

                        if (string.IsNullOrWhiteSpace(dataUrl) || string.IsNullOrWhiteSpace(base64) || string.IsNullOrWhiteSpace(MimeType)
                            || string.IsNullOrWhiteSpace(imageName))
                        {
                            img.Remove();
                            continue;
                        }

                        //Calculating file size
                        var paddingCount = base64.Substring(base64.Length - 2, 2).Count(c => c == '=');
                        var size = 3 * (base64.Length / 4) - paddingCount;

                        if (size / 1024 / 1024 > maxSizeMB)
                        {
                            img.Remove();
                            continue;
                        }

                        IFormFile formFile;

                        using var stream = new MemoryStream(Convert.FromBase64String(base64));
                        formFile = new FormFile(stream, 0, stream.Length, Path.GetFileNameWithoutExtension(imageName), imageName)
                        {
                            Headers = new HeaderDictionary(),
                            ContentType = MimeType
                        };

                        var file = await _fileData.AddAsync(await _fileManagement.SaveFileAsync(formFile));
                        var path = Url.Page("GetFile", new { id = file.FileId });

                        img.SetAttributeValue("src", path);
                        img.Attributes["data-filename"].Remove();

                        files.Add(file);
                    }
                    catch (Exception ex)
                    {
                        _Logger.LogDebug($"Error saving image from Summernote: {ex}");
                        img.Remove();
                    }

                }

                html = doc.DocumentNode.WriteTo();
            }

            return (html, files);
        }

    }
}