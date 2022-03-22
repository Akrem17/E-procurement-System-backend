﻿using E_proc.Models;
using E_proc.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_proc.Repositories.Implementations
{
    public class TenderRepository : ITenderRepository
    {

        private readonly AuthContext _dbContext;


        public TenderRepository(AuthContext dbContext)
        {
            _dbContext = dbContext; 
            
        }
        public async Task<Tender> CreateAsync(Tender tender)
        {
          var  representative = _dbContext.Representative.Where(r=>r.SocialSecurityNumber==tender.Responsible.SocialSecurityNumber).FirstOrDefault();       
            if (representative!=null)
            {
                tender.responsibleId = representative.Id;
                tender.Responsible = null;
            }
            var tenderResult = await _dbContext.Tender.AddAsync(tender);
            _dbContext.SaveChanges();


            return tender;

        }

        public async Task<int> Delete(int id)
        {

            var tender = await ReadById(id);
            if (tender != null)
            {
                var deletedUser = _dbContext.Tender.Remove(tender);
                await _dbContext.SaveChangesAsync();
                return 200;
            };
            return 404;

        }

        public Task<List<Tender>> FindBy(string? email, bool? confirmed, string? phone, DateTime? date)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Tender>> ReadAsync(int skip, int take)
        { 
            var tender = await _dbContext.Tender.Include(t=>t.AddressReceipt).Include(t => t.Institute) .Skip(skip).Take(take).ToArrayAsync();
            


            return tender;
        }


        //public async Task<Institute> GetInstituteOfTender(int id)
        //{
        //    var institute = await _dbContext.Institute.FirstOrDefaultAsync(user => user.Id == id);
        //    return institute;

        //}





        public int CountData()
        {

            var items =  _dbContext.Tender.Count();



            return items;
        }

        public async Task<Tender> ReadById(int id)
        {

            return await _dbContext.Tender.Include(t => t.AddressReceipt).Include(t => t.Institute).Include(t=>t.TenderClassification).Include(t => t.Responsible).FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<Tender> UpdateAsync(int id, Tender tender)
        {
            var oldUser = await ReadById(id);

            if (oldUser != null)
            {
                
                oldUser.StartDate = tender.StartDate;oldUser.Financing = tender.Financing;oldUser.Budget=tender.Budget;oldUser.BusinessKind=tender.BusinessKind;oldUser.Departement=tender.Departement;oldUser.EvaluationMethod=tender.EvaluationMethod;oldUser.Name = tender.Name; oldUser.specificationURL = tender.specificationURL;
                oldUser.responsibleId = tender.responsibleId;   
                oldUser.updatedAt = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();

                    await _dbContext.SaveChangesAsync();
                    return oldUser;
                }
              
                    return null;

                
            }
          
      
    }
}
