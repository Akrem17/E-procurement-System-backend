﻿#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_proc.Models;
using E_proc.Repositories.Interfaces;
using E_proc.Models.StatusModel;
using E_proc.MyHub;
using Microsoft.AspNetCore.SignalR;

namespace E_proc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffersController : ControllerBase
    {
        private readonly AuthContext _context;
        private IFileDataRepository _fileRepository;
        private IOfferRepository _offerRepository;
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        public OffersController(AuthContext context, IFileDataRepository fileRepository, IOfferRepository offerRepository, IHubContext<NotificationHub> notificationHubContext)
        {
            _context = context;
            _fileRepository = fileRepository;
            _offerRepository = offerRepository;
            _notificationHubContext = notificationHubContext;


        }

        // GET: api/Offers
        [HttpGet]
        public async Task<IActionResult> GetOffer(int? skip = 0, int? take = 10, string? supplierId = null, string? supplierEmail = null,string? offerNumber=null, string? tenderName = null, string? city = null, string? postDate = null)
        {
            if (supplierId == null &&   supplierEmail == null &&  offerNumber == null &&  tenderName == null &&  city == null && postDate == null)
            {
                var offer = await _offerRepository.ReadAsync((int)skip, (int)take);
                var itemsNumber = _offerRepository.CountData();

                if (offer == null) return new Success(false, "message.OfferNotFound");

                return new Success(true, "message.sucess", offer);
            }
            else
            {
                var offer = await _offerRepository.FindBy((int)skip, (int)take,supplierId, supplierEmail, offerNumber , tenderName,  city ,   postDate );
                var itemsNumber = _offerRepository.CountDataWithFilters((int)skip, (int)take, supplierId, supplierEmail, offerNumber, tenderName, city, postDate);


                if (offer == null) return new Success(false, "message.UserNotFound");

                return new Success(true, "message.sucess",new  { offer , itemsNumber });
            }
            return new Success(false, "message.failed");


        }

        // GET: api/Offers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOffer(int id)

        {

            var offer = await _offerRepository.ReadById(id);
            if (offer == null) return new Success(false, "message.user Not Found");
            return new Success(true, "message.success", offer);

        }

        // PUT: api/Offers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOffer(int id, Offer offer)
        {
            var newOffer = await _offerRepository.UpdateAsync(id, offer);

            if (newOffer == null)
                return new Success(false, "message.User not found ");
            return new Success(true, "message.success", newOffer);
        }

        [HttpPost("files")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PostAsync([FromForm] FileModel model, int? tenderId)
        {


            try
            {
                List<FileRecord> files = await _fileRepository.SaveFileAsync(model.MyFile);


                if (files.Count() != 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        files[i].AltText = model.AltText;
                        files[i].Description = model.Description;
                    }



                    var file = _fileRepository.SaveToDBForOffer(files, tenderId);
                    return new Success(true, "message.success", file);
                }
                else
                    return new Success(false, "message.failed");

            }
            catch (Exception ex)
            {
                return new Success(false, "message." + ex.Message);
            }
        }



        [HttpPost]
        public async Task<IActionResult> PostOffer(Offer offer)
        {
            if (offer != null)
            {



                var offerAdded = await _offerRepository.CreateAsync(offer);


                if (offerAdded != null)
                {
                    Notification n = new Notification();
                    var tender  = await _context.Tender.Include(t => t.Institute).Where(t => t.Id == offer.TenderId).FirstOrDefaultAsync();
                    var institute = await _context.Institute.FindAsync(tender.instituteId);
                    var offerCompleted =  _context.Offer.Include(o => o.Supplier).Where(o => o.Id == offerAdded.Id).FirstOrDefaultAsync();
                    n.OfferId = offerAdded.Id;
                    n.Institute = institute;
                    n.InstituteId =(int) tender.instituteId;
                    n.message = "Offer";
                    n.Offer = await offerCompleted;
                    await _notificationHubContext.Clients.Group("instituteNotificationCenter").SendAsync("Send", n);
                    await _context.Notification.AddAsync(n);
                    await _context.SaveChangesAsync();
                    return new Success(true, "message.success", offerAdded);

                }
            }

            

            return new Success(false, "message.User is empty");
        }

        // DELETE: api/Offers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOffer(int id)
        {
            var res = await _offerRepository.Delete(id);
            if (res == 200)
                return new Success(true, "message.success"); ;

            return new Success(false, "Tender not found");
        }

        private bool OfferExists(int id)
        {
            return _context.Offer.Any(e => e.Id == id);
        }
    }
}
