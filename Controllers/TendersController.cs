﻿#nullable disable
using Microsoft.AspNetCore.Mvc;
using E_proc.Models;
using E_proc.Repositories.Interfaces;
using E_proc.Models.StatusModel;

namespace E_proc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TendersController : ControllerBase
    {
        private readonly AuthContext _context;
        private readonly ITenderRepository _reposTender;

        public TendersController(AuthContext context,ITenderRepository reposTender)
        {

            _reposTender = reposTender;
            _context = context;

        }


        // GET: api/Tenders
        [HttpGet]
        public async Task<IActionResult> GetTenders(int? skip = 0, int? take = 10, string? bidNumber = null, string? bidName = null,string? city =null,string? postDate=null)
        {
            if (bidNumber == null && bidName == null && city == null && postDate == null)
            {

            

                var tenders = await _reposTender.ReadAsync((int)skip, (int)take);

            if (tenders == null) return new Success(false, "message.UserNotFound");

            var items = _reposTender.CountData();

            return new Success(true, "message.success", new { tenders, items });


        }
            else
            {
                var date = Convert.ToDateTime(postDate).ToUniversalTime();
                var institutes = await _reposTender.FindBy(bidNumber, bidName,city, postDate);
                if (institutes.Count() != 0)
                {
                    return new Success(true, "message.sucess", institutes);

                }
                return new Success(false, "message.not found");

            }
        }

        // GET: api/Tenders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTender(int id)
        {
            var tender = await _reposTender.ReadById(id);
       

            if (tender == null) return new Success(false, "message.Usernot found");
            return new Success(true, "message.success",  new { tender, tender.Offers });
        }

        // PUT: api/Tenders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTender(int id, Tender tender)
        {
            var newUser = await _reposTender.UpdateAsync(id, tender);

            if (newUser == null)
                return new Success(false, "message.User not found ");
            return new Success(true, "message.success", newUser);
        }

        // POST: api/Tenders
        [HttpPost]
        public async Task<IActionResult> PostTender([FromBody] Tender tender)
        {
            if (tender != null)
            {

                //verify institute
                var status = await _reposTender.CreateAsync(tender);


                if (status!=null)
                return new Success(true, "message.success", tender);

            }


            return new Success(false, "message.User is empty");

        }

        // DELETE: api/Tenders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTender(int id)
        {
            var res = await _reposTender.Delete(id);
            if (res == 200)
                return new Success(true, "message.success"); ;

            return new Success(false, "Tender not found");

        }

        private bool TenderExists(int id)
        {
            return _context.Tender.Any(e => e.Id == id);
        }


        

        [HttpGet("{id}/extract")]
        public async Task<IActionResult> ExtractResult(int id)
        {

        var tender = _context.Tender.Where(o => o.Id == id).FirstOrDefault();
            var date = tender.DeadLine;
            string[] st = date.Split("T");

            string[] d = st[0].Split("-");

            if ((Int16.Parse(d[1]) >= new DateTimeOffset(DateTime.UtcNow).Month) && (Int16.Parse(d[2]) > new DateTimeOffset(DateTime.UtcNow).Day))
            {
                return new Success(false, "tender is not closed yet");

            }
            var notReviwedOffers =  _context.Offer.Where(o => o.TenderId == id && o.isAccepted==null).ToList();  
            if(notReviwedOffers.Count()> 0)
            {
                return new Success(false, "There are offers not reviewd yet");

            }
            var offerWin=  _context.Offer.Where(o => o.TenderId == id && o.isAccepted==true).OrderBy(o=>o.TotalMontant).FirstOrDefault();

            return new Success(true, "Success", offerWin);

        }

    }
}
